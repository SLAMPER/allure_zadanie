using PetStoreApi.Infrastructure;
using PetStoreApi.Models;

namespace PetStoreApi.Endpoints;

public static class StoreEndpoints
{
    public static RouteGroupBuilder MapStoreEndpoints(this RouteGroupBuilder v2)
    {
        v2.MapGet("/store/inventory", (InMemoryDataStore store) =>
        {
            var inventory = store.Pets.Values
                .Where(p => !string.IsNullOrWhiteSpace(p.Status))
                .GroupBy(p => p.Status!)
                .ToDictionary(g => g.Key, g => g.Count(), StringComparer.Ordinal);

            return Results.Json(inventory, store.JsonOptions);
        });

        v2.MapPost("/store/order", async (HttpRequest request, InMemoryDataStore store) =>
        {
            var order = await request.ReadBodyAsync<Order>(store.JsonOptions);
            if (order is null) return Results.StatusCode(StatusCodes.Status500InternalServerError);

            order.Id ??= HttpHelpers.NextId(store.Orders.Keys);
            store.Orders[order.Id.Value] = order;
            return request.Respond(order, store.JsonOptions);
        });

        v2.MapGet("/store/order/{orderId:long}", (HttpRequest request, long orderId, InMemoryDataStore store) =>
        {
            if (orderId < 1) return Results.BadRequest();

            if (!store.Orders.TryGetValue(orderId, out var order)) return Results.NotFound();

            return request.Respond(order, store.JsonOptions);
        });

        v2.MapDelete("/store/order/{orderId:long}", (long orderId, InMemoryDataStore store) =>
        {
            if (orderId < 1) return Results.BadRequest();

            if (!store.Orders.TryRemove(orderId, out _)) return Results.NotFound();

            return Results.Ok();
        });

        return v2;
    }
}
