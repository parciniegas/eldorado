using Inventory.Services; // Added for InventoryDbContext and ProductService
using Microsoft.EntityFrameworkCore; // Added for UseSqlServer
using Inventory.Endpoints; // Added for endpoint handlers

var builder = WebApplication.CreateBuilder(args);
// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add Entity Framework Core
builder.Services.AddDbContext<InventoryDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Register ProductService
builder.Services.AddScoped<IProductServices, ProductService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Map product endpoints
GetProductByIdHandler.Map(app);
CreateProductHandler.Map(app);
UpdateProductHandler.Map(app);
ReserveProductHandler.Map(app);
ReleaseReserveHandler.Map(app);
ConfirmReserveHandler.Map(app);
// TODO: Add other handlers for DeleteProduct and GetAllProducts

app.Run();
