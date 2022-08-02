using CurrencyExchange.API.Filters;
using CurrencyExchange.API.Middlewares;
using CurrencyExchange.Core.Repositories;
using CurrencyExchange.Core.Services;
using CurrencyExchange.Core.UnitOfWorks;
using CurrencyExchange.Repository;
using CurrencyExchange.Repository.Repositories;
using CurrencyExchange.Repository.UnitOfWorks;
using CurrencyExchange.Service.Services;
using CurrencyExchange.Service.Validations;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.


builder.Services.AddControllers(options => { options.Filters.Add(new ValidateFilterAttribute()); })
    .AddFluentValidation(x => x.RegisterValidatorsFromAssemblies(AppDomain.CurrentDomain.GetAssemblies()));

builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.SuppressModelStateInvalidFilter = true;
});
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
builder.Services.AddScoped(typeof(IPasswordRepository), typeof(PasswordRepository));
builder.Services.AddScoped(typeof(IAccountRepository), typeof(AccountRepository));
builder.Services.AddScoped(typeof(IBalanceRepository), typeof(BalanceRepository));
builder.Services.AddScoped(typeof(IUserRepository), typeof(UserRepository));
builder.Services.AddScoped(typeof(ITokenRepository), typeof(TokenRepository));
builder.Services.AddScoped(typeof(ICryptoCoinPriceRepository), typeof(CryptoCoinPriceRepository));
builder.Services.AddScoped(typeof(IUserBalanceHistoryRepository), typeof(UserBalanceHistoryRepository));
builder.Services.AddScoped(typeof(ICryptoCoinRepository), typeof(CryptoCoinRepository));
builder.Services.AddScoped(typeof(ISellCryptoCoinService<>), typeof(SellCryptoCoinService<>));

builder.Services.AddScoped(typeof(TokenControlFilter<>));
builder.Services.AddScoped(typeof(NotFoundFilter<>));

builder.Services.AddScoped(typeof(IBuyCryptoCoinService<>), typeof(BuyCryptoCoinService<>));
builder.Services.AddScoped(typeof(IService<>), typeof(Service<>));
builder.Services.AddScoped(typeof(IUserRegister<>), typeof(UserRegisterService<>));
builder.Services.AddScoped(typeof(ICryptoCoinService), typeof(CryptoCoinService));
builder.Services.AddScoped(typeof(ICryptoCoinPriceService), typeof(CryptoCoinPriceService));
builder.Services.AddScoped(typeof(ICryptoCoinPriceService), typeof(CryptoCoinPriceService));
builder.Services.AddScoped(typeof(IUserInformationService<>), typeof(UserInformationService<>));
builder.Services.AddScoped(typeof(IAccount<>), typeof(AccountService<>));

builder.Services.AddScoped(typeof(NotFoundFilter<>));
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"), options =>
    {
        options.MigrationsAssembly(Assembly.GetAssembly(typeof(ApplicationDbContext)).GetName().Name);
    });
});



builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddMemoryCache();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();



app.UseAuthorization();

app.UseCustomException();

app.MapControllers();

app.Run();