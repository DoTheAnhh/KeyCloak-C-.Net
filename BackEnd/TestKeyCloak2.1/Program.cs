using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using TestKeyCloak2._1.Service;
using TestKeyCloak2._1.Service.impl;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddJsonFile("appsettings.json");

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp",
        policy =>
        {
            policy.WithOrigins("http://localhost:3000", "https://localhost:3000/*")
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowCredentials();
        });

});

// Đăng ký HttpClient và các dịch vụ khác
builder.Services.AddHttpClient<IAuthService, AuthService>();
builder.Services.AddHttpClient<IRealmService, RealmService>();

// Cấu hình JWT Authentication
builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.Authority = builder.Configuration["Keycloak:BaseUrl"] + "/realms/" + builder.Configuration["Keycloak:Realm"];
        options.RequireHttpsMetadata = false;
        options.Audience = builder.Configuration["Keycloak:ClientId"];
        options.TokenValidationParameters = new TokenValidationParameters()
        {
            ValidateIssuer = true,
            ValidIssuer = builder.Configuration["Keycloak:BaseUrl"] + "/realms/" + builder.Configuration["Keycloak:Realm"]
        };
    });

builder.Services.AddControllers();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo() { Title = "Keycloak API", Version = "v1" });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Keycloak API v1"));
}

app.UseHttpsRedirection();

app.UseCors("AllowReactApp");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();