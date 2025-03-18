using ElDorado.Api.Endpoints.Constraints.Add;
using ElDorado.Api.Extensions;
using ElDorado.Domain.Constraints;
using ElDorado.Domain.Constraints.Contracts;
using ElDorado.Infrastructure;
using ElDorado.Infrastructure.MessageBroker;
using ElDorado.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<ElDoradoDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("ElDorado")));

builder.Services.AddScoped<IConstraintRepository, ConstraintRepository>();
builder.Services.AddScoped<IConstraintManager, ConstraintManager>();
builder.Services.AddSingleton<IConstraintRemovedPublisher, ConstraintRemovedPublisher>();


var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<ElDoradoDbContext>();
    context.Database.EnsureCreated();
}
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapEndpoints();

await app.RunAsync();
