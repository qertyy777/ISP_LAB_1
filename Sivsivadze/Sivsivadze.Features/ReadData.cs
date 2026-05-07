using System.Text.Json;
using Sivsivadze.Domain;
using Sivsivadze.Mediator;

namespace Sivsivadze.Features;

public record ReadDataRequest(string FileName) : IRequest<IEnumerable<KaitenTask>>;

public class ReadDataRequestHandler : IRequestHandler<ReadDataRequest, IEnumerable<KaitenTask>>
{
    public async Task<IEnumerable<KaitenTask>> Handle(
        ReadDataRequest request,
        CancellationToken? cancellationToken = default)
    {
        if (!File.Exists(request.FileName))
        {
            return [];
        }

        await using FileStream fileStream = File.OpenRead(request.FileName);
        return await JsonSerializer.DeserializeAsync<List<KaitenTask>>(
                   fileStream,
                   cancellationToken: cancellationToken ?? CancellationToken.None)
               ?? [];
    }
}
