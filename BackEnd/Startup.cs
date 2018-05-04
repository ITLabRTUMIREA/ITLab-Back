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
                    options.UseSqlServer("Server=tcp:labworksqlserver.database.windows.net,1433;Initial Catalog=labworkcontroldb;Persist Security Info=False;User ID=realityshift;Password=RedAlert5;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;"));
#endif
            services.Configure<JsonSerializerSettings>(Configuration.GetSection(nameof(JsonSerializerSettings)));
            services.Configure<DBInitialize>(Configuration.GetSection(nameof(DBInitialize)));
            services.AddMvc(options =>
            {
                options.Filters.Add<ValidateModelAttribute>();
                // TODO WorkAround for property names!
                options.OutputFormatters.Insert(0, new JsonOutputFormatter(
                    services
                    .BuildServiceProvider()
                    .GetService<IOptions<JsonSerializerSettings>>()
                    .Value, ArrayPool<char>.Shared));
            });

            services.AddAutoMapper();

            var jwtAppSettingOptions = Configuration.GetSection(nameof(JwtIssuerOptions)).Get<JwtIssuerOptions>();

            services.AddSingleton<IJwtFactory, JwtFactory>();

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

            services.AddTransient<IEmailSender, EmailService>();
            services.AddTransient<IEventsManager, EventsManager>();
            services.AddTransient<DataBaseFiller>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IServiceProvider serviceProvider)
        {
            if (Configuration.GetValue<bool>("DBInitialize:Need"))
                using (var serviceScope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope())
                    serviceScope.ServiceProvider.GetService<DataBaseFiller>().Fill().Wait();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseMiddlewareClassTemplate();
            app.UseAuthentication();
            app.UseMvc();
        }
    }
}
