using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using TIKSN.Integration.Messages;
using Xunit;

namespace TIKSN.Tests.Integration.Messages;

public class ValidationBehaviorTests
{
    [Fact]
    public async Task Handle_InvalidRequest_ShouldThrowValidationException()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddSingleton<IValidator<TestRequest>, TestRequestValidator>();
        var serviceProvider = services.BuildServiceProvider();

        var behavior = new ValidationBehavior<TestRequest, TestResponse>(serviceProvider);

        var request = new TestRequest { Name = "" }; // Invalid
        var next = new RequestHandlerDelegate<TestResponse>(ct => Task.FromResult(new TestResponse()));

        // Act & Assert
        var exception =
            await Should.ThrowAsync<ValidationException>(() => behavior.Handle(request, next, CancellationToken.None));
        exception.Errors.ShouldNotBeEmpty();
    }

    [Fact]
    public async Task Handle_InvalidResponse_ShouldThrowValidationException()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddSingleton<IValidator<TestRequest>, TestRequestValidator>();
        services.AddSingleton<IValidator<TestResponse>, TestResponseValidator>();
        var serviceProvider = services.BuildServiceProvider();

        var behavior = new ValidationBehavior<TestRequest, TestResponse>(serviceProvider);

        var request = new TestRequest { Name = "Valid" };
        var response = new TestResponse { ResponseData = "" }; // Invalid

        var next = new RequestHandlerDelegate<TestResponse>(ct => Task.FromResult(response));

        // Act & Assert
        var exception =
            await Should.ThrowAsync<ValidationException>(() => behavior.Handle(request, next, CancellationToken.None));
        exception.Errors.ShouldNotBeEmpty();
    }

    [Fact]
    public async Task Handle_ValidRequestAndResponse_ShouldPass()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddSingleton<IValidator<TestRequest>, TestRequestValidator>();
        services.AddSingleton<IValidator<TestResponse>, TestResponseValidator>();
        var serviceProvider = services.BuildServiceProvider();

        var behavior = new ValidationBehavior<TestRequest, TestResponse>(serviceProvider);

        var request = new TestRequest { Name = "Valid" };
        var response = new TestResponse { ResponseData = "Valid Response" };

        var next = new RequestHandlerDelegate<TestResponse>(ct => Task.FromResult(response));

        // Act
        var result = await behavior.Handle(request, next, CancellationToken.None);

        // Assert
        result.ShouldBeSameAs(response);
    }

    public class TestRequest : IRequest<TestResponse>
    {
        public string Name { get; set; } = string.Empty;
    }

    public class TestRequestValidator : AbstractValidator<TestRequest>
    {
        public TestRequestValidator() => this.RuleFor(x => x.Name).NotEmpty();
    }

    public class TestResponse
    {
        public string ResponseData { get; set; } = string.Empty;
    }

    public class TestResponseValidator : AbstractValidator<TestResponse>
    {
        public TestResponseValidator() => this.RuleFor(x => x.ResponseData).NotEmpty();
    }
}
