using System.Text;
using BackEnd.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace BackEnd.Services
{
    public static class JwtStartup
    {
        public static void ConfigureJwt(this WebApplicationBuilder builder)
        {
            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.SaveToken = true;
                options.RequireHttpsMetadata = false;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = builder.Configuration["Authentication:Issuer"],
                    ValidAudience = builder.Configuration["Authentication:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.ASCII.GetBytes(GetAuthKeyFromService(builder.Services)))
                };
            });
        }

        /// <summary>
        /// Helper method to get the auth key from the KeyVault service
        /// </summary>
        /// <param name="services">Service collection</param>
        /// <returns>Auth key string</returns>
        private static string GetAuthKeyFromService(IServiceCollection services)
        {
            var serviceProvider = services.BuildServiceProvider();
            var keyVaultService = serviceProvider.GetRequiredService<IKeyVaultService>();
            return keyVaultService.AuthKey;
        }

            //builder.Services.AddAuthentication("Bearer")
            //    .AddJwtBearer(options =>
            //        { 
            //            options.SaveToken = true;
            //            options.RequireHttpsMetadata = false;
            //            options.TokenValidationParameters = new()
            //            {
            //                ValidateIssuer = true,
            //                ValidateAudience = true,
            //                ValidateIssuerSigningKey = true,
            //                ValidIssuer = builder.Configuration["Authentication:Issuer"],
            //                ValidAudience = builder.Configuration["Authentication:Audience"],
            //                IssuerSigningKey = new SymmetricSecurityKey(
            //                    Encoding.ASCII.GetBytes(secret.Value))
            //            };
            //        }
            //    );
        
    }
}