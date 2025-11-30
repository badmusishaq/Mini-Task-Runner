// using MiniTaskRunner.Core.Abstractions;
// using MiniTaskRunner.Infrastructure.Services;
// using MiniTaskRunner.Infrastructure.Persistence;
// using Microsoft.EntityFrameworkCore;

// var builder = WebApplication.CreateBuilder(args);

// // Add services to the container.
// // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
// builder.Services.AddOpenApi();
// builder.Services.AddScoped<IJobService, JobService>();

// builder.Services.AddDbContext<JobDbContext>(options =>
//     options.UseInMemoryDatabase("MiniTaskRunnerDb"));

// var app = builder.Build();

// // Configure the HTTP request pipeline.
// if (app.Environment.IsDevelopment())
// {
//     app.MapOpenApi();
// }

// app.UseHttpsRedirection();

// var summaries = new[]
// {
//     "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
// };

// app.MapGet("/weatherforecast", () =>
// {
//     var forecast =  Enumerable.Range(1, 5).Select(index =>
//         new WeatherForecast
//         (
//             DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
//             Random.Shared.Next(-20, 55),
//             summaries[Random.Shared.Next(summaries.Length)]
//         ))
//         .ToArray();
//     return forecast;
// })
// .WithName("GetWeatherForecast");

// app.Run();

// record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
// {
//     public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
// }

//SWAGGER DEFAULT
// var builder = WebApplication.CreateBuilder(args);

// builder.Services.AddEndpointsApiExplorer();
// builder.Services.AddSwaggerGen();

// var app = builder.Build();

// if (app.Environment.IsDevelopment())
// {
//     app.UseSwagger();
//     app.UseSwaggerUI();
// }

// app.UseHttpsRedirection();

// app.Run();

using Microsoft.EntityFrameworkCore;
using MiniTaskRunner.Core.Abstractions;
using MiniTaskRunner.Infrastructure.Persistence;
using MiniTaskRunner.Infrastructure.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// Swagger / OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// DbContext (InMemory for now; swap later for SQL Server/PostgreSQL)
builder.Services.AddDbContext<JobDbContext>(options =>
    options.UseInMemoryDatabase("MiniTaskRunnerDb"));

// Domain services
builder.Services.AddScoped<IJobService, JobService>();

// Controllers (if you’re using [ApiController] controllers)
builder.Services.AddControllers();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Map controllers
app.MapControllers();

// Simple health endpoint (optional but useful)
app.MapGet("/", () => "Mini Task Runner API is running.")
   .WithName("Root");

app.Run();
