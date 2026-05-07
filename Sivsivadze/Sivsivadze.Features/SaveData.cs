using System.Text.Json;
using Sivsivadze.Domain;
using Sivsivadze.Mediator;

namespace Sivsivadze.Features;

public record SaveDataRequest(IEnumerable<KaitenTask> Data, string FileName) : IRequest;

public class SaveDataRequestHandler : IRequestHandler<SaveDataRequest>
{
    public async Task Handle(SaveDataRequest request, CancellationToken? cancellationToken = default)
    {
        string? directory = Path.GetDirectoryName(request.FileName);
        if (!string.IsNullOrWhiteSpace(directory))
        {
            Directory.CreateDirectory(directory);
        }

        var options = new JsonSerializerOptions
        {
            WriteIndented = true
        };

        await using FileStream fileStream = File.Create(request.FileName);
        await JsonSerializer.SerializeAsync(
            fileStream,
            request.Data,
            options,
            cancellationToken ?? CancellationToken.None);
    }
}
