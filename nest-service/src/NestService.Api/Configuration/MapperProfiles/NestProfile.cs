using AutoMapper;
using NestService.Api.Models;
using NestService.Api.ViewModels;
using NestService.Api.ViewModels.NestObjects;
using System.Collections.Generic;

namespace NestService.Api.Configuration.MapperProfiles
{
    public class NestProfile : Profile
    {
        public NestProfile()
        {
            CreateMap<NestObjectViewModel, NestObject>()
                .Include<NestPolygonViewModel, NestPolygon>()
                .Include<NestCurveViewModel, NestCurve>()
                .Include<NestGroupViewModel, NestGroup>();
            CreateMap<NestPolygonViewModel, NestPolygon>();
            CreateMap<NestCurveViewModel, NestCurve>();
            CreateMap<NestGroupViewModel, NestGroup>();
            CreateMap<IEnumerable<NestObjectPlacement>, NestResultViewModel>()
                .ForMember(dst => dst.Placements, opt => opt.MapFrom(src => src));
            CreateMap<NestObjectPlacement, NestObjectPlacementViewModel>();
            CreateMap<NestObjectPoint, NestObjectPointViewModel>();
            CreateMap<NestObjectPointViewModel, NestObjectPoint>();
        }
    }
}
