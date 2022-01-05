using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MovieStore.Profiles
{
    public class RatingProfile : Profile
    {
        public RatingProfile()
        {
            CreateMap<Entities.Rating, Models.RatingDto>();
            CreateMap<Models.RatingForCreationDto, Entities.Rating>();
            CreateMap<Models.RatingForUpdateDto, Entities.Rating>()
                .ReverseMap();
        }
    }
}
