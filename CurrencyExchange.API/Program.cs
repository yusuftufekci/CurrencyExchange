﻿using CurrencyExchange.API.Filters;
using CurrencyExchange.API.Middlewares;
using CurrencyExchange.Repository;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using CurrencyExchange.API.Modules;
using CurrencyExchange.Core.ConfigModels;
using CurrencyExchange.Service.Validations;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers(options => options.Filters.Add(new ValidateFilterAttribute()))
    .AddFluentValidation(x => x.RegisterValidatorsFromAssemblyContaining<BuyCoinRequestValidator>());

builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.SuppressModelStateInvalidFilter = false;
});


builder.Services.Configure<UrlList>(builder.Configuration.GetSection("UrlList"));
builder.Services.Configure<AppSettings>(builder.Configuration.GetSection("AppSettings"));
builder.Services.Configure<RabbitMqSettings>(builder.Configuration.GetSection("RabbitMqSettings"));
builder.Services.Configure<ControlCryptoCoinAmountSettings>(builder.Configuration.GetSection("ControlCryptoCoinAmountSettings"));


builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"), options =>
    {
        options.MigrationsAssembly(Assembly.GetAssembly(typeof(ApplicationDbContext)).GetName().Name);
    });
});


// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen();

builder.Services.AddMemoryCache();

builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());
builder.Host.ConfigureContainer<ContainerBuilder>(ContainerBuilder =>
    ContainerBuilder.RegisterModule(new ServiceModule()));
builder.Host.ConfigureContainer<ContainerBuilder>(ContainerBuilder =>
    ContainerBuilder.RegisterModule(new CacheModule()));
builder.Host.ConfigureContainer<ContainerBuilder>(ContainerBuilder =>
    ContainerBuilder.RegisterModule(new LogModule()));
builder.Host.ConfigureContainer<ContainerBuilder>(ContainerBuilder =>
    ContainerBuilder.RegisterModule(new RepositoryModule()));
builder.Host.ConfigureContainer<ContainerBuilder>(ContainerBuilder =>
    ContainerBuilder.RegisterModule(new ConfigModule()));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCustomException();

app.MapControllers();

app.Run();
