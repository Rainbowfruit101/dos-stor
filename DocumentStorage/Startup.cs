using DbContexts;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Mappers;
using Models;
using ViewModels.Views;

namespace DocumentStorage
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            services.AddControllers().AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
            });

            services.AddDbContext<DocumentStorageContext>(options => 
            {
                options.UseSqlServer(Configuration.GetConnectionString("Database"));
                //options.UseSqlServer("data source=localhost;initial catalog=DocumentsStorageDB;Trusted_connection=True;");
            });

            services.AddScoped<FileExtensionContentTypeProvider>();
            services.AddScoped<DocumentSearchService>();

            services.AddSingleton<IMapper<User, UserView>, UserMapper>();
            services.AddSingleton<IMapper<Role, RoleView>, RoleMapper>();
            services.AddSingleton<IMapper<Document, DocumentView>, DocumentMapper>();
            services.AddSingleton<IMapper<Tag, TagView>, TagMapper>();

            services.AddScoped<DocumentFileService>(options =>
            {
                var rootDirectoryPath = Configuration["RootDirectory"];
                if (!Directory.Exists(rootDirectoryPath))
                {
                    Directory.CreateDirectory(rootDirectoryPath);
                }

                return new DocumentFileService(new DirectoryInfo(rootDirectoryPath));
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseDeveloperExceptionPage();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapDefaultControllerRoute();
            });
        }
    }
}
