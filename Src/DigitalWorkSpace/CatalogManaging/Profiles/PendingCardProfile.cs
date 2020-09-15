using CatalogManaging.Core.Model.CatalogAggregate;
using CatalogManaging.Model;
using AutoMapper;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CatalogManaging.Profiles
{
    public class PendingCardProfile : Profile
    {
        public PendingCardProfile()
        {
            CreateMap<CardDto,PendingCard>().ForMember(dest => dest.Id,
               opt => opt.MapFrom(src => src.CardId)).ForMember(dest => dest.Version,
               opt => opt.MapFrom(src => src.CardVersion));
        }
    }
}
