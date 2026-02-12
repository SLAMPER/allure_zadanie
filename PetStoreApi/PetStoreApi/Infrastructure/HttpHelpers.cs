using System.Text;
using System.Text.Json;
using System.Xml.Serialization;
using Microsoft.Extensions.Primitives;

namespace PetStoreApi.Infrastructure;

public static class HttpHelpers
{
    public static async Task<T?> ReadBodyAsync<T>(this HttpRequest request, JsonSerializerOptions jsonOptions) where T : class
    {
        if (request.ContentLength == 0) return default;

        try
        {
            if (request.ContentType?.Contains("xml", StringComparison.OrdinalIgnoreCase) == true)
            {
                var serializer = new XmlSerializer(typeof(T));
                return serializer.Deserialize(request.Body) as T;
            }

            return await request.ReadFromJsonAsync<T>(jsonOptions);
        }
        catch (JsonException)
        {
            return default;
        }
        catch (InvalidOperationException)
        {
            return default;
        }
    }

    public static IResult Respond(this HttpRequest request, object payload, JsonSerializerOptions jsonOptions)
    {
        if (request.Headers.Accept.Any(a => a is not null && a.Contains("application/xml", StringComparison.OrdinalIgnoreCase)))
            return Results.Content(SerializeXml(payload), "application/xml", Encoding.UTF8);

        return Results.Json(payload, jsonOptions);
    }

    public static long NextId(IEnumerable<long> keys)
    {
        var max = 0L;
        foreach (var key in keys)
            if (key > max)
                max = key;

        return max + 1;
    }

    public static HashSet<string> ParseMultiQuery(StringValues values)
    {
        var result = new HashSet<string>(StringComparer.Ordinal);
        foreach (var value in values)
        {
            if (string.IsNullOrWhiteSpace(value)) continue;

            foreach (var part in value.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)) result.Add(part);
        }

        return result;
    }

    private static string SerializeXml(object payload)
    {
        var serializer = new XmlSerializer(payload.GetType());
        using var sw = new StringWriter();
        serializer.Serialize(sw, payload);
        return sw.ToString();
    }
}