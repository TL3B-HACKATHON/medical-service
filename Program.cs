using Carter;
using Marten;
using medical_profile_service.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<IStorageService, StorageService>();
builder.Services.AddSingleton<IDatabaseService, PostgresDatabaseService>();

builder.Services.AddMarten(config =>
{
	var host = builder.Configuration.GetValue<string>("Postgres:Host");
	var port = builder.Configuration.GetValue<string>("Postgres:Port");
	var database = builder.Configuration.GetValue<string>("Postgres:Database");
	var username = builder.Configuration.GetValue<string>("Postgres:Username");
	var password = builder.Configuration.GetValue<string>("Postgres:Password");
	config.Connection($"Host={host};Port={port};Database={database};Username={username};Password={password}");
});


builder.Services.AddCarter();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();
app.MapCarter();

app.MapGet("/", () => "Medical Profile Service is Working");

app.Run();