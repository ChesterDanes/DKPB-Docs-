using BLL.DTOModels.ResponseDTO;
using BLL.ServiceInterfaces.Interfaces;
using BLL_EF;
using DAL;
using System.Globalization;
using BLL_DB.Services;
using BLL.Services;
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
            builder.Services.AddDbContext<WebstoreContext>();
           // builder.Services.AddScoped<IProductService, ProductService>();
            builder.Services.AddScoped<IUserService, UserService>();
           // builder.Services.AddScoped<IOrderService, OrderService>();
            builder.Services.AddScoped<IProductGroupService,ProductGroupService>();
            builder.Services.AddScoped<IOrderService, OrderServiceDB>();
            builder.Services.AddScoped<IProductService, ProductServiceDB>();
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

            CultureInfo.DefaultThreadCurrentCulture = new CultureInfo("en-US");
            CultureInfo.DefaultThreadCurrentUICulture = new CultureInfo("en-US");
            app.Run();
        }
    }
}
