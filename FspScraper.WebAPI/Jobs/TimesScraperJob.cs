using System;
using System.Linq;
using System.Threading.Tasks;
using FspScraper.Common.Interfaces;
using FspScraper.Common.Models.Contexts;
using Hangfire;
using Microsoft.Extensions.Logging;

namespace FspScraper.WebAPI.Jobs
{
    public class TimesScraperJob : ITimesScraperJob
    {
        private readonly ILogger _logger;
        private readonly FspTimesContext _timesContext;
        private readonly ITimesScraper _scraper;
        public TimesScraperJob(ILogger<TimesScraperJob> logger, FspTimesContext context, ITimesScraper scraper)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));;
            _timesContext = context ?? throw new ArgumentNullException(nameof(context));;
            _scraper = scraper ?? throw new ArgumentNullException(nameof(scraper));
        }
        public async Task Run(IJobCancellationToken token)
        {
            token.ThrowIfCancellationRequested();
            await UpdateTimes();
        }
        public async Task UpdateTimes()
        {
            _logger.LogInformation($"{DateTime.Now}: Attempting to start scraping background service.");
            var times = _scraper.Run();
            foreach (var set in times)
            {
                if (_timesContext.Times.Any(e => e.RegistrationNum == set.RegistrationNum))
                    _timesContext.Update(set);
                else 
                    _timesContext.Add(set);
            }
            await _timesContext.SaveChangesAsync();
            _logger.LogInformation($"{DateTime.Now}: Scraping service finished.");
        }
    }
}