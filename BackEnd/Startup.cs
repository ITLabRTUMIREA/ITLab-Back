using System;
using System.Collections.Generic;
using System.Text;
using AutoMapper;
using BackEnd.Authorize;
using BackEnd.DataBase;
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
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using Newtonsoft.Json.Serialization;
using BackEnd.Models;
using BackEnd.Models.Settings;
using Models.People.Roles;
using WebApp.Configure.Models;
using BackEnd.Services.ConfigureServices;
using BackEnd.Services.Email;
using BackEnd.Services.Notify;
using WebApp.Configure.Models.Invokations;
using BackEnd.Services.UserProperties;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.Swagger;
using App.Metrics;
using App.Metrics.Formatters.Prometheus;
using System.Linq;
using BackEnd.Exceptions;
using System.Reflection;
using System.IO;
using Microsoft.AspNetCore.Mvc;
using BackEnd.Formatting.MapperProfiles;
using BackEnd.Services.Notify.Debug;

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
            SetupDB(services);
            services.Configure<JsonSerializerSettings>(Configuration.GetSection(nameof(JsonSerializerSettings)));
            services.Configure<DBInitializeSettings>(Configuration.GetSection(nameof(DBInitializeSettings)));
            services.Configure<List<RegisterTokenPair>>(Configuration.GetSection(nameof(RegisterTokenPair)));
            services.Configure<EmailSenderSettings>(Configuration.GetSection(nameof(EmailSenderSettings)));
            services.Configure<BuildInformation>(Configuration.GetSection(nameof(BuildInformation)));
            services.Configure<NotifierSettings>(Configuration.GetSection(nameof(NotifierSettings)));
            services.Configure<JwtIssuerOptions>(Configuration.GetSection(nameof(JwtIssuerOptions)));

            services.AddMvc(options =>
            {
                options.Filters.Add<ValidateModelAttribute>();
                var policy = new AuthorizationPolicyBuilder()
                     .RequireAuthenticatedUser()
                     .AddAuthenticationSchemes("Bearer")
                     .Build();
                options.Filters.Add(new AuthorizeFilter(policy));
                options.Filters.Add(new ProducesAttribute("application/json"));
            }).AddJsonOptions(options =>
                {
                    options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                    options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                    options.SerializerSettings.DateTimeZoneHandling = DateTimeZoneHandling.Utc;
                    options.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
                }
            );

            services.AddAutoMapper(config =>config.AddBackendProfiles(), Assembly.GetExecutingAssembly());

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

            services.AddAuthentication("Bearer")
            .AddJwtBearer("Bearer", options =>
            {
                options.Authority = Configuration.GetValue<string>("Authority");
                options.RequireHttpsMetadata = false;
                options.Audience = "api1";
            });


            services.AddIdentity<User, Role>(identityOptions =>
            {
                identityOptions.Password.RequireDigit = false;
                identityOptions.Password.RequireLowercase = false;
                identityOptions.Password.RequireUppercase = false;
                identityOptions.Password.RequireNonAlphanumeric = false;
                identityOptions.Password.RequiredLength = 6;
                identityOptions.User.RequireUniqueEmail = true;
            })
             .AddEntityFrameworkStores<DataBaseContext>()
             .AddDefaultTokenProviders();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info { Title = "IT Lab develop API", Version = "v1" });
                c.AddSecurityDefinition("Bearer",
                    new ApiKeyScheme
                    {
                        In = "header",
                        Description = "Please enter into field the word 'Bearer' following by space and JWT",
                        Name = "Authorization",
                        Type = "apiKey"
                    });
                c.AddSecurityRequirement(new Dictionary<string, IEnumerable<string>>
                {
                    { "Bearer", Enumerable.Empty<string>() }
                });
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);
            });

            services.AddCors();


            if (Configuration.GetValue<bool>("UseDebugEmailSender"))
                services.AddTransient<IEmailSender, DebugEmailService>();
            else
                services.AddTransient<IEmailSender, EmailService>();


            services.AddTransient<IUserRegisterTokens, DbUserRegisterTokens>();
            services.AddTransient<IEventsManager, EventsManager>();
            services.AddSingleton<ISmsSender, SmsService>();


            services.AddTransient<IUserPropertiesManager, UserPropertiesManager>();


            services.AddWebAppConfigure()
                .AddTransientConfigure<EquipmentUpgradeMigrate>(Configuration.GetValue<bool>(EquipmentUpgradeMigrate.ConditionKey))
                .AddTransientConfigure<DBInitService>(Configuration.GetValue<bool>("DB_INIT"))
                .AddTransientConfigure<ApplyMigration>(Configuration.GetValue<bool>("MIGRATE"))
                ;

            ConfigureNotify(services);


            var metrics = AppMetrics.CreateDefaultBuilder()
                .OutputMetrics.AsPrometheusPlainText()
                .Build();

            services.AddMetrics(metrics);
            services.AddMetricsTrackingMiddleware();
            services.AddMetricsEndpoints(options =>
                options.MetricsTextEndpointOutputFormatter = metrics.OutputMetricsFormatters.OfType<MetricsPrometheusTextOutputFormatter>().First()
            );

            services.AddSpaStaticFiles(spa => spa.RootPath = "wwwroot");
        }

        private void ConfigureNotify(IServiceCollection services)
        {
            services.AddSingleton<INotifyMessagesQueue, CuncurrentBagMessagesQueue>();
            services.AddHostedService<NotifierHostedService>();
            services.AddTransient<INotifier, MessageQueueNotifier>();

            switch (Configuration.GetValue<string>("NotifyType"))
            {
                case "http":
                    services.AddSingleton<HttpNotifierHostSaver>();
                    services.AddHttpClient(HttpNotifySender.HttpClientName, (provider, client) =>
                    {
                        var configs = provider.GetService<IOptions<NotifierSettings>>();
                        var host = configs.Value.Host;
                        if (configs.Value.NeedChangeUrl)
                        {
                            host = provider.GetService<HttpNotifierHostSaver>().Host;
                        }
                        client.BaseAddress = new Uri(host);
                    });
                    services.AddTransient<INotifySender, HttpNotifySender>();
                    break;
                default:
                    services.AddTransient<INotifySender, ConsoleNotifySender>();
                    break;
            }
            if (Configuration.GetValue<bool>("UseRandomEventsGenerator"))
            {
                services.AddHostedService<RandomEventsGenerator>();
            }
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(
            IApplicationBuilder app)
        {
            app.UseCors(config =>
                config.AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowAnyOrigin()
                    .AllowCredentials());
            app.UseMetricsAllEndpoints();
            app.UseMetricsAllMiddleware();
            app.UseWebAppConfigure();
            app.UseSwagger(c => { c.RouteTemplate = "api/{documentName}/swagger.json"; });
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/api/v1/swagger.json", "My API V1");
                c.RoutePrefix = "api";
            });
            app.UseExceptionHandlerMiddleware();
            app.UseAuthentication();
            app.UseMvc();
            app.UseSpaStaticFiles();
            app.UseSpa(spa => { });
        }

        private void SetupDB(IServiceCollection services)
        {
            services.AddEntityFrameworkNpgsql();
            var dbType = Configuration.GetValue<DbType>("DB_TYPE");
            services.AddDbContext<DataBaseContext>(GetOptionsBuilder(dbType));
        }
        private Action<DbContextOptionsBuilder> GetOptionsBuilder(DbType dbType)
        {
            switch (dbType)
            {
                case DbType.SQL_SERVER_REMOTE:
                    return options => options.UseSqlServer(Configuration.GetConnectionString(nameof(DbType.SQL_SERVER_REMOTE)));
                case DbType.SQL_SERVER_LOCAL:
                    return options => options.UseSqlServer(Configuration.GetConnectionString(nameof(DbType.SQL_SERVER_LOCAL)));
                case DbType.IN_MEMORY:
                    return options => options.UseInMemoryDatabase(nameof(DbType.IN_MEMORY));
                case DbType.POSTGRES_LOCAL:
                    return options => options.UseNpgsql(Configuration.GetConnectionString(nameof(DbType.POSTGRES_LOCAL)));
                default:
                    throw new ArgumentOutOfRangeException(nameof(dbType));
            }
        }

    }
}
