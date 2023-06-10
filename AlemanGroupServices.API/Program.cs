using Microsoft.EntityFrameworkCore;
using AlemanGroupServices.EF;
using AlemanGroupServices.Core.Repositories;
using AlemanGroupServices.EF.Repositories;
using AlemanGroupServices.Core;
using Swashbuckle.AspNetCore.SwaggerUI;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
var connectionString =
    builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<MySQLDBContext>(options =>
{
    options.UseMySql(connectionString, ServerVersion.Parse("8.0.28-mysql"));
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddSingleton<IDataAccess, DataAccess>();
builder.Services.AddTransient<IStationUnitOfWork, StationUnitOfWork>();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Aleman Group API V1");
        c.DocExpansion(DocExpansion.None);
    });
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
