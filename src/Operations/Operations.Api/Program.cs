using Microsoft.EntityFrameworkCore;
using Operations.Api.Endpoints;
using Operations.Api.Services;
using Operations.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<OperationsContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddKeyedScoped<IRepository, RedisRepository>("redis");
builder.Services.AddKeyedScoped<IRepository, SqlServerRepository>("sql");
builder.Services.AddScoped<IOperationsService, OperationsService>();  

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

ProcessOperationHandler.Map(app);

app.Run();