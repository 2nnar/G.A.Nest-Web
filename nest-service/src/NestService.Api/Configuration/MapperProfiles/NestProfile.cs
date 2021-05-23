using AutoMapper;
using NestService.Api.Models;
using NestService.Api.ViewModels;
using NestService.Api.ViewModels.NestObjects;
using System.Collections.Generic;

namespace NestService.Api.Configuration.MapperProfiles
{
#pragma warning disable CS1591 // Отсутствует комментарий XML для открытого видимого типа или члена
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

            CreateMap<IEnumerable<string>, GCodeResultViewModel>()
                .ForMember(dst => dst.Commands, opt => opt.MapFrom(src => src));
        }
    }
}
