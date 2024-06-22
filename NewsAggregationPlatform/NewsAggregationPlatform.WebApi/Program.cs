using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using NewsAggregationPlatform.Data;
using NewsAggregationPlatform.Data.CQS.Queries.Articles;
using NewsAggregationPlatform.Services.Abstraction;
using NewsAggregationPlatform.Services.Implementation;
using System.Reflection;

namespace NewsAggregationPlatform.WebApi
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

            builder.Services.AddScoped<IArticleService, ArticleService>();
            builder.Services.AddScoped<ICategoryService, CategoryService>();
            builder.Services.AddScoped<ISourceService, SourceService>();

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(opt =>
            {
                opt.SwaggerDoc("v1", new OpenApiInfo()
                {
                    Title = "NewsAggregationPlatform.WebApi",
                    Version = "v1",
                    Description = "ASP.NET Core News Aggregation Platform Web API",

                });

                var xmlName = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                opt.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlName));
            });

            builder.Services.AddMediatR(cfg =>
               cfg.RegisterServicesFromAssembly(
                   typeof(GetArticlesWithNoTextIdAndUrlQuery).Assembly));

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
