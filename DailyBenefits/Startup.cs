using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using DailyBenefits.Model;

namespace DailyBenefits
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddConfiguration(out var Config);
            services.AddDistributedMemoryCache();
            services.AddSession();
            services.AddJoyOIUserCenter();
            services.AddMvc();
            services.AddDbContext<DailyBenefitsContext>(x => x.UseMySql(Config["ConnectionString"]));
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseDeveloperExceptionPage();
            app.UseSession();
            app.UseStaticFiles();
            app.UseMvcWithDefaultRoute();
        }
    }
}
