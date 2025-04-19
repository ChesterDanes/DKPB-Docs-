using BLL.DTOModels.ResponseDTO;
using BLL.ServiceInterfaces.Interfaces;
using BLL_MongoDb.Services;
using BLL_MongoDb.Context;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MongoDB.Driver;
using System.Globalization;

namespace Cw2
{
    public class Program
    {
        public static UserResponseDTO LoggedInUser { get; set; }

        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();

            // Add MongoDbContext to the DI container
            builder.Services.AddSingleton<MongoDbContext>(provider =>
            {
                var configuration = builder.Configuration;
                return new MongoDbContext(configuration);
            });

            // Register services from MongoDB (poprawione - u¿ywamy Mongo serwisów!)
            builder.Services.AddScoped<IProductService, ProductServiceMongoDb>();
            builder.Services.AddScoped<IUserService, UserServiceMongoDb>();
            builder.Services.AddScoped<IOrderService, OrderServiceMongoDb>();
            builder.Services.AddScoped<IProductGroupService, ProductGroupServiceMongoDb>();

            // Zakomentowane stare rejestracje EF/SQL
            // builder.Services.AddDbContext<WebstoreContext>();
            // builder.Services.AddScoped<IProductService, ProductServiceDB>();
            // builder.Services.AddScoped<IUserService, UserService>();
            // builder.Services.AddScoped<IOrderService, OrderServiceDB>();
            // builder.Services.AddScoped<IProductGroupService, ProductGroupService>();

            var app = builder.Build();

            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
            }
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            // Set culture
            CultureInfo.DefaultThreadCurrentCulture = new CultureInfo("en-US");
            CultureInfo.DefaultThreadCurrentUICulture = new CultureInfo("en-US");

            app.Run();
        }
    }
}
