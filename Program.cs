using Microsoft.EntityFrameworkCore;
using TrabajoFinalRestaurante.Repository;
using TrabajoFinalRestaurante.Services;
using TrabajoFinalRestaurante.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();


builder.Services.AddControllers();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<ReservaRestaurantContext>(
options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IReservaService,ReservaService>();
builder.Services.AddScoped<IReservaRepository, ReservaRepository>();
builder.Services.AddScoped<ValidacionService>();

var app = builder.Build();



app.UseHttpsRedirection();

app.UseAuthorization();
app.UseSwagger();
app.UseSwaggerUI();

app.MapControllers();

app.Run();