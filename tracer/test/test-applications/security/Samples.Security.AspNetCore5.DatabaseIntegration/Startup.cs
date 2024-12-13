using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
#if NET7_0_OR_GREATER
using Microsoft.EntityFrameworkCore;
#endif
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using SQLitePCL;

namespace Samples.Security.AspNetCore5.DatabaseIntegration
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        private IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddRazorPages();
            services.AddControllersWithViews();

            if (int.TryParse(Environment.GetEnvironmentVariable("IAST_TEST_SESSION_IDLE_TIMEOUT"), out var parsed))
            {
                services.AddSession(
                    options =>
                    {
                        options.IdleTimeout = TimeSpan.FromMinutes(parsed);
                    });
            }
            else
            {
                services.AddSession();
            }
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
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            
            // Directory listing leak vulnerability on request path /Iast/directory_listing_leak
            if (Environment.GetEnvironmentVariable("IAST_TEST_ENABLE_DIRECTORY_LISTING_REQUEST_PATH") == "true")
            {
                app.UseDirectoryBrowser(new DirectoryBrowserOptions
                {
                    RequestPath = "/Iast/directory_listing_leak",
                    FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory()))
                });
            }
            
            // Directory listing leak vulnerability on whole app
            if (Environment.GetEnvironmentVariable("IAST_TEST_ENABLE_DIRECTORY_LISTING_WHOLE_APP") == "true")
            {
                app.UseDirectoryBrowser();
            }
            
            // Directory listing leak vulnerability on string path /Iast/directory_listing_leak
            if (Environment.GetEnvironmentVariable("IAST_TEST_ENABLE_DIRECTORY_LISTING_STRING_PATH") == "true")
            {
                app.UseDirectoryBrowser("/Iast/directory_listing_leak");
            }

            app.UseSession();
            app.UseRouting();

            app.UseAuthorization();
            app.UseAuthentication();
            app.Map(
                "/alive-check",
                builder =>
                {
                    builder.Run(
                        async context =>
                        {
                            await context.Response.WriteAsync("Yes");
                        });
                });

            app.Map(
                "/shutdown",
                builder =>
                {
                    builder.Run(
                        async context =>
                        {
                            await context.Response.WriteAsync("Shutting down");
                            _ = Task.Run(() => builder.ApplicationServices.GetService<IHostApplicationLifetime>().StopApplication());
                        });
                });

            app.Use(
                async (context, next) =>
                {
                    // make sure if we go into this middleware after blocking has happened that it s not a second request issued by the developerhandlingpage middleware! if it s that no worries it s another request, not the attack one., its  a redirect.
                    // await context.Response.WriteAsync("do smth before all");
                    await next.Invoke();
                });


            app.UseEndpoints(
                endpoints =>
                {
                    endpoints.MapControllerRoute(
                        name: "default",
                        pattern: "{controller=Home}/{action=Index}/{id?}");

                    endpoints.MapRazorPages();
                });
        }
    }
}
