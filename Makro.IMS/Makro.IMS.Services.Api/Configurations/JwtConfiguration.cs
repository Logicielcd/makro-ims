using Makro.IMS.Infra.Data.Auth;
using Makro.IMS.Infra.Data.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Makro.IMS.Services.Api.Configurations
{
    public static class JwtConfiguration
    {
        public static void AddJwtConfiguration(this IServiceCollection services, IConfiguration config)
        {
            services.AddSingleton<IJwtFactory, JwtFactory>();

            // Get options from app settings
            var key = config[nameof(JwtIssuerOptions.SigningKey)] ?? "";
            var _signingKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(key));

            // Configure JwtIssuerOptions
            services.Configure<JwtIssuerOptions>(options =>
            {
                options.SigningKey = config[nameof(JwtIssuerOptions.SigningKey)];
                options.Issuer = config[nameof(JwtIssuerOptions.Issuer)];
                options.Audience = config[nameof(JwtIssuerOptions.Audience)];
                options.SigningCredentials = new SigningCredentials(_signingKey, SecurityAlgorithms.HmacSha256);
                options.ValidFor = TimeSpan.FromMinutes(Convert.ToDouble(config[nameof(JwtIssuerOptions.ValidFor)]));
            });

            List<string> audiences = new List<string>
            {
                config[nameof(JwtIssuerOptions.Audience)]
            };

            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = false,
                ValidIssuer = config[nameof(JwtIssuerOptions.Issuer)],
                ValidateAudience = false,
                //ValidAudience = config[nameof(JwtIssuerOptions.Audience)],                          
                ValidAudiences = config.GetSection(nameof(JwtIssuerOptions.Audience)).AsEnumerable().Select(x => x.Value),

                ValidateIssuerSigningKey = true,
                IssuerSigningKey = _signingKey,

                //RequireExpirationTime = false,
                //ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero // TimeSpan.FromMinutes(5)
                //ClockSkew = TimeSpan.FromSeconds(5),
                //RequireExpirationTime = true,                
            };

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(configureOptions =>
            {
                configureOptions.ClaimsIssuer = config[nameof(JwtIssuerOptions.Issuer)];
                configureOptions.TokenValidationParameters = tokenValidationParameters;
                configureOptions.SaveToken = true;                
            });

            // api user claim policy
            services.AddAuthorization(options =>
            {
                options.AddPolicy("ApiUser", policy => policy.RequireClaim(Constants.Strings.JwtClaimIdentifiers.Rol, Constants.Strings.JwtClaims.ApiAccess));
            });
        }
    }
}
