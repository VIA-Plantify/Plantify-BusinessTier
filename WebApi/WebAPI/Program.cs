using System.Text;
using GrpcRepositories;
using GrpcRepositories.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using RepositoryContracts;
using Serilog;
using ServiceContracts;
using Services;

var builder = WebApplication.CreateBuilder(args);

Directory.CreateDirectory("Logs");

builder.Services.AddControllers();
builder.Services.AddAuthorization();

// Configure CORS
var allowedOrigins = builder.Configuration
    .GetSection("AllowedOrigins")
    .Get<string[]>() ?? [];

builder.Services.AddCors(options =>
{
    options.AddPolicy("Frontend", policy =>
    {
        policy
            .WithOrigins(allowedOrigins)
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials();
    });
});

// Configure Serilog
builder.Host.UseSerilog((ctx, services, lc) => lc
    .MinimumLevel.Information()
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.File(
        path: "Logs/myapp-.txt",
        outputTemplate:
        "{Timestamp:yyyy-MM-dd HH:mm:ss} [{Level:u3}] {Message:lj}{NewLine}{Exception}",
        rollingInterval: RollingInterval.Day,
        retainedFileCountLimit: 14)
);

// Configure gRPC clients
var grpcAddress = builder.Configuration["GrpcServer:Address"]
                  ?? throw new InvalidOperationException("GrpcServer:Address is missing.");

builder.Services.AddGrpcClient<AuthServiceProto.AuthServiceProtoClient>(options =>
{
    options.Address = new Uri(grpcAddress);
});

builder.Services.AddGrpcClient<UserServiceProto.UserServiceProtoClient>(options =>
{
    options.Address = new Uri(grpcAddress);
});
builder.Services.AddGrpcClient<PlantServiceProto.PlantServiceProtoClient>(options =>
{
    options.Address = new Uri(grpcAddress);
});


// Configure JWT
var jwtKey = builder.Configuration["Jwt:Key"]
             ?? throw new InvalidOperationException("Jwt:Key is missing.");

var jwtIssuer = builder.Configuration["Jwt:Issuer"]
                ?? throw new InvalidOperationException("Jwt:Issuer is missing.");

var jwtAudience = builder.Configuration["Jwt:Audience"]
                  ?? throw new InvalidOperationException("Jwt:Audience is missing.");

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.MapInboundClaims = false;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidAudience = jwtAudience,
            ValidIssuer = jwtIssuer,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
            ClockSkew = TimeSpan.Zero
        };
    });

// Repositories
builder.Services.AddScoped<IAuthRepository, AuthRepositoryGrpc>();
builder.Services.AddScoped<IUserRepository, UserRepositoryGrpc>();
builder.Services.AddScoped<IPlantRepository, PlantRepositoryGrpc>();




// Services
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IPlantService, PlantService>();


var app = builder.Build();

app.UseHttpsRedirection();

app.UseRouting();
app.UseCors("Frontend");
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();