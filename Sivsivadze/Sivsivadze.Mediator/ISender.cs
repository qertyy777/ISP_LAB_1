namespace Sivsivadze.Mediator;

public interface ISender
{
    void Send<TRequest>(TRequest request)
        where TRequest : IRequest;

    TResponse Send<TResponse>(IRequest<TResponse> request);
}
