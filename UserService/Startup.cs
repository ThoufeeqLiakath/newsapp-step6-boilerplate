using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.DataProtection.AuthenticatedEncryption;
using Microsoft.AspNetCore.DataProtection.AuthenticatedEncryption.ConfigurationModel;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Text;
using UserService.Models;
using UserService.Repository;
using UserService.Services;
namespace UserService
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            //register all dependencies here
            services.AddScoped<UserContext>();
            services.AddScoped<UserService.Services.IUserService, UserService.Services.UserService>();
            services.AddScoped<UserService.Repository.IUserRepository, UserService.Repository.UserRepository>();
            services.AddCors(options =>
            {
                options.AddPolicy("AllowAll",
                builder =>
                {
                    builder.
                    AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader();

                });
            });
            services.AddDataProtection()
                        .UseCryptographicAlgorithms(
                            new AuthenticatedEncryptorConfiguration()
                            {
                                EncryptionAlgorithm = EncryptionAlgorithm.AES_256_CBC,
                                ValidationAlgorithm = ValidationAlgorithm.HMACSHA256
                            });
            //Implement token validation logic
            ValidateToken(services);
            services.AddControllers();
        }
        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseCors("AllowAll");
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }

        private void ValidateToken(IServiceCollection services)
        {
            var key = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(Configuration.GetSection("Audience")["Secret"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var token = new TokenValidationParameters()
            {
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero,

                ValidateAudience = true,
                ValidAudience = Configuration.GetSection("Audience")["Audience"],

                ValidateIssuer = true,
                ValidIssuer = Configuration.GetSection("Audience")["Iss"],

                ValidateIssuerSigningKey = true,
                IssuerSigningKey = key
            };
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(bearer =>
            {
                bearer.TokenValidationParameters = token;
                bearer.RequireHttpsMetadata = false;
                bearer.SaveToken = true;
            });

        }
    }
}
