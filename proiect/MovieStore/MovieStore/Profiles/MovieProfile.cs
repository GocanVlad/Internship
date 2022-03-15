using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MovieStore.Profiles
{
    public class MovieProfile : Profile
    {
        public MovieProfile()
        {
            //first source, second destination
            CreateMap<Entities.Movie, Models.MovieDto>(); 
            CreateMap<Models.MovieForCreationDto,Entities.Movie>();
            

        }
    }
}
