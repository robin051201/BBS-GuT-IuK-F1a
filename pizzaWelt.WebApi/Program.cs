global using System.ComponentModel;
global using System.ComponentModel.DataAnnotations;
global using System.IdentityModel.Tokens.Jwt;
global using System.Net;
global using System.Reflection;
global using System.Security.Claims;
global using System.Security.Cryptography;
global using System.Text;
global using AutoMapper;
global using Duende.IdentityServer.EntityFramework.Options;
global using Microsoft.AspNetCore.ApiAuthorization.IdentityServer;
global using Microsoft.AspNetCore.Authentication;
global using Microsoft.AspNetCore.Authentication.Cookies;
global using Microsoft.AspNetCore.Authentication.JwtBearer;
global using Microsoft.AspNetCore.Diagnostics;
global using Microsoft.AspNetCore.Identity;
global using Microsoft.AspNetCore.Mvc;
global using Microsoft.AspNetCore.Mvc.Filters;
global using Microsoft.AspNetCore.Mvc.NewtonsoftJson;
global using Microsoft.EntityFrameworkCore;
global using Microsoft.Extensions.Options;
global using Microsoft.IdentityModel.Tokens;
global using Newtonsoft.Json;
global using PizzaWelt.Data;
global using PizzaWelt.DTOs;
global using PizzaWelt.Filter;
global using PizzaWelt.JwtFeatures;
global using PizzaWelt.MappingConfiguration;
global using PizzaWelt.Models;
global using PizzaWelt.Services;
using PizzaWelt.Extension;

var builder = WebApplication.CreateBuilder(args);

builder.Services.ConfigureDbContext(builder.Configuration.GetConnectionString("DefaultConnection"));

builder.Services.ConfigureServices();

builder.Services.ConfigureCors();

builder.Services.ConfigureServices();

builder.Services.ConfigureHttpsRedirection();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.ConfigureIdentity();

builder.Services.ConfigureAuthentication(builder.Configuration);

builder.Services.ConfigureIdentityServer(builder.Configuration);

var app = builder.Build();

app.UseCors();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseMigrationsEndPoint();
    app.ConfigureCustomExceptionMiddleware();
}
else
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
    app.ConfigureCustomExceptionMiddleware();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();

app.UseIdentityServer();

app.UseAuthorization();

app.MapControllers();

app.Run();
