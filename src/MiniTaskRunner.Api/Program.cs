using Microsoft.EntityFrameworkCore;
using MiniTaskRunner.Core.Abstractions;
using MiniTaskRunner.Infrastructure.Persistence;
//using MiniTaskRunner.Infrastructure.Services;
using Serilog;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File("logs/api.log", rollingInterval: RollingInterval.Day)
    .Enrich.FromLogContext()
    .CreateLogger();


var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog();

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

// Add CORS for linking backend to frontend
builder.Services.AddCors(options =>
{
    options.AddPolicy("DashboardPolicy", policy =>
    {
        policy.WithOrigins("http://localhost:5173")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

//Enable CORS before MapController
app.UseCors("DashboardPolicy");

// Map controllers
app.MapControllers();

// Simple health endpoint (optional but useful)
app.MapGet("/", () => "Mini Task Runner API is running.")
   .WithName("Root");

app.Run();
