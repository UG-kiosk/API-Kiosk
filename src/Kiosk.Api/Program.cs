using Kiosk.Repositories;
using Kiosk.Repositories.Interfaces;
using KioskAPI.Services;
using KioskAPI.Services.Interfaces;
using Microsoft.AspNetCore.Rewrite;
using MongoDB.Driver;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetSection("KioskDatabase:ConnectionString").Value;
var databaseName = builder.Configuration.GetSection("KioskDatabase:DatabaseName").Value;

var database = new MongoClient(connectionString).GetDatabase(databaseName);

builder.Host.UseSerilog((context, configuration) =>
    configuration.ReadFrom.Configuration(context.Configuration));

builder.Services.AddSingleton(database);

builder.Services.AddControllers();

builder.Services.AddScoped<IEctsSubjectRepository, EctsSubjectRepository>()
    .AddScoped<IEctsSubjectService, EctsSubjectService>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    var option = new RewriteOptions();
    option.AddRedirect("^$", "swagger");
    app.UseRewriter(option);
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();