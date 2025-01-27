using Microsoft.EntityFrameworkCore;
using VehicleTrackingApp.Data;
using VehicleTrackingApp.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//Here is our DbContext
builder.Services.AddDbContext<VehicleTrackingContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("VehicleTrackingDB")));

// here is we Register our  repositories
builder.Services.AddScoped(typeof(IUserRepository), typeof(UserRepository));
builder.Services.AddScoped(typeof(IUserVehicleRepository), typeof(UserVehicleRepository));


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
