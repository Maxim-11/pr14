using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using NLog;
using NLog.Web;
using System.Text;

var logger = NLog.LogManager.Setup().LoadConfigurationFromAppSettings().GetCurrentClassLogger();
logger.Debug("Start");
try
{
    var builder = WebApplication.CreateBuilder(args);
    var key = Convert.ToBase64String(System.Security.Cryptography.RandomNumberGenerator.GetBytes(32));
    Console.WriteLine($"Generated JWT Key: {key}");
    builder.Logging.ClearProviders();
    builder.Logging.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.None);
    builder.Host.UseNLog();
    builder.Services.AddControllers();
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSingleton<JwtTokenService>();
    builder.Services.AddSwaggerGen();
    builder.Services.AddControllers();
    builder.Services.AddDbContext<Class.Models.ClassContext>(option => option.UseSqlServer(builder.Configuration.GetConnectionString("con")));
    builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
                ValidateIssuer = false,
                ValidateAudience = false,
            };
        });
    var app = builder.Build();
    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }
    app.UseHttpsRedirection();
    app.UseAuthentication();
    app.UseAuthorization();
    app.MapControllers();
    app.Run();
}
catch (Exception exception)
{
    logger.Error(exception, "Error");
    throw;
}
finally
{
    NLog.LogManager.Shutdown();
}