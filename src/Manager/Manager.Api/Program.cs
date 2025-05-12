using Common.Operations;
using Infrastructure.Repositories;
using Manager.Api.Endpoints;
using Manager.Api.Services;
using Manager.Api.Utils;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<OperationsContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddScoped<OperationsHttpClient>();
builder.Services.AddScoped<ConstraintHttpClient>();
builder.Services.AddKeyedScoped<IOperationsRepository, RedisRepository>("RedisKey");
builder.Services.AddKeyedScoped<IOperationsRepository, SqlServerRepository>("SqlKey");
builder.Services.AddKeyedScoped<ISyncProcesor, SyncProcessorWithEvents>("SyncProcessorWithEvents");
builder.Services.AddKeyedScoped<ISyncProcesor, SyncProcessorWithRequest>("SyncProcessorWithRequest");

var connectionString = builder.Configuration.GetConnectionString("RedisConnection");

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<OperationsContext>();
    context.Database.EnsureCreated();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

ManageOperationsHandler.Map(app);

app.Run();
