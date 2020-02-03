using BackEnd.DataBase;
using IdentityServer.Services;
using IdentityServer.Services.Configure;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Models.People;
using Models.People.Roles;
using System;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using WebApp.Configure.Models;

namespace IdentityServer
{
    public class Startup
    {
        public IConfiguration Configuration { get; }
        public IHostingEnvironment Environment { get; }

        public Startup(IConfiguration configuration, IHostingEnvironment environment)
        {
            Configuration = configuration;
            Environment = environment;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            
            if (Configuration.GetValue<bool>("IN_MEMORY"))
            {
                services.AddDbContext<DataBaseContext>(options =>
                    options.UseInMemoryDatabase("memory"));
            }
            else
            {
                services.AddDbContext<DataBaseContext>(options =>
                    options.UseNpgsql(Configuration.GetConnectionString("DefaultConnection"),
                        builder => builder.MigrationsAssembly(nameof(BackEnd.DataBase))));
            }

            services.AddIdentity<User, Role>()
                .AddEntityFrameworkStores<DataBaseContext>()
                .AddDefaultTokenProviders();

            services.AddMvc(options => options.EnableEndpointRouting = false)
                .SetCompatibilityVersion(Microsoft.AspNetCore.Mvc.CompatibilityVersion.Version_3_0);
            var migrationsAssembly = typeof(Startup).GetTypeInfo().Assembly.GetName().Name;


            var builder = services.AddIdentityServer(options =>
            {
                options.PublicOrigin = Configuration.GetValue<string>("PublicOrigin");
                options.Events.RaiseErrorEvents = true;
                options.Events.RaiseInformationEvents = true;
                options.Events.RaiseFailureEvents = true;
                options.Events.RaiseSuccessEvents = true;
            })
                .AddConfigurationStore(options =>
                {
                    options.ConfigureDbContext = b =>
                        b.UseNpgsql(Configuration.GetConnectionString("ConfigurationDatabase"), sql => sql.MigrationsAssembly(migrationsAssembly));
                })
                .AddAspNetIdentity<User>()
                .AddProfileService<IdentityProfileService>();

            if (Configuration.GetValue<bool>("DEBUG_CREDENTIAL"))
            {
                builder.AddDeveloperSigningCredential();
            }
            else
            {
                using var certificate = new X509Certificate2(
                    Configuration.GetValue<string>("ISKeyPath"),
                    Configuration.GetValue<string>("ISKeyPassword"));
                builder.AddSigningCredential(certificate);
            }

            var corsOrigins = Configuration.GetSection("CORS:Origins").AsEnumerable().Skip(1).Select(kvp => kvp.Value).ToArray();
            services.AddCors(options =>
            {
                options.AddPolicy("default", policy =>
                {
                    policy.WithOrigins(corsOrigins)
                        .AllowAnyHeader()
                        .AllowAnyMethod();
                });
            });

            services.AddWebAppConfigure()
                .AddTransientConfigure<DefaultUserConfigureWork>(Environment.IsDevelopment() && Configuration.GetValue<bool>("DEFAULT_USER"))
                .AddTransientConfigure<FillConfigurationConfigureWork>()
                .AddTransientConfigure<MigrateConfigureDatabaseWork>();

            services.Configure<ForwardedHeadersOptions>(options =>
            {
                options.ForwardedHeaders =
                    ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
            });


            //services.AddAuthentication()
            //    .AddGoogle(options =>
            //    {
            //        // register your IdentityServer with Google at https://console.developers.google.com
            //        // enable the Google+ API
            //        // set the redirect URI to http://localhost:5000/signin-google
            //        options.ClientId = "copy client ID from Google here";
            //        options.ClientSecret = "copy client secret from Google here";
            //    });
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseForwardedHeaders();

            if (Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseCors("default");
            app.UseStaticFiles();
            app.UseIdentityServer();
            app.UseMvcWithDefaultRoute();
        }
    }
}