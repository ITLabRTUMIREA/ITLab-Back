using System;
using System.Collections.Generic;
using System.Text;
using AutoMapper;
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
using BackEnd.Services.ConfigureServices;
using BackEnd.Services.Email;
using BackEnd.Services.Notify;
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
using RTUITLab.AspNetCore.Configure.Configure;
using RTUITLab.AspNetCore.Configure.Invokations;
using Microsoft.OpenApi.Models;
using RTUITLab.EmailService.Client;
using LocalInterfaces = BackEnd.Services.Interfaces;

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
            services.Configure<EmailTemplateSettings>(Configuration.GetSection(nameof(EmailTemplateSettings)));
            services.Configure<BuildInformation>(Configuration.GetSection(nameof(BuildInformation)));

            services.AddMvc(options =>
            {
                options.Filters.Add<ValidateModelAttribute>();
                var policy = new AuthorizationPolicyBuilder()
                     .RequireAuthenticatedUser()
                     .AddAuthenticationSchemes("Bearer")
                     .RequireClaim("scope", "itlab.events")
                     .Build();
                options.Filters.Add(new AuthorizeFilter(policy));
                options.Filters.Add(new ProducesAttribute("application/json"));
            }).AddNewtonsoftJson(options =>
                {
                    options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                    options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                    options.SerializerSettings.DateTimeZoneHandling = DateTimeZoneHandling.Utc;
                    options.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
                }
            );

            services.AddAutoMapper(config => config.AddBackendProfiles(), Assembly.GetExecutingAssembly());

            services.AddAuthentication("Bearer")
                .AddJwtBearer("Bearer", options =>
                {
                    options.Authority = Configuration.GetValue<string>("Authority");
                    options.RequireHttpsMetadata = false;
                    options.Audience = "itlab";
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
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "IT Lab develop API", Version = "v1" });

                var securotyScheme = new OpenApiSecurityScheme
                {
                    In = ParameterLocation.Header,
                    Description = "Please enter into field the word 'Bearer' following by space and JWT",
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey
                };

                c.AddSecurityDefinition("Bearer", securotyScheme);


                var requirement = new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        Array.Empty<string>()
                    }
                };
                c.AddSecurityRequirement(requirement);

                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);
            });

            services.AddCors();


            if (Configuration.GetValue<bool>("UseDebugEmailSender"))
            {
                services.AddTransient<LocalInterfaces.IEmailSender, DebugEmailService>();
            }
            else
            {
                services.AddEmailSender(Configuration
                    .GetSection(nameof(EmailSenderOptions))
                    .Get<EmailSenderOptions>());
                services.AddTransient<LocalInterfaces.IEmailSender, EmailService>();
            }


            services.AddTransient<IUserRegisterTokens, DbUserRegisterTokens>();
            services.AddTransient<IEventsManager, EventsManager>();


            services.AddTransient<IUserPropertiesManager, UserPropertiesManager>();


            services.AddWebAppConfigure()
                .AddTransientConfigure<ApplyMigration>(0)
                .AddTransientConfigure<DBInitService>(Configuration.GetValue<bool>("DB_INIT"), 1)
                ;

            ConfigureNotify(services);
            services.Configure<DocsGeneratorSettings>(Configuration.GetSection(nameof(DocsGeneratorSettings)));


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
                    services.Configure<HttpNotifierSettings>(Configuration.GetSection(nameof(HttpNotifierSettings)));
                    services.AddSingleton<HttpNotifierHostSaver>();
                    services.AddHttpClient(HttpNotifySender.HttpClientName, (provider, client) =>
                    {
                        var configs = provider.GetService<IOptions<HttpNotifierSettings>>();
                        var host = configs.Value.Host;
                        if (configs.Value.NeedChangeUrl)
                        {
                            host = provider.GetService<HttpNotifierHostSaver>().Host;
                        }
                        client.BaseAddress = new Uri(host);
                    });
                    services.AddTransient<INotifySender, HttpNotifySender>();
                    break;
                case "redis":
                    services.Configure<RedisNotifierSettings>(Configuration.GetSection(nameof(RedisNotifierSettings)));
                    services.AddTransient<INotifySender, RedisNotifySender>();
                    break;
                case "console":
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
                    .AllowAnyOrigin());
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

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints => endpoints.MapControllers());
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
            var migrationAssymply = typeof(DataBaseContext).GetTypeInfo().Assembly.GetName().Name;
            switch (dbType)
            {
                case DbType.IN_MEMORY:
                    return options => options.UseInMemoryDatabase(nameof(DbType.IN_MEMORY));
                case DbType.POSTGRES:
                    return options => options.UseNpgsql(Configuration.GetConnectionString(nameof(DbType.POSTGRES)),
                        builder => builder.MigrationsAssembly(migrationAssymply));
                default:
                    throw new ArgumentOutOfRangeException(nameof(dbType));
            }
        }

    }
}
