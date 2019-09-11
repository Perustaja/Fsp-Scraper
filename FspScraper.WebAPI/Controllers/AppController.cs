using System.Linq;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using FspScraper.Scraper;
using FspScraper.Common;
using FspScraper.Common.Models.Contexts;
using Microsoft.EntityFrameworkCore;

namespace FspScraper.WebAPI.Controllers
{
    public class AppController : ControllerBase
    {
        private readonly FspTimesContext _timesContext;
        public AppController(FspTimesContext context)
        {
            _timesContext = context;
        }
        public async void UpdateTimes([FromServices] FspTimesScraper scraper)
        {
            var times = scraper.Run();
            if (ModelState.IsValid)
            {
                try
                {
                    _timesContext.UpdateRange(times);
                    await _timesContext.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    ModelState.AddModelError("", "Error updating times.");
                }
            }

        }
    }
}