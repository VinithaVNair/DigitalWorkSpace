using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using CatalogManaging.Core.Model.CatalogAggregate;
using CatalogManaging.Model;

namespace CatalogManaging.Profiles
{
    public class CardProfile : Profile
    {
        public CardProfile()
        {
            CreateMap<CardDto,Card>().ForMember(dest => dest.Id,
                opt => opt.MapFrom(src => src.CardId)).ForMember(dest => dest.Version,
                opt => opt.MapFrom(src => src.CardVersion));
            
        }
    }
}
