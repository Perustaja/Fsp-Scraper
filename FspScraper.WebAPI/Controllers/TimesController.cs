using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FspScraper.Common.Interfaces;
using FspScraper.Common.Models;
using Microsoft.AspNetCore.Mvc;

namespace FspScraper.WebAPI.Controllers
{
    [ApiController]
    [Route("api/")]
    public class TimesController : ControllerBase
    {
        private readonly ITimesRepository _timesRepo;
        public TimesController(ITimesRepository timesRepo)
        {
            _timesRepo = timesRepo ?? throw new ArgumentNullException(nameof(timesRepo));
        }

        [HttpGet("times")]
        public async Task<ActionResult<List<FspTimes>>> Times()
        {
            return Ok(await _timesRepo.GetTimes());
        }

        [HttpGet("times/{regNum}")]
        public async Task<ActionResult<FspTimes>> TimesByRegNum(string regNum)
        {
            var res = await _timesRepo.GetTimesByReg(regNum);
            if (res != null)
                return Ok(res);
            return NotFound(regNum);
        }
    }
}