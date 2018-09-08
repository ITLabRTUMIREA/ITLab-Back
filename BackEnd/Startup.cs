using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using BackEnd.Authorize;
using BackEnd.DataBase;
using BackEnd.Exceptions;
using BackEnd.Formatting;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Models;
using Newtonsoft.Json;
using BackEnd.Services.Interfaces;
using BackEnd.Services;
using Microsoft.CodeAnalysis.Options;
using Models.People;
using System.Runtime.InteropServices;
using BackEnd.Extensions;
using BackEnd.Hubs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using Newtonsoft.Json.Serialization;
using BackEnd.Models;
using BackEnd.Models.Settings;

namespace BackEnd
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
#if DEBUG
            if (Configuration.GetValue<bool>("IS_DOCKER"))
            {
                Console.WriteLine("IM IN IS DOCKER YAY");
                services
                     .AddEntityFrameworkNpgsql()
                     .AddDbContext<DataBaseContext>(options =>
                     options.UseNpgsql(Configuration.GetConnectionString("DockerPosgresDataBase")));
            }
            else
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                services
                    .AddDbContext<DataBaseContext>(options =>
                    options.UseSqlServer(Configuration.GetConnectionString("LocalDBDataBase")));
            else
                services
                    .AddEntityFrameworkNpgsql()
                    .AddDbContext<DataBaseContext>(options =>
                    options.UseNpgsql(Configuration.GetConnectionString("PosgresDataBase")));
#else
            services
                    .AddDbContext<DataBaseContext>(options =>
                    options.UseSqlServer(Configuration.GetConnectionString("RemoteDB")));
#endif
            services.Configure<JsonSerializerSettings>(Configuration.GetSection(nameof(JsonSerializerSettings)));
            services.Configure<DBInitialize>(Configuration.GetSection(nameof(DBInitialize)));
            services.Configure<List<RegisterTokenPair>>(Configuration.GetSection(nameof(RegisterTokenPair)));
            services.Configure<EmailSenderSettings>(Configuration.GetSection(nameof(EmailSenderSettings)));
            services.AddMvc(options =>
            {
                options.Filters.Add<ValidateModelAttribute>();
                var policy = new AuthorizationPolicyBuilder()
                     .RequireAuthenticatedUser()
                     .AddAuthenticationSchemes("Bearer")
                     .Build();
                options.Filters.Add(new AuthorizeFilter(policy));
            }).AddJsonOptions(options =>
                {
                    options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                    options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                    options.SerializerSettings.DateTimeZoneHandling = DateTimeZoneHandling.Utc;
                    options.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
                }
            );

            services.AddAutoMapper();

            var jwtAppSettingOptions = Configuration.GetSection(nameof(JwtIssuerOptions)).Get<JwtIssuerOptions>();

            services.AddTransient<IJwtFactory, JwtFactory>();

            SymmetricSecurityKey signingKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(
                jwtAppSettingOptions.SecretKey));

            services.Configure<JwtIssuerOptions>(options =>
            {
                options.Issuer = jwtAppSettingOptions.Issuer;
                options.Audience = jwtAppSettingOptions.Audience;
                options.SigningCredentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);
            });

            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = jwtAppSettingOptions.Issuer,

                ValidateAudience = true,
                ValidAudience = jwtAppSettingOptions.Audience,

                ValidateIssuerSigningKey = true,
                IssuerSigningKey = signingKey,

                RequireExpirationTime = false,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            };

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(configureOptions =>
            {
                configureOptions.ClaimsIssuer = jwtAppSettingOptions.Issuer;
                configureOptions.TokenValidationParameters = tokenValidationParameters;
                configureOptions.SaveToken = true;
            });

            // add identity
            services.AddIdentity<User, Role>(identityOptions =>
            {
                // configure identity options
                identityOptions.Password.RequireDigit = false;
                identityOptions.Password.RequireLowercase = false;
                identityOptions.Password.RequireUppercase = false;
                identityOptions.Password.RequireNonAlphanumeric = false;
                identityOptions.Password.RequiredLength = 6;
            })
             .AddEntityFrameworkStores<DataBaseContext>()
             .AddDefaultTokenProviders();

            services.AddCors();
            services.AddSignalR();

            services.AddTransient<IUserRegisterTokens, DbUserRegisterTokens>();
            services.AddTransient<IEmailSender, EmailService>();
            services.AddTransient<IEventsManager, EventsManager>();
            services.AddTransient<DataBaseFiller>();
            services.AddSingleton<ISmsSender, SmsService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(
            IApplicationBuilder app,
            IHostingEnvironment env,
            ILoggerFactory loggerFactory)
        {
            if (Configuration.GetValue<bool>("IS_DOCKER"))
                try
                {


                    using (var scope = app.ApplicationServices.CreateScope())
                        scope
                            .ServiceProvider
                            .GetService<DataBaseContext>()
                            .Database
                            .Migrate();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }

            if (Configuration.GetValue<bool>("DB_INIT"))
                using (var serviceScope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope())
                    serviceScope.ServiceProvider.GetService<DataBaseFiller>().Fill().Wait();

            app.UseCors(config =>
                config.AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowAnyOrigin()
                    .AllowCredentials());
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseSignalR(routes =>
            {
                routes.MapHub<MirrorHub>("/chatHub");
            });
            app.UseExceptionHandlerMiddleware();
            app.UseAuthentication();
            app.UseMvc();
        }
    }
}
