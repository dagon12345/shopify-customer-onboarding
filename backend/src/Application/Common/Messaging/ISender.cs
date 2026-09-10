namespace Application.Common.Messaging;

/// <summary>Dispatches a command to its registered handler, running validation first.</summary>
public interface ISender
{
    Task<TResponse> Send<TResponse>(ICommand<TResponse> command, CancellationToken cancellationToken);
}
