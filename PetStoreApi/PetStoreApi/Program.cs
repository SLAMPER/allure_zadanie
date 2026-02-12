using PetStoreApi.Endpoints;
using PetStoreApi.Infrastructure;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<InMemoryDataStore>();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();
app.MapGet("/", () => Results.Redirect("/swagger"));

var v2 = app.MapGroup("/v2");
v2.MapPetEndpoints();
v2.MapStoreEndpoints();
v2.MapUserEndpoints();
app.MapOpenApi();

app.Run();
