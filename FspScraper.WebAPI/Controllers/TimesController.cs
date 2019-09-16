using System.Threading.Tasks;
using FspScraper.Common.Models;
using FspScraper.Common.Models.Contexts;
using FspScraper.WebAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace FspScraper.WebAPI.Controllers
{
    [ApiController, Route("api/[controller]")]
    public class TimesController : ControllerBase
    {
        private readonly ITimesRepository _timesRepository;
        public TimesController(ITimesRepository repo)
        {
            _timesRepository = repo;
        }
        // GET api/times
        [HttpGet]
        public async Task<ActionResult<FspTimes[]>> Get()
        {
            return await _timesRepository.GetTimes();
        }
        // GET /api/times/regNum
        [HttpGet("{regNum}")]
        public async Task<ActionResult<FspTimes>> Get(string regNum)
        {
            return await _timesRepository.GetTimesByReg(regNum);
        }
    }
}