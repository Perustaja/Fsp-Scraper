using System.Reflection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FspScraper.Common.Models.Contexts;
using FspScraper.Scraper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Sqlite;
using FspScraper.WebAPI.Services;
using Hangfire;
using Hangfire.SQLite;
using FspScraper.WebAPI.Jobs;

namespace FspScraper.WebAPI
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
            services.AddDbContext<FspTimesContext>(options => options.UseSqlite(
                Configuration.GetConnectionString("FspTimesContext"),
                assembly => assembly.MigrationsAssembly("FspScraper.WebAPI")));
            // NOTE: The Hangfire connection string ends with a ;. This is because of the SQLite extension checking for one. 
            services.AddHangfire(config => config.UseSQLiteStorage(Configuration.GetConnectionString("HangfireConnection")));
            services.AddScoped<ITimesRepository, TimesRepository>();
            services.AddScoped<ITimesScraperJob, TimesScraperJob>();
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                // app.UseHsts();
            }
            app.UseHangfireServer(new BackgroundJobServerOptions
            {
                WorkerCount = 1
            });

            // app.UseHttpsRedirection();
            app.UseMvc();
            GlobalJobFilters.Filters.Add(new AutomaticRetryAttribute {Attempts = 0});
            HangfireScheduler.ScheduleRecurringJobs();
        }
    }
}
