using CineLog.BLL;
using CineLog.BLL.Services;
using CineLog.Data.Models;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Controllers
builder.Services.AddControllers();

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// DB Context
builder.Services.AddDbContext<CineLogContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// IMPORTANT: register factory version (NOT EfCineLogService directly)
builder.Services.AddScoped<ICineLogService>(provider =>
{
    var context = provider.GetRequiredService<CineLogContext>();
    return CineLogServiceFactory.Create(context);
});

var app = builder.Build();

// Swagger
app.UseSwagger();
app.UseSwaggerUI();

app.MapControllers();

app.Run();
