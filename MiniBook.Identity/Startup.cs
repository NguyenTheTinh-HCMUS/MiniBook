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
using MiniBook.Identity.Validations;
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
            //    var connectionString = Configuration.GetConnectionString("DefaultConnection");


            services.AddControllers().AddNewtonsoftJson(
               options =>
               {
                   options.SerializerSettings.NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore;
                   options.SerializerSettings.Formatting = Newtonsoft.Json.Formatting.Indented;
               });
            services.AddDbContext<MinibookContext>(options =>
            {
                // options.UseSqlServer(connectionString);
                options.UseInMemoryDatabase("memory");
            });

            services.AddIdentity<IdentityUser, IdentityRole>(
                config =>
                {
                    config.Password.RequiredLength = 4;
                    config.Password.RequireDigit = false;
                    config.Password.RequireLowercase = false;
                    config.Password.RequireNonAlphanumeric = false;
                    config.Password.RequireUppercase = false;
                    config.SignIn.RequireConfirmedPhoneNumber = true;
                    
                 
                }
                )
                .AddEntityFrameworkStores<MinibookContext>()
                .AddDefaultTokenProviders();

            //services.Configure<IdentityOptions>(options =>
            //{
            //    options.Password.RequiredLength = 6;
            //    options.Password.RequireDigit = false;
            //    options.Password.RequireLowercase = false;
            //    options.Password.RequireNonAlphanumeric = false;
            //    options.Password.RequireUppercase = false;
            //});



            services.AddIdentityServer()
               .AddInMemoryPersistedGrants()
                .AddInMemoryIdentityResources(Config.GetIdentityResources())
                .AddInMemoryApiResources(Config.GetApiResources())
                .AddInMemoryClients(Config.GetClients())
                .AddInMemoryApiScopes(Config.GetApiScope())
                .AddAspNetIdentity<IdentityUser>()
               .AddExtensionGrantValidator<PhoneNumberTokenSmsGrantValidator>()
                .AddExtensionGrantValidator<PhoneNumberPasswordGrantValidator>()
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
            app.UseRouting();

            app.UseIdentityServer();



            //app.UseAuthorization();


            app.UseEndpoints(builder => builder.MapDefaultControllerRoute());

        }
    }
}
