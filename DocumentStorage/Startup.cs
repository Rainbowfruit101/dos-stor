using System.IO;
using System.Text.Json.Serialization;
using DbContexts;
using Mappers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Models;
using Services;
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

            services.AddScoped<IMapper<User, UserView>, UserMapper>();
            services.AddScoped<IMapper<Role, RoleView>, RoleMapper>();
            services.AddScoped<IMapper<Document, DocumentView>, DocumentMapper>();
            services.AddScoped<IMapper<Tag, TagView>, TagMapper>();

            services.AddScoped<UserService>(options =>
            {
                var dbContext = options.GetService<DocumentStorageContext>();
                return new UserService(dbContext, Configuration["PasswordHashSalt"]);
            });
            
            services.AddScoped<DocumentFileService>(options =>
            {
                var rootDirectoryPath = Configuration["RootDirectory"];
                if (!Directory.Exists(rootDirectoryPath))
                {
                    Directory.CreateDirectory(rootDirectoryPath);
                }

                return new DocumentFileService(new DirectoryInfo(rootDirectoryPath));
            });
            
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.RequireHttpsMetadata = false;
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateLifetime = true, // будет ли валидироваться время существования
                        
                        ValidateIssuer = true, // укзывает, будет ли валидироваться издатель при валидации токена
                        ValidIssuer = AuthOptions.Issuer, // строка, представляющая издателя
                        
                        ValidateAudience = true, // будет ли валидироваться потребитель токена
                        ValidAudience = AuthOptions.Audience, // установка потребителя токена
                        
                        ValidateIssuerSigningKey = true, // валидация ключа безопасности
                        IssuerSigningKey = AuthOptions.GetSymmetricSecurityKey(), // установка ключа безопасности
                        
                    };
                });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseDeveloperExceptionPage();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapDefaultControllerRoute();
            });
        }
    }
}
