using FluentValidation.TestHelper;
using JobFlow.Application.Jobs.DTOs;
using JobFlow.Application.Jobs.Validators;

namespace JobFlow.UnitTests.Jobs;

public class CreateJobRequestValidatorTests
{
    private readonly CreateJobRequestValidator _validator = new();

    [Fact]
    public void Empty_type_is_invalid()
    {
        var result = _validator.TestValidate(new CreateJobRequest
        {
            Type = string.Empty,
            Payload = "{}"
        });

        result.ShouldHaveValidationErrorFor(x => x.Type);
    }

    [Fact]
    public void Empty_payload_is_invalid()
    {
        var result = _validator.TestValidate(new CreateJobRequest
        {
            Type = "EmailNotification",
            Payload = string.Empty
        });

        result.ShouldHaveValidationErrorFor(x => x.Payload);
    }

    [Fact]
    public void Type_longer_than_100_characters_is_invalid()
    {
        var result = _validator.TestValidate(new CreateJobRequest
        {
            Type = new string('a', 101),
            Payload = "{}"
        });

        result.ShouldHaveValidationErrorFor(x => x.Type);
    }

    [Fact]
    public void Payload_longer_than_10000_characters_is_invalid()
    {
        var result = _validator.TestValidate(new CreateJobRequest
        {
            Type = "EmailNotification",
            Payload = new string('x', 10001)
        });

        result.ShouldHaveValidationErrorFor(x => x.Payload);
    }

    [Fact]
    public void Valid_request_passes()
    {
        var result = _validator.TestValidate(new CreateJobRequest
        {
            Type = "EmailNotification",
            Payload = "{\"subject\":\"Welcome\"}"
        });

        result.ShouldNotHaveAnyValidationErrors();
    }
}
