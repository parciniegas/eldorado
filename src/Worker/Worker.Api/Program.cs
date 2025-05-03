using Common.Operations;
using Infrastructure.Repositories;
using Worker.Api.Endpoints;
using Worker.Api.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// builder.Services.AddDbContext<OperationsContext>(options =>
//     options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddScoped<IOperationsRepository, RedisRepository>();
builder.Services.AddScoped<IOperationsService, OperationsService>();  

var connectionString = builder.Configuration.GetConnectionString("RedisConnection");

var app = builder.Build();

// using (var scope = app.Services.CreateScope())
// {
//     var services = scope.ServiceProvider;
//     var context = services.GetRequiredService<OperationsContext>();
//     context.Database.EnsureCreated();
// }

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

ProcessOperationHandler.Map(app);

app.Run();
