using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using OnlineShop.DB;
using Serilog;
using System.Globalization;
using OnlineShop.DB.Storages;
using OnlineShop.DB.Interfaces;
using Microsoft.AspNetCore.Identity;
using OnlineShop.DB.Models;

namespace WomanShop
{
    public class Program
    {

        public static void Main(string[] args)
        {

            var builder = WebApplication.CreateBuilder(args);
            builder.Host.UseSerilog((context, configuration) => configuration.ReadFrom.Configuration(context.Configuration));

            builder.Services.AddControllersWithViews();
            string connection = builder.Configuration.GetConnectionString("online_shop");
            builder.Services.AddDbContext<DatabaseContext>(options => options.UseSqlServer(connection));
            builder.Services.AddDbContext<IdentityContext>(options => options.UseSqlServer(connection));
            builder.Services.AddIdentity<User, IdentityRole>().AddEntityFrameworkStores<IdentityContext>().AddDefaultTokenProviders();
            builder.Services.Configure<DataProtectionTokenProviderOptions>
                (opt =>opt.TokenLifespan = TimeSpan.FromHours(2));
            builder.Services.ConfigureApplicationCookie(option =>
            {
                option.ExpireTimeSpan = TimeSpan.FromDays(1);
                option.LoginPath = "/Auth/Login";
                option.LogoutPath = "/Auth/Logout";
                option.Cookie = new CookieBuilder
                {
                    IsEssential = true
                };
            });

            builder.Services.AddTransient<IProductsStorage, DbProductsStorage>();
            builder.Services.AddTransient<IFavoritesStorage, DbFavoritesStorage>();
            builder.Services.AddTransient<ICartsStorage, DbCartsStorage>();
            builder.Services.AddTransient<IOrdersStorage, DbOrdersStorage>();
            builder.Services.Configure<RequestLocalizationOptions>(options =>
            {
                var supportedCultures = new[] { new CultureInfo("en-US") };
                options.DefaultRequestCulture = new Microsoft.AspNetCore.Localization.RequestCulture("en-US");
                options.SupportedCultures = supportedCultures;
                options.SupportedUICultures = supportedCultures;
            }
            );
            var app = builder.Build();

           
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }
            app.UseSerilogRequestLogging();

            app.UseStaticFiles();

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            var localizationOptions = app.Services.GetService<IOptions<RequestLocalizationOptions>>().Value;
            app.UseRequestLocalization(localizationOptions);
            app.MapControllerRoute(
                name: "MyArea",
                pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                var userManager = services.GetRequiredService<UserManager<User>>();
                var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
                IdentityInitializer.Initialize(userManager, roleManager);
            }

            app.Run();
        }
    }
}