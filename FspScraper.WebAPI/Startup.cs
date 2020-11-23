using FspScraper.Common.Models.Contexts;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using FspScraper.WebAPI.Services;
using Hangfire;
using Hangfire.SQLite;
using FspScraper.WebAPI.Jobs;
using FspScraper.Common.Interfaces;
using FspScraper.Scraper;
using FspScraper.Common.Options;

namespace FspScraper.WebAPI
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<FspTimesContext>(options => options.UseSqlite(
                Configuration.GetConnectionString("FspTimesContext"),
                assembly => assembly.MigrationsAssembly("FspScraper.WebAPI")));
            // NOTE: The Hangfire connection string ends with a ;. This is because of the SQLite extension checking for one. 
            services.AddHangfire(config => config.UseSQLiteStorage(Configuration.GetConnectionString("HangfireConnection")));

            services.Configure<LoginOptions>(Configuration.GetSection("Login"));
            services.AddScoped<ITimesScraper, FspTimesScraper>();
            services.AddScoped<ITimesParser, FspTimesParser>();
            services.AddScoped<ITimesRepository, TimesRepository>();
            services.AddScoped<ITimesScraperJob, TimesScraperJob>();

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseHangfireServer(new BackgroundJobServerOptions
            {
                WorkerCount = 1
            });

            app.UseHttpsRedirection();
            app.UseMvc();
            GlobalJobFilters.Filters.Add(new AutomaticRetryAttribute {Attempts = 0});
            BackgroundJob.Enqueue<ITimesScraperJob>(j => j.Run(JobCancellationToken.Null)); // Setup one-time scraper job on start
            HangfireScheduler.ScheduleRecurringJobs();
        }
    }
}
