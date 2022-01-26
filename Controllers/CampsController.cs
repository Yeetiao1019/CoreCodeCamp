using AutoMapper;
using CoreCodeCamp.Data;
using CoreCodeCamp.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoreCodeCamp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]     //使 Controller 作為 API 並接收 Json
    public class CampsController : ControllerBase
    {
        private readonly ICampRepository _campRepository;
        private readonly IMapper _mapper;
        private readonly LinkGenerator _linkGenerator;

        public CampsController(ICampRepository campRepository, IMapper mapper, LinkGenerator linkGenerator)
        {
            this._campRepository = campRepository;
            this._mapper = mapper;
            this._linkGenerator = linkGenerator;
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

        [HttpPost]
        public async Task<ActionResult<CampModel>> Post(CampModel campModel)        // Model Binding
        {
            try
            {
                var Existing = _campRepository.GetCampAsync(campModel.Moniker);
                if (Existing != null)
                {
                    return BadRequest("此綽號已存在");
                }

                var Location = _linkGenerator.GetPathByAction("Get",
                    "Camps",
                    new { moniker = campModel.Moniker });     // 製作呼叫 Get Action 這一個 API 的 URI

                if (string.IsNullOrWhiteSpace(Location))
                {
                    return BadRequest("無法使用此綽號");
                }

                // 新增 Camp
                var Camp = _mapper.Map<Camp>(campModel);
                _campRepository.Add(Camp);

                if (await _campRepository.SaveChangesAsync())
                {
                    return Created(Location, _mapper.Map<CampModel>(Camp));     //成功建立後會呼叫的 URI
                }
            }
            catch (Exception)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError, "Database Failure");
            }

            return BadRequest();
        }

        [HttpPut("{moniker}")]
        public async Task<ActionResult<CampModel>> Put(string moniker, CampModel campModel)
        {
            try
            {
                var OldCamp = await _campRepository.GetCampAsync(moniker);
                if (OldCamp == null) return NotFound($"找不到營區，{moniker} 此綽號不存在");

                _mapper.Map(campModel, OldCamp);

                if (await _campRepository.SaveChangesAsync())
                {
                    return _mapper.Map<CampModel>(OldCamp);
                }
            }
            catch (Exception)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError, "Database Failure");
            }

            return BadRequest();
        }
    }
}
