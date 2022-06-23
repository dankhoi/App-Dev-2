using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using App_Dev_2.Areas.Authenticated.Controllers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.EntityFrameworkCore;
using App_Dev_2.Data;
using App_Dev_2.DbInitializer;
using App_Dev_2.Models;
using App_Dev_2.SendMailService;
using App_Dev_2.Utility;
using Microsoft.AspNetCore.Http;
using Microsoft.CodeAnalysis.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace App_Dev_2
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
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(
                    Configuration.GetConnectionString("DefaultConnection")));
            services.AddDatabaseDeveloperPageExceptionFilter();

            services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
                .AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>();
            services.AddControllersWithViews();
            
            services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = $"/Identity/Account/Login";
                options.LogoutPath = $"/Identity/Account/Logout";
                options.AccessDeniedPath = $"/Identity/Account/AccessDenied";
            });
            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(30);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });
            
            services.AddAuthentication()
                .AddGoogle(googleOptions =>
                {
                    IConfigurationSection googleAuthNSection= Configuration.GetSection("Authentication:Google");
                    googleOptions.ClientId = "294440161-ug5ba9hdp6mi5g4i7s9i7k5iok6oe99s.apps.googleusercontent.com";
                    googleOptions.ClientSecret = "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";
                })
                .AddFacebook(facebookOptions => {
                    // Đọc cấu hình
                    IConfigurationSection facebookAuthNSection = Configuration.GetSection("Authentication:Facebook");
                    facebookOptions.AppId = "1852069598516528";
                    facebookOptions.AppSecret = "9ed6f5c918d2be36ae028c60aff917c5";
                    // Thiết lập đường dẫn Facebook chuyển hướng đến
                    // facebookOptions.CallbackPath = "/dang-nhap-tu-facebook";
                    
                });
            
            services.AddScoped<IDbInitializer, DbInitializer.DbInitializer>();
            
            // map ping mailsettings trong apsettings.js với class mailsettings
            services.AddOptions ();                                         // Kích hoạt Options
            var mailsettings = Configuration.GetSection ("MailSettings");  // đọc config
            services.Configure<MailSettings> (mailsettings);
            
            
            services.AddTransient<ISendMailService, SendMailService.SendMailService>();
            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(30);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });
            services.AddHttpContextAccessor();
        }
        
        

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IDbInitializer dbInitializer)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseMigrationsEndPoint();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();
            app.UseSession();
            app.UseAuthentication();
            app.UseAuthorization();
            
            dbInitializer.Initialize();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{area=UnAuthenticated}/{controller=Home}/{action=Index}/{id?}");
                endpoints.MapRazorPages();
            });
        }
    }
}