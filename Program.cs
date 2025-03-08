using Microsoft.EntityFrameworkCore;
using QLTT.Helpers;
using QLTT.Models;
using QLTT.ModelFromDB;



namespace QLTT
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            //Connect VNPay API


            // Add services to the container
            builder.Services.AddControllersWithViews();
            builder.Services.AddDbContext<QLTK>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("cnn")));


            builder.Services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(30);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });
            builder.Services.AddLogging();

            // Momo API Payment
            var app = builder.Build();

            // Configure the HTTP request pipeline
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();
            app.UseSession();
            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}
