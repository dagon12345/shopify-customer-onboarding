using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using ValidationException = Application.Common.Exceptions.ValidationException;

namespace Application.Common.Messaging;

/// <summary>
/// Minimal in-process command dispatcher: resolves the matching <see cref="ICommandHandler{TCommand,TResponse}"/>
/// from DI, runs any registered FluentValidation validators first, then invokes the handler.
/// A lightweight, dependency-free stand-in for a full mediator library.
/// </summary>
public sealed class Sender : ISender
{
    private readonly IServiceProvider _serviceProvider;

    public Sender(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task<TResponse> Send<TResponse>(ICommand<TResponse> command, CancellationToken cancellationToken)
    {
        var commandType = command.GetType();

        var validatorType = typeof(IValidator<>).MakeGenericType(commandType);
        var validators = _serviceProvider.GetServices(validatorType).Cast<IValidator>().ToList();

        if (validators.Count > 0)
        {
            var context = new ValidationContext<object>(command);
            var failures = new List<FluentValidation.Results.ValidationFailure>();

            foreach (var validator in validators)
            {
                var result = await validator.ValidateAsync(context, cancellationToken);
                failures.AddRange(result.Errors);
            }

            if (failures.Count > 0)
                throw new ValidationException(failures);
        }

        var handlerType = typeof(ICommandHandler<,>).MakeGenericType(commandType, typeof(TResponse));
        dynamic handler = _serviceProvider.GetRequiredService(handlerType);

        return await handler.Handle((dynamic)command, cancellationToken);
    }
}
