using System;
using System.Collections.Generic;
using System.Text;
using AutoMapper;
using BackEnd.Authorize;
using BackEnd.DataBase;
using BackEnd.Exceptions;
using BackEnd.Formatting;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Models;
using Newtonsoft.Json;
using BackEnd.Services.Interfaces;
using BackEnd.Services;
using Models.People;
using System.Runtime.InteropServices;
using BackEnd.Hubs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using Newtonsoft.Json.Serialization;
using BackEnd.Models;
using BackEnd.Models.Settings;
using Models.People.Roles;
using WebApp.Configure.Models;
using BackEnd.Services.ConfigureServices;
using BackEnd.Services.Notify;
using WebApp.Configure.Models.Invokations;
using BackEnd.Services.UserProperties;
using Microsoft.Extensions.Options;
using Npgsql.EntityFrameworkCore.PostgreSQL.Query.ExpressionTranslators.Internal;
using Swashbuckle.AspNetCore.Swagger;
using Extensions;

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
                    options.UseInMemoryDatabase("local"));
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
                    options.UseNpgsql(Configuration.GetConnectionString("PostgresDataBase")));
#else
            services
                    .AddDbContext<DataBaseContext>(options =>
                    options.UseSqlServer(Configuration.GetConnectionString("RemoteDB")));
#endif
            services.Configure<JsonSerializerSettings>(Configuration.GetSection(nameof(JsonSerializerSettings)));
            services.Configure<DBInitializeSettings>(Configuration.GetSection(nameof(DBInitializeSettings)));
            services.Configure<List<RegisterTokenPair>>(Configuration.GetSection(nameof(RegisterTokenPair)));
            services.Configure<EmailSenderSettings>(Configuration.GetSection(nameof(EmailSenderSettings)));
            services.Configure<BuildInformation>(Configuration.GetSection(nameof(BuildInformation)));
            services.Configure<NotifierSettings>(Configuration.GetSection(nameof(NotifierSettings)));

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

            var signingKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(
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
                options.DefaultForbidScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
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

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info { Title = "IT Lab develop API", Version = "v1" });
            });

            services.AddCors();
            services.AddSignalR();

            services.AddTransient<IUserRegisterTokens, DbUserRegisterTokens>();
            services.AddTransient<IEmailSender, EmailService>();
            services.AddTransient<IEventsManager, EventsManager>();
            services.AddSingleton<ISmsSender, SmsService>();


            services.AddSingleton<IUserPropertiesConstants, InMemoryUserPropertiesConstants>();
            services.AddTransient<IUserPropertiesManager, UserPropertiesManager>();


            services.AddWebAppConfigure()
                    .AddTransientConfigure<DBInitService>()
                    .AddTransientConfigure<LoadCustomPropertiesService>();


            services.AddHttpClient(Notifier.HttpClientName, (provider, client) =>
            {
                var configs = provider.GetService<IOptions<NotifierSettings>>();
                client.BaseAddress = new Uri(configs.Value.Host);
            });
            if (Configuration.GetValue<bool>("UseConsoleLogger"))
                services.AddTransient<INotifier, DebugLogNotifier>();
            else
                services.AddTransient<INotifier, Notifier>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(
            IApplicationBuilder app,
            IHostingEnvironment env)
        {
            app.UseCors(config =>
                config.AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowAnyOrigin()
                    .AllowCredentials());
            app.UseWebAppConfigure();
            app.UseSwagger(c => {
                c.RouteTemplate = "api/{documentName}/swagger.json";
            });
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/api/v1/swagger.json", "My API V1");
                c.RoutePrefix = "api";
            });
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
