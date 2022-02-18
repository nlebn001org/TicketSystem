using Autofac;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using TicketSystem.Web.LogServices;
using TicketSystem.Web.Models;
using TicketSystem.Web.RepositoryServices;

namespace TicketSystem.Web
{
    public class Startup
    {
        public IConfiguration Configuration { get; }
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {

            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.LoginPath = new Microsoft.AspNetCore.Http.PathString("/Account/Login/"); //anonymus will be redirrected here in cases of source access, where auth. will be required
                    options.AccessDeniedPath = new Microsoft.AspNetCore.Http.PathString("/Account/Login/");
                });

            services.AddControllersWithViews();
            //  services.AddDbContext<SystemDbContext>(options => options.UseSqlServer
            //("server=(localdb)\\MSSQLLocaldb;database=tsystemdb;trusted_connection=true"));

        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, DbContext db)
        {
            //db.Database.EnsureCreated();
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseStaticFiles();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
           {
               endpoints.MapControllerRoute("Default", "{controller=Home}/{action=Index}/{id?}");
           });
        }

        public void ConfigureContainer(ContainerBuilder builder)
        {
            IConfigurationRoot configuration = new ConfigurationBuilder()
                   .SetBasePath(Directory.GetCurrentDirectory())
                   .AddJsonFile("appsettings.json")
                   .Build();
            var connectionString = configuration.GetConnectionString("ConnectionString");

            //register Db context
            builder.Register(c =>
            {
                return new SystemDbContext(connectionString);
            })
                .As<DbContext>()
                .As<SystemDbContext>()
                .InstancePerDependency();

            //register repositories
            builder.RegisterAssemblyTypes(this.GetType().GetTypeInfo().Assembly)
           .AsClosedTypesOf(typeof(IRepository<>))
           .InstancePerDependency();

            //register logging service
            builder.RegisterType<ConsoleLogService>()
                .As<ILogService>()
                .SingleInstance();

        }
    }
}
