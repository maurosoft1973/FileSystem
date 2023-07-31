using Microsoft.OpenApi.Models;
using MinimalHelpers.OpenApi;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
    options.AddMissingSchemas();

    // Enable Swagger integration for custom parameters.
});

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseHttpsRedirection();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapGet("/api/sample", () =>
{
    return TypedResults.NoContent();
})
;

app.Run();

public enum Environment
{
    Development,
    Staging,
    Production
}