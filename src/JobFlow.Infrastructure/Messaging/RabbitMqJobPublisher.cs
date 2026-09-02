using System.Text.Json;
using JobFlow.Application.Jobs;
using JobFlow.Domain.Entities;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;

namespace JobFlow.Infrastructure.Messaging;

public class RabbitMqJobPublisher : IJobPublisher, IAsyncDisposable
{
    private readonly RabbitMqOptions _options;
    private readonly ILogger<RabbitMqJobPublisher> _logger;
    private readonly SemaphoreSlim _gate = new(1, 1);

    private IConnection? _connection;
    private IChannel? _channel;

    public RabbitMqJobPublisher(
        IOptions<RabbitMqOptions> options,
        ILogger<RabbitMqJobPublisher> logger)
    {
        _options = options.Value;
        _logger = logger;
    }

    public async Task PublishAsync(Job job, CancellationToken cancellationToken)
    {
        await EnsureConnectedAsync(cancellationToken);

        var body = JsonSerializer.SerializeToUtf8Bytes(new JobMessage { JobId = job.Id });

        await _channel!.BasicPublishAsync(
            exchange: string.Empty,
            routingKey: _options.QueueName,
            body: body,
            cancellationToken: cancellationToken);

        _logger.LogInformation("Published job {JobId} to queue {QueueName}", job.Id, _options.QueueName);
    }

    private async Task EnsureConnectedAsync(CancellationToken cancellationToken)
    {
        if (_channel is { IsOpen: true })
        {
            return;
        }

        await _gate.WaitAsync(cancellationToken);
        try
        {
            if (_channel is { IsOpen: true })
            {
                return;
            }

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
        }
        finally
        {
            _gate.Release();
        }
    }

    public async ValueTask DisposeAsync()
    {
        if (_channel is not null)
        {
            await _channel.CloseAsync();
        }

        if (_connection is not null)
        {
            await _connection.CloseAsync();
        }

        _gate.Dispose();
    }
}
