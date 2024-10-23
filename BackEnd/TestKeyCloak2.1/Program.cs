using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using TestKeyCloak2._1.Service;
using TestKeyCloak2._1.Service.impl;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp",
        policy =>
        {
            policy.WithOrigins("http://localhost:5173")
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowCredentials();
        });
});

// Cấu hình JWT Authentication
builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.Authority = "http://localhost:8080/realms/DemoRealm";
        options.RequireHttpsMetadata = false;
        options.Audience = "democlient";
        options.TokenValidationParameters = new TokenValidationParameters()
        {
            ValidateIssuer = true,
            ValidIssuer = "http://localhost:8080/realms/DemoRealm"
        };
    });


builder.Services.AddHttpClient<IAuthService, AuthService>();
builder.Services.AddHttpClient<IUserService, UserService>();
builder.Services.AddHttpClient<IRealmService, RealmService>();


// Thêm dịch vụ Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo() { Title = "Keycloak API", Version = "v1" });
});

// Thêm dịch vụ HttpClient để gọi Keycloak API
builder.Services.AddHttpClient();

builder.Services.AddControllers();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Keycloak API v1"));
}



// Sử dụng Middleware
app.UseHttpsRedirection();
app.UseCors("AllowReactApp");
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();