using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PlanningPoker.Core.Utilities;
using PlanningPoker.Website.Data;
using PlanningPoker.Website.Hubs;

namespace PlanningPoker.Website
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllersWithViews();
            builder.Services.AddTransient<IGameUtility, GameUtility>();
            builder.Services.AddTransient<IEmailUtility, EmailUtility>();

            builder.Services.AddSignalR();

            builder.Services.AddDbContext<GameContext>();

            var app = builder.Build();

            if (!builder.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            
            app.UseStaticFiles();
            app.UseRouting();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
                endpoints.MapHub<VoteHub>("/votehub");
            });

            app.Run();
        }
    }
}
