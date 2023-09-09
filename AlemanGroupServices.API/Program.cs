using AlemanGroupServices.Core;
using AlemanGroupServices.Core.Models;
using AlemanGroupServices.Core.Repositories;
using AlemanGroupServices.EF;
using AlemanGroupServices.EF.Repositories;
using Microsoft.EntityFrameworkCore;
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
builder.Services.AddAutoMapper(typeof(StationMapper));
builder.Services.AddAutoMapper(typeof(MarketingCompanyMapper));
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<MySQLDBContext>();
        context.Database.EnsureCreated();
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred creating the DB.");
    }
}

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
