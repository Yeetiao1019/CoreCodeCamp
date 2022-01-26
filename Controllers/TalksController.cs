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
    [ApiController]
    [Route("api/camps/{moniker}/talks")]
    public class TalksController : ControllerBase
    {
        private readonly ICampRepository _campRepository;
        private readonly IMapper _mapper;
        private readonly LinkGenerator _linkGenerator;

        public TalksController(ICampRepository campRepository, IMapper mapper, LinkGenerator linkGenerator)
        {
            this._campRepository = campRepository;
            this._mapper = mapper;
            this._linkGenerator = linkGenerator;
        }

        [HttpGet]
        public async Task<ActionResult<TalkModel[]>> Get(string moniker)
        {
            try
            {
                var Talks = await _campRepository.GetTalksByMonikerAsync(moniker);

                return _mapper.Map<TalkModel[]>(Talks);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "伺服器錯誤");
            }
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<TalkModel>> Get(string moniker, int id)
        {
            try
            {
                var Talk = await _campRepository.GetTalkByMonikerAsync(moniker, id);

                return _mapper.Map<TalkModel>(Talk);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "伺服器錯誤");
            }
        }

        [HttpPost]
        public async Task<ActionResult<TalkModel>> Post(string moniker, TalkModel talkModel)
        {
            var Camp = await _campRepository.GetCampAsync(moniker);

            if (Camp == null) return NotFound("找不到此營區");

            var Talk = _mapper.Map<Talk>(talkModel);
            Talk.Camp = Camp;

            if (talkModel.Speaker == null) return BadRequest("Speaker ID 是必要的");
            var Speaker = await _campRepository.GetSpeakerAsync(talkModel.Speaker.SpeakerId);
            if (Speaker == null) return BadRequest("Speaker 不存在");

            Talk.Speaker = Speaker;
            _campRepository.Add(Talk);

            if (await _campRepository.SaveChangesAsync())
            {
                var Location = _linkGenerator.GetPathByAction(HttpContext
                    , "Get"
                    , values : new { moniker = moniker, id = Talk.TalkId });

                return Created(Location, _mapper.Map<TalkModel>(Talk));
            }
            else
            {
                return BadRequest("新增失敗");
            }
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult<TalkModel>> Put(string moniker, int id, TalkModel talkModel)
        {
            try
            {
                var Talk = await _campRepository.GetTalkByMonikerAsync(moniker, id);
                if (Talk == null) return BadRequest("找不到 Talk");

                _mapper.Map(talkModel, Talk);

                if(talkModel.Speaker != null)
                {
                    var Speaker = await _campRepository.GetSpeakerAsync(talkModel.Speaker.SpeakerId);
                    if(Speaker != null)
                    {
                        Talk.Speaker = Speaker;
                    }
                }

                if(await _campRepository.SaveChangesAsync())
                {
                    return _mapper.Map<TalkModel>(Talk);
                }
                else
                {
                    return BadRequest("無法更新資料");
                }

            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "伺服器錯誤");
            }
        }
    }
}
