using IdentityServer4.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MiniBook.Identity.Configuration;
using MiniBook.Identity.Data;
using MiniBook.Identity.Models;
using System.Globalization;

namespace MiniBook.Identity
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
            var connectionString = Configuration.GetConnectionString("DefaultConnection");

            services.AddDbContext<MinibookContext>(options => {
                options.UseSqlServer(connectionString);
            });
            services.AddIdentity<User, IdentityRole>()
                .AddEntityFrameworkStores<MinibookContext>()
                .AddDefaultTokenProviders();

            services.Configure<IdentityOptions>(options =>
            {
                options.Password.RequiredLength = 6;
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
            });


            services.AddControllers().AddNewtonsoftJson(
               options =>
               {
                   options.SerializerSettings.NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore;
                   options.SerializerSettings.Formatting = Newtonsoft.Json.Formatting.Indented;
               });

            services.AddIdentityServer()
              //  .AddInMemoryPersistedGrants()
                .AddInMemoryIdentityResources(Config.GetIdentityResources())
                .AddInMemoryApiResources(Config.GetApiResources())
                .AddInMemoryClients(Config.GetClients())
                .AddInMemoryApiScopes(Config.GetApiScope())
                .AddAspNetIdentity<User>()              
                .AddDeveloperSigningCredential();

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseRequestLocalization(new RequestLocalizationOptions { 
                DefaultRequestCulture=new RequestCulture("en"),
                SupportedCultures=new [] {new CultureInfo("en"),new CultureInfo("vi")},
                SupportedUICultures=new[] { new CultureInfo("en"), new CultureInfo("vi") },
                RequestCultureProviders=new IRequestCultureProvider[]
                {
                    new QueryStringRequestCultureProvider(),
                    new AcceptLanguageHeaderRequestCultureProvider()
                }
            });
            app.UseIdentityServer();

            app.UseRouting();


            //app.UseAuthorization();


            app.UseEndpoints(builder => builder.MapDefaultControllerRoute());

        }
    }
}
