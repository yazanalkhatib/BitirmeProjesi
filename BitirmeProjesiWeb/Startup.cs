using System;
using System.Collections.Generic;
using System.Globalization;
using BitirmeProjesiWeb.Models;
using BitirmeProjesiWeb.Utilities;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace BitirmeProjesiWeb
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
            services.AddLocalization(options => options.ResourcesPath = "Resources");

            services.Configure<RequestLocalizationOptions>(options =>
            {
                var cultures = new List<CultureInfo> {
                    new CultureInfo("en"),
                    new CultureInfo("tr"),
                    new CultureInfo("ar")
                };

                options.DefaultRequestCulture = new Microsoft.AspNetCore.Localization.RequestCulture("en");
                options.SupportedCultures = cultures;
                options.SupportedUICultures = cultures;
            });

            services.AddMvc()
                .AddViewLocalization(Microsoft.AspNetCore.Mvc.Razor.LanguageViewLocationExpanderFormat.Suffix)
                .AddDataAnnotationsLocalization();

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            services.AddSingleton<ISecurity, Security>();
            services.AddSingleton<IEncryptor, Aes>();
            services.AddSingleton<IMailing, Mail>();

            services.AddSession();

            services.AddSession(options => {
                options.IdleTimeout = TimeSpan.FromMinutes(60);
            });

            services.AddDbContext<BitirmeContext>(options =>
                    options.UseSqlServer(Configuration.GetConnectionString("myconn")));

            services.AddControllersWithViews();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            //var cultures = new List<CultureInfo> {
            //    new CultureInfo("en"),
            //    new CultureInfo("tr"),
            //    new CultureInfo("ar")
            //};

            //app.UseRequestLocalization(options => {
            //    options.DefaultRequestCulture = new Microsoft.AspNetCore.Localization.RequestCulture("en");
            //    options.SupportedCultures = cultures;
            //    options.SupportedUICultures = cultures;
            //});

            app.UseSession();

            app.UseRequestLocalization(app.ApplicationServices.GetRequiredService<IOptions<RequestLocalizationOptions>>().Value);
            //app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}