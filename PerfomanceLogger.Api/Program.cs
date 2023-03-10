using PerfomanceLogger.Api.Services;
using PerfomanceLogger.Api.Middlewares;
using PerfomanceLogger.Domain.Interfaces;
using PerfomanceLogger.Infrastructure.Context;
using PerfomanceLogger.Infrastructure.Repositories;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddPerfomanceLoggerContext(builder.Configuration);

builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IDocumentService, DocumentService>();
builder.Services.AddScoped<IPerfomanceRepository, PerfomanceRepository>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<ExceptionHandlerMiddleware>();

app.MapControllers();

app.Run();
