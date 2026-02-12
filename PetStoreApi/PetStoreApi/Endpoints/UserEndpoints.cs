using PetStoreApi.Infrastructure;
using PetStoreApi.Models;

namespace PetStoreApi.Endpoints;

public static class UserEndpoints
{
    public static RouteGroupBuilder MapUserEndpoints(this RouteGroupBuilder v2)
    {
        v2.MapPost("/user/createWithList", async (HttpRequest request, InMemoryDataStore store) =>
        {
            var users = await request.ReadBodyAsync<List<User>>(store.JsonOptions) ?? [];
            foreach (var user in users.Where(u => !string.IsNullOrWhiteSpace(u.Username))) store.Users[user.Username!] = user;

            return Results.Ok();
        });

        v2.MapGet("/user/{username}", (HttpRequest request, string username, InMemoryDataStore store) =>
        {
            if (string.IsNullOrWhiteSpace(username)) return Results.BadRequest();

            if (!store.Users.TryGetValue(username, out var user)) return Results.NotFound();

            return request.Respond(user, store.JsonOptions);
        });

        v2.MapPut("/user/{username}", async (string username, HttpRequest request, InMemoryDataStore store) =>
        {
            if (string.IsNullOrWhiteSpace(username)) return Results.BadRequest();

            var user = await request.ReadBodyAsync<User>(store.JsonOptions);
            if (user is null) return Results.BadRequest();

            user.Username = username;
            store.Users[username] = user;
            return Results.Ok();
        });

        v2.MapDelete("/user/{username}", (string username, InMemoryDataStore store) =>
        {
            if (string.IsNullOrWhiteSpace(username)) return Results.BadRequest();

            if (!store.Users.TryRemove(username, out _)) return Results.NotFound();

            return Results.Ok();
        });

        v2.MapGet("/user/login",
            (HttpRequest request, HttpResponse response, string? username, string? password, InMemoryDataStore store) =>
            {
                if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password)) return Results.BadRequest();

                response.Headers.Append("X-Rate-Limit", "5000");
                response.Headers.Append("X-Expires-After", DateTime.UtcNow.AddHours(1).ToString("O"));
                return request.Respond($"logged in user session:{Guid.NewGuid()}", store.JsonOptions);
            });

        v2.MapGet("/user/logout", () => Results.Ok());

        v2.MapPost("/user/createWithArray", async (HttpRequest request, InMemoryDataStore store) =>
        {
            var users = await request.ReadBodyAsync<List<User>>(store.JsonOptions) ?? [];
            foreach (var user in users.Where(u => !string.IsNullOrWhiteSpace(u.Username))) store.Users[user.Username!] = user;

            return Results.Ok();
        });

        v2.MapPost("/user", async (HttpRequest request, InMemoryDataStore store) =>
        {
            var user = await request.ReadBodyAsync<User>(store.JsonOptions);
            if (user is null || string.IsNullOrWhiteSpace(user.Username)) return Results.BadRequest();

            store.Users[user.Username] = user;
            return Results.Ok();
        });

        return v2;
    }
}