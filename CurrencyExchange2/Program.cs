global using Microsoft.EntityFrameworkCore;
global using Microsoft.EntityFrameworkCore.Design;
using CurrencyExchange.Core.Repositories;
using CurrencyExchange.Core.UnitOfWorks;
using CurrencyExchange.Repository;
using CurrencyExchange.Repository.Repositories;
using CurrencyExchange.Repository.UnitOfWorks;
using Serilog;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.



builder.Services.AddControllers();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
// builder.Services.AddScoped(typeof(IService<>), typeof(GenericRepository<>));

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"), options =>
    {
        options.MigrationsAssembly(Assembly.GetAssembly(typeof(ApplicationDbContext)).GetName().Name);
    });
});


var _loggrer = new LoggerConfiguration()
.ReadFrom.Configuration(builder.Configuration).Enrich.FromLogContext()
// .MinimumLevel.Error()
// .WriteTo.File("F:\\LaernCore\\Logs\\ApiLog-.log",rollingInterval:RollingInterval.Day)
.CreateLogger();
builder.Logging.AddSerilog(_loggrer);


// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();




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
