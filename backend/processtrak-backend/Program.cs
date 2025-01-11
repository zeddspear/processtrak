using Microsoft.EntityFrameworkCore;
using processtrak_backend.Api.data;
using processtrak_backend.Services;

// Load environment variables from .env file
EnvReader.Load(".env");

var builder = WebApplication.CreateBuilder(args);

var DbConnectionString =
    $"Host={Environment.GetEnvironmentVariable("DB_HOST")}; Database={Environment.GetEnvironmentVariable("DATABASE_NAME")}; Username={Environment.GetEnvironmentVariable("USERNAME")}; Password={Environment.GetEnvironmentVariable("DB_PASSWORD")}";

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<AppDbContext>(options => options.UseNpgsql(DbConnectionString));
builder.Services.AddHttpClient();
builder.Services.AddScoped<AuthService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
else
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseDeveloperExceptionPage();
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthorization();

app.MapControllers();

app.Run();
