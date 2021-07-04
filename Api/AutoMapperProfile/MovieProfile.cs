using Api.Dto;
using AutoMapper;
using Model;

namespace Api.AutoMapperProfile
{
    public class MovieProfile : Profile
    {
        public MovieProfile()
        {
            CreateMap<MovieForCreation, Movie>()
                .ForMember(movieModel => movieModel.CreatedOn, movieDto => movieDto.NullSubstitute(null));
        }
    }
}
