using Blazor.FileReader;
using Blazored.Modal;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OpenBullet2.Repositories;
using OpenBullet2.Services;
using RuriLib.Services;

namespace OpenBullet2
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddRazorPages();
            services.AddServerSideBlazor().AddCircuitOptions(options => { options.DetailedErrors = true; });
            services.AddBlazoredModal();
            services.AddFileReaderService();
            services.AddScoped<IConfigRepository, DiskConfigRepository>();
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlite(Configuration.GetConnectionString("DefaultConnection")));

            // Repositories
            services.AddScoped<IProxyRepository, DbProxyRepository>();
            services.AddScoped<IProxyGroupRepository, DbProxyGroupRepository>();
            services.AddScoped<IWordlistRepository, HybridWordlistRepository>();
            services.AddScoped<IHitRepository, DbHitRepository>();
            services.AddScoped<IJobRepository, DbJobRepository>();

            // Singletons
            services.AddSingleton<MetricsService>();
            services.AddSingleton<RuriLibSettingsService>();
            services.AddSingleton<PersistentSettingsService>();
            services.AddSingleton<VolatileSettingsService>();
            services.AddSingleton<ConfigService>();
            services.AddSingleton<JobManagerService>();
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

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller}/{action}");

                endpoints.MapControllers();
                endpoints.MapBlazorHub();
                endpoints.MapFallbackToPage("/_Host");
            });

            // Load the configs
            var configService = app.ApplicationServices.GetService<ConfigService>();
            var configRepo = app.ApplicationServices.GetService<IConfigRepository>();
            configService.Configs = configRepo.GetAll().Result;
        }
    }
}
