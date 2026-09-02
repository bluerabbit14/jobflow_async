using System.Text.Json;
using JobFlow.Application.Workers;
using JobFlow.Infrastructure.Messaging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using WorkerEntity = JobFlow.Domain.Entities.Worker;

namespace JobFlow.Worker;

public class JobConsumer : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly RabbitMqOptions _options;
    private readonly ILogger<JobConsumer> _logger;

    private IConnection? _connection;
    private IChannel? _channel;
    private Guid _workerId;

    public JobConsumer(
        IServiceScopeFactory scopeFactory,
        IOptions<RabbitMqOptions> options,
        ILogger<JobConsumer> logger)
    {
        _scopeFactory = scopeFactory;
        _options = options.Value;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await RegisterWorkerAsync(stoppingToken);
        await ConnectAndConsumeAsync(stoppingToken);

        try
        {
            await Task.Delay(Timeout.Infinite, stoppingToken);
        }
        catch (OperationCanceledException)
        {
            _logger.LogInformation("Job consumer is stopping.");
        }
    }

    private async Task RegisterWorkerAsync(CancellationToken cancellationToken)
    {
        using var scope = _scopeFactory.CreateScope();
        var workers = scope.ServiceProvider.GetRequiredService<IWorkerRepository>();

        var worker = WorkerEntity.Create(Environment.MachineName);
        await workers.AddAsync(worker, cancellationToken);
        _workerId = worker.Id;

        _logger.LogInformation(
            "Registered worker {WorkerId} on host {Hostname}",
            worker.Id,
            worker.Hostname);
    }

    private async Task ConnectAndConsumeAsync(CancellationToken cancellationToken)
    {
        var factory = new ConnectionFactory
        {
            HostName = _options.Host,
            Port = _options.Port,
            UserName = _options.Username,
            Password = _options.Password
        };

        _connection = await factory.CreateConnectionAsync(cancellationToken);
        _channel = await _connection.CreateChannelAsync(cancellationToken: cancellationToken);

        await _channel.QueueDeclareAsync(
            queue: _options.QueueName,
            durable: true,
            exclusive: false,
            autoDelete: false,
            cancellationToken: cancellationToken);

        await _channel.BasicQosAsync(
            prefetchSize: 0,
            prefetchCount: 1,
            global: false,
            cancellationToken: cancellationToken);

        var consumer = new AsyncEventingBasicConsumer(_channel);
        consumer.ReceivedAsync += OnMessageReceivedAsync;

        await _channel.BasicConsumeAsync(
            queue: _options.QueueName,
            autoAck: false,
            consumer: consumer,
            cancellationToken: cancellationToken);

        _logger.LogInformation("Listening for jobs on queue {QueueName}", _options.QueueName);
    }

    private async Task OnMessageReceivedAsync(object sender, BasicDeliverEventArgs eventArgs)
    {
        try
        {
            var message = JsonSerializer.Deserialize<JobMessage>(eventArgs.Body.Span);
            if (message is null || message.JobId == Guid.Empty)
            {
                _logger.LogWarning("Received an invalid job message. Rejecting without requeue.");
                await _channel!.BasicNackAsync(eventArgs.DeliveryTag, multiple: false, requeue: false);
                return;
            }

            using var scope = _scopeFactory.CreateScope();
            var processor = scope.ServiceProvider.GetRequiredService<IJobProcessor>();
            await processor.ProcessAsync(message.JobId, _workerId, CancellationToken.None);

            await _channel!.BasicAckAsync(eventArgs.DeliveryTag, multiple: false);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to process job message. The message will be nacked.");

            if (_channel is not null)
            {
                await _channel.BasicNackAsync(eventArgs.DeliveryTag, multiple: false, requeue: true);
            }
        }
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        if (_channel is not null)
        {
            await _channel.CloseAsync();
        }

        if (_connection is not null)
        {
            await _connection.CloseAsync();
        }

        await base.StopAsync(cancellationToken);
    }
}
