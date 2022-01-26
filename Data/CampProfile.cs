using AutoMapper;
using CoreCodeCamp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoreCodeCamp.Data
{
    public class CampProfile : Profile
    {
        public CampProfile()
        {
            this.CreateMap<Camp, CampModel>()
                .ForMember(c => c.Venue, o => o.MapFrom(m => m.Location.VenueName))      //自訂 Mapping 欄位
                .ReverseMap();

            this.CreateMap<Talk, TalkModel>()
                .ReverseMap()
                .ForMember(t => t.Camp , o => o.Ignore())
                .ForMember(t => t.Speaker, o => o.Ignore());        //避免 null 錯誤

            this.CreateMap<Speaker, SpeakerModel>()
                .ReverseMap();
        }
    }
}
