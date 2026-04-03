using System.Text;
using Grpc.Net.Client;
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

// Configure GrpcClients
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

// Configure Token
builder.Services.AddAuthentication().AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
{
    options.MapInboundClaims = false;
    options.TokenValidationParameters = new TokenValidationParameters()
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidAudience = builder.Configuration["Jwt:Audience"],
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"] ?? "")),
        ClockSkew = TimeSpan.Zero,
    }; 
});

// Configure Services

// Grpc Repositories
builder.Services.AddScoped<IAuthRepository, AuthRepositoryGrpc>();
builder.Services.AddScoped<IUserRepository, UserRepositoryGrpc>();

// Services
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IUserService, UserService>();


var app = builder.Build();
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
}

app.UseRouting();
app.UseCors(x => x
    .AllowAnyMethod()
    .AllowAnyHeader()
    .SetIsOriginAllowed(origin => true) 
    .AllowCredentials());
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.Run();