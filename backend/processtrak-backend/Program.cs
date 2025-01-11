using System.Text;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using processtrak_backend.Api.data;
using processtrak_backend.Services;
using processtrak_backend.Utils;

// Load environment variables from .env file
EnvReader.Load(".env");

var builder = WebApplication.CreateBuilder(args);

var DbConnectionString =
    $"Host={Environment.GetEnvironmentVariable("DB_HOST")}; Database={Environment.GetEnvironmentVariable("DATABASE_NAME")}; Username={Environment.GetEnvironmentVariable("USERNAME")}; Password={Environment.GetEnvironmentVariable("DB_PASSWORD")}";

// Add services to the container.
builder
    .Services.AddControllers(config =>
    {
        var policy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build();
        config.Filters.Add(new AuthorizeFilter(policy)); // Global authorization
    })
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.Preserve;
    });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition(
        "Bearer",
        new OpenApiSecurityScheme
        {
            Name = "Authorization",
            Type = SecuritySchemeType.Http,
            Scheme = "Bearer",
            BearerFormat = "JWT",
            In = ParameterLocation.Header,
            Description =
                "JWT Authorization header using the Bearer scheme. Example: 'Bearer {token}'",
        }
    );

    options.AddSecurityRequirement(
        new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer",
                    },
                },
                Array.Empty<string>()
            },
        }
    );
});

builder.Services.AddDbContext<AppDbContext>(options => options.UseNpgsql(DbConnectionString));
builder.Services.AddHttpClient();
builder.Services.AddScoped<AuthService>();

builder
    .Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(GlobalVariables.JwtSecret)
            ),
            ValidateIssuer = false,
            ValidateAudience = false,
        };
    });

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

app.UseMiddleware<JwtMiddleware>();
app.UseRouting();
app.UseAuthorization();

app.MapControllers();

app.Run();
