using AutoMapper;

namespace Api.AutoMapperProfile
{
    public class MapperProfile : Profile
    {
        public MapperProfile()
        {
            CreateMap<Dto.MovieForCreation, Entity.Movie>()
                .ForMember(movieModel => movieModel.CreatedOn, movieDto => movieDto.NullSubstitute(null));

            CreateMap<Dto.MovieForUpdate, Entity.Movie>();
            CreateMap<Dto.MovieForUpdate, Entity.Movie>().ReverseMap();

            CreateMap<Dto.PosterForCreation, Entity.Poster>();

            CreateMap<Dto.Poster, Entity.Poster>().ReverseMap();
        }
    }
}
