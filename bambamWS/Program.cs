using System;
using System.Linq;
using bambamWS.Models;
using Microsoft.EntityFrameworkCore;

string myPolicies = "FILTRO";

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options => options.AddPolicy(myPolicies, policy => {
    policy.WithOrigins("*").AllowAnyMethod().AllowAnyHeader();
}));

// Add services to the container.

builder.Services.AddDbContext<UsuarioContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("BAMBAMCnx")));

builder.Services.AddControllers();
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

app.UseCors(myPolicies);

app.UseAuthorization();

app.MapControllers();
app.Run();

