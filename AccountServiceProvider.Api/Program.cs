using AccountServiceProvider.Api.Data.Contexts;
using AccountServiceProvider.Api.Services;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AccountDbContext>(x => x.UseSqlServer(builder.Configuration.GetConnectionString("AccountSqlConnection")));
builder.Services.AddScoped<IAccountService, AccountService>();

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(x =>
{
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    x.IncludeXmlComments(xmlPath);
});

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI(x => x.SwaggerEndpoint("/swagger/v1/swagger.json", "Ventixe Account Service Provider API"));

app.UseRewriter(new RewriteOptions().AddRedirect("^$", "swagger"));
app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();
app.Run();
