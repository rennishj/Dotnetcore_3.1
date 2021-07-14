using Api.Dto;
using AutoMapper;

namespace Api.AutoMapperProfile
{
    public class MovieProfile : Profile
    {
        public MovieProfile()
        {
            CreateMap<MovieForCreation, Entity.Movie>()
                .ForMember(movieModel => movieModel.CreatedOn, movieDto => movieDto.NullSubstitute(null));

            CreateMap<MovieForUpdate, Entity.Movie>();
            CreateMap<Entity.Movie, MovieForUpdate>();
        }
    }
}
