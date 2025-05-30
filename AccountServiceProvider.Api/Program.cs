using AccountServiceProvider.Api.Data.Contexts;
using AccountServiceProvider.Api.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AccountDbContext>(x => x.UseSqlServer(builder.Configuration.GetConnectionString("AccountSqlConnection")));
builder.Services.AddScoped<IAccountService, AccountService>();

builder.Services.AddControllers();
builder.Services.AddOpenApi();


var app = builder.Build();

app.MapOpenApi();
app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
