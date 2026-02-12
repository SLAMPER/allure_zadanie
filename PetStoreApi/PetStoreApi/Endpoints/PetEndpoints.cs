using PetStoreApi.Infrastructure;
using PetStoreApi.Models;

namespace PetStoreApi.Endpoints;

public static class PetEndpoints
{
    public static RouteGroupBuilder MapPetEndpoints(this RouteGroupBuilder v2)
    {
        v2.MapPost("/pet/{petId:long}/uploadImage", async (HttpRequest request, long petId, InMemoryDataStore store) =>
        {
            var form = await request.ReadFormAsync();
            var metadata = form["additionalMetadata"].ToString();
            var file = form.Files.GetFile("file");

            var response = new ApiResponse
            {
                Code = 200,
                Type = "uploaded",
                Message = $"petId={petId}, metadata={metadata}, size={file?.Length ?? 0}"
            };

            return request.Respond(response, store.JsonOptions);
        });

        v2.MapPost("/pet", async (HttpRequest request, InMemoryDataStore store) =>
        {
            var pet = await request.ReadBodyAsync<Pet>(store.JsonOptions);
            if (pet is null || string.IsNullOrWhiteSpace(pet.Name) || pet.PhotoUrls.Count == 0)
                return Results.StatusCode(StatusCodes.Status405MethodNotAllowed);

            pet.Id ??= HttpHelpers.NextId(store.Pets.Keys);
            store.Pets[pet.Id.Value] = pet;
            return request.Respond(pet, store.JsonOptions);
        });

        v2.MapPut("/pet", async (HttpRequest request, InMemoryDataStore store) =>
        {
            var pet = await request.ReadBodyAsync<Pet>(store.JsonOptions);
            if (pet?.Id is null || pet.Id <= 0) return Results.BadRequest();

            if (!store.Pets.ContainsKey(pet.Id.Value)) return Results.NotFound();

            if (string.IsNullOrWhiteSpace(pet.Name) || pet.PhotoUrls.Count == 0)
                return Results.StatusCode(StatusCodes.Status405MethodNotAllowed);

            store.Pets[pet.Id.Value] = pet;
            return request.Respond(pet, store.JsonOptions);
        });

        v2.MapGet("/pet/findByStatus", (HttpRequest request, InMemoryDataStore store) =>
        {
            var statuses = HttpHelpers.ParseMultiQuery(request.Query["status"]);
            if (statuses.Count == 0 || statuses.Any(s => !store.PetStatuses.Contains(s))) return Results.BadRequest();

            var result = store.Pets.Values
                .Where(p => p.Status is not null && statuses.Contains(p.Status))
                .ToList();

            return request.Respond(result, store.JsonOptions);
        });

        v2.MapGet("/pet/findByTags", (HttpRequest request, InMemoryDataStore store) =>
        {
            var tags = HttpHelpers.ParseMultiQuery(request.Query["tags"]);
            if (tags.Count == 0) return Results.BadRequest();

            var result = store.Pets.Values
                .Where(p => p.Tags is not null && p.Tags.Any(t => t.Name is not null && tags.Contains(t.Name)))
                .ToList();

            return request.Respond(result, store.JsonOptions);
        });

        v2.MapGet("/pet/{petId:long}", (HttpRequest request, long petId, InMemoryDataStore store) =>
        {
            if (petId <= 0) return Results.BadRequest();

            if (!store.Pets.TryGetValue(petId, out var pet)) return Results.NotFound();

            return request.Respond(pet, store.JsonOptions);
        });

        v2.MapPost("/pet/{petId:long}", async (long petId, HttpRequest request, InMemoryDataStore store) =>
        {
            var form = await request.ReadFormAsync();
            if (!store.Pets.TryGetValue(petId, out var pet)) return Results.StatusCode(StatusCodes.Status405MethodNotAllowed);

            var name = form["name"].ToString();
            var status = form["status"].ToString();

            if (!string.IsNullOrWhiteSpace(name)) pet.Name = name;

            if (!string.IsNullOrWhiteSpace(status)) pet.Status = status;

            store.Pets[petId] = pet;
            return Results.Ok();
        });

        v2.MapDelete("/pet/{petId:long}", (long petId, InMemoryDataStore store) =>
        {
            if (petId <= 0) return Results.BadRequest();

            if (!store.Pets.TryRemove(petId, out _)) return Results.NotFound();

            return Results.Ok();
        });

        return v2;
    }
}