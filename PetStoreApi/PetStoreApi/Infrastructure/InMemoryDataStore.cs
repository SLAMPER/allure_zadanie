using System.Collections.Concurrent;
using System.Text.Json;
using PetStoreApi.Models;

namespace PetStoreApi.Infrastructure;

public sealed class InMemoryDataStore
{
    public ConcurrentDictionary<long, Pet> Pets { get; } = new();
    public ConcurrentDictionary<long, Order> Orders { get; } = new();
    public ConcurrentDictionary<string, User> Users { get; } = new(StringComparer.Ordinal);
    public JsonSerializerOptions JsonOptions { get; } = new(JsonSerializerDefaults.Web);
    public HashSet<string> PetStatuses { get; } = new(StringComparer.Ordinal) { "available", "pending", "sold" };
}