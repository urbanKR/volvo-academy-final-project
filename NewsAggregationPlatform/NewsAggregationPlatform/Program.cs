using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using NewsAggregationPlatform.Data;
using NewsAggregationPlatform.Data.CQS.Queries.Articles;
using NewsAggregationPlatform.Interfaces;
using NewsAggregationPlatform.Models.Entities;
using NewsAggregationPlatform.Services.Abstraction;
using NewsAggregationPlatform.Services.Implementation;
using NewsAggregationPlatform.Sources;
using Serilog;

namespace NewsAggregationPlatform
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
            builder.Services.AddDbContext<AppDbContext>(options =>
                           options.UseSqlServer(connectionString));

            builder.Services.AddIdentity<User, IdentityRole<Guid>>(options =>
            {
                options.Password.RequiredLength = 8;
                options.User.RequireUniqueEmail = true;
                options.SignIn.RequireConfirmedEmail = false;
            })
            .AddEntityFrameworkStores<AppDbContext>().AddDefaultTokenProviders().AddDefaultUI();

            builder.Host.UseSerilog((context, configuration) => configuration.ReadFrom.Configuration(context.Configuration));

            builder.Services.AddScoped<IArticleService, ArticleService>();
            builder.Services.AddScoped<ICategoryService, CategoryService>();
            builder.Services.AddScoped<ISourceService, SourceService>();

            builder.Services.AddControllersWithViews();

            builder.Services.AddRazorPages();

            builder.Services.AddMediatR(cfg =>
               cfg.RegisterServicesFromAssembly(
                   typeof(GetArticlesWithNoTextIdAndUrlQuery).Assembly));


            builder.Services.AddScoped<IArticleSource, ESPNRssArticleSource>();
            builder.Services.AddScoped<IArticleSource, TheGuardianRssArticleSource>();
            builder.Services.AddScoped<IArticleSource, TheVergeRssArticleSource>();
            builder.Services.AddScoped<IPositivityAnalysisService, PositivityAnalysisService>();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseSerilogRequestLogging();

            app.UseRouting();


            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.MapRazorPages();

            app.Run();
        }
    }
}
