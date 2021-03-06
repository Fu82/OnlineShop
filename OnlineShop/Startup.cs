using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OnlineShop.Models;
using System;

namespace OnlineShop
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
            services.AddHttpContextAccessor();

            services.AddControllersWithViews();
            services.AddControllers();
            services.AddDbContext<OnlineShopContext>(options =>
                                                    options.UseSqlServer(Configuration.GetConnectionString("OnlineShopDatabase")));
            services.AddAuthentication();

            //session設定
            services.AddSession(o =>
            {
                o.IdleTimeout = TimeSpan.FromSeconds(1800);
            });

            services.AddMvc().ConfigureApiBehaviorOptions(options =>
            {
                options.SuppressModelStateInvalidFilter = true; //tuer後 不管有沒有報錯 都先會進 controller
            });

            services.AddRazorPages();

            services.AddDistributedMemoryCache();
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
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();


            //Cookie驗證
            app.UseCookiePolicy();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseSession();//啟用session

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
            });
        }
    }
}
