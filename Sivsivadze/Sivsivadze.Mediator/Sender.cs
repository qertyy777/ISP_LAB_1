using System.Reflection;

namespace Sivsivadze.Mediator;

public class Sender : ISender
{
    private readonly Assembly _assembly;
    private readonly Dictionary<Type, Type> _handlerImplementations = new();

    public Sender(Assembly? assembly = null)
    {
        _assembly = assembly ?? Assembly.GetCallingAssembly();
        RegisterHandlers(_assembly);
    }

    private void RegisterHandlers(Assembly assembly)
    {
        var handlers = assembly
            .GetTypes()
            .Where(type => type is { IsAbstract: false, IsClass: true })
            .SelectMany(
                type => type.GetInterfaces()
                    .Where(IsRequestHandlerInterface)
                    .Select(handlerInterface => new { HandlerInterface = handlerInterface, Implementation = type }));

        foreach (var handler in handlers)
        {
            _handlerImplementations[handler.HandlerInterface] = handler.Implementation;
        }
    }

    public void Send<TRequest>(TRequest request)
        where TRequest : IRequest
    {
        Type handlerType = typeof(IRequestHandler<>).MakeGenericType(request.GetType());
        dynamic handler = CreateHandler(handlerType);
        handler.Handle((dynamic)request).GetAwaiter().GetResult();
    }

    public TResponse Send<TResponse>(IRequest<TResponse> request)
    {
        Type handlerType = typeof(IRequestHandler<,>).MakeGenericType(request.GetType(), typeof(TResponse));
        dynamic handler = CreateHandler(handlerType);
        return handler.Handle((dynamic)request).GetAwaiter().GetResult();
    }

    private dynamic CreateHandler(Type handlerType)
    {
        if (!_handlerImplementations.TryGetValue(handlerType, out Type? implementationType))
        {
            throw new InvalidOperationException($"Handler for {handlerType.Name} not found.");
        }

        return Activator.CreateInstance(implementationType)
               ?? throw new InvalidOperationException($"Cannot create handler {implementationType.Name}.");
    }

    private static bool IsRequestHandlerInterface(Type type)
    {
        if (!type.IsGenericType)
        {
            return false;
        }

        Type genericType = type.GetGenericTypeDefinition();
        return genericType == typeof(IRequestHandler<>)
               || genericType == typeof(IRequestHandler<,>);
    }
}
