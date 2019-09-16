using System.Threading.Tasks;
using Hangfire;

namespace FspScraper.WebAPI.Jobs
{
    public interface ITimesScraperJob
    {
         Task Run(IJobCancellationToken token);
         Task UpdateTimes();
    }
}