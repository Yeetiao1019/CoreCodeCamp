using AutoMapper;
using CoreCodeCamp.Data;
using CoreCodeCamp.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoreCodeCamp.Controllers
{
    [Route("api/[controller]")]
    public class CampsController : ControllerBase
    {
        private readonly ICampRepository _campRepository;
        private readonly IMapper _mapper;

        public CampsController(ICampRepository campRepository, IMapper mapper)
        {
            this._campRepository = campRepository;
            this._mapper = mapper;
        }
        [HttpGet]
        public async Task<ActionResult<CampModel[]>> Get(bool includeTalks = false)
        {
            try
            {
                var Result = await _campRepository.GetAllCampsAsync(includeTalks);

                return _mapper.Map<CampModel[]>(Result);
            }
            catch (Exception)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError, "Database Failure");
            }
        }
        [HttpGet("{moniker}")]      //HttpGet Template 值要與 Action 的參數名稱一致
        public async Task<ActionResult<CampModel>> Get(string moniker)
        {
            try
            {
                var Result = await _campRepository.GetCampAsync(moniker);
                if (Result == null) return NotFound();

                return _mapper.Map<CampModel>(Result);
            }
            catch (Exception)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError, "Database Failure");
            }
        }

        [HttpGet("search")]
        public async Task<ActionResult<CampModel[]>> GetByEventDate(DateTime theDate, bool includeTalks = false)
        {
            try
            {
                var Result = await _campRepository.GetAllCampsByEventDate(theDate, includeTalks);

                if (!Result.Any()) return NotFound();

                return _mapper.Map<CampModel[]>(Result);
            }
            catch (Exception)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError, "Database Failure");
            }
        }
    }
}
