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
            services.AddDbContext<DataBaseContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("LocalDatabase")));
            services.Configure<JsonSerializerSettings>(Configuration.GetSection(nameof(JsonSerializerSettings)));
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
            services.AddIdentity<User, IdentityRole<Guid>>(identityOptions =>
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
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IServiceProvider serviceProvider)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMiddlewareClassTemplate();
            app.UseAuthentication();
            app.UseMvc();

            //CreateRoles(serviceProvider).Wait();
        }
        public async Task CreateRoles(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole<Guid>>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<User>>();
            string[] roles = { "Admin", "User", "Zuev))0)" };
            IdentityResult roleResult;

            foreach (var role in roles)
            {
                var roleExist = await roleManager.RoleExistsAsync(role);
                if (!roleExist)
                {
                    var identityRole = new IdentityRole<Guid> { Name = role };
                    roleResult = await roleManager.CreateAsync(identityRole);
                }
            }
            var config = Configuration.GetSection("UserSettings");
            var waitRoles = config.GetSection("Roles")
                .AsEnumerable()
                .Select(KVP => KVP.Value)
                .Where(V => V != null)
                .ToList();
            if (config["UserEmail"] == null || waitRoles.Count == 0)
                return;

            var powerUser = await userManager.FindByEmailAsync(Configuration.GetSection("UserSettings")["UserEmail"]);
            if (powerUser == null)
                return;

            foreach (var role in waitRoles)
            {
                if (!await userManager.IsInRoleAsync(powerUser, role))
                    await userManager.AddToRoleAsync(powerUser, role);
            }
        }
    }
}
