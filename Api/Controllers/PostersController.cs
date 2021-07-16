using Api.Dto;
using AutoMapper;
using DataAccess.Repository;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Api.Controllers
{
    [Route("api/movies/{movieId}/posters")]
    [ApiController]
    public class PostersController : ControllerBase
    {
        private readonly IPosterRepository _posterRepo;
        private readonly IMapper _mapper;

        public PostersController(IPosterRepository posterRepo, IMapper mapper)
        {
            _posterRepo = posterRepo;
            _mapper = mapper;
        }

        [HttpGet("{movieId}", Name = "GetPoster")]
        public async Task<IActionResult> GetPoster(int posterId)
        {
            if (posterId <= 0 )
                return BadRequest();
            var posterEntity = await _posterRepo.GetByIdAsync(posterId);
            return Ok(_mapper.Map<Dto.Poster>(posterEntity));
        }

        [HttpPost]
        public async Task<IActionResult> CreatePoster(int movieId, [FromBody] PosterForCreation poster)
        {
            if (poster == null || movieId <= 0)
                return BadRequest();

            // return 422 - Unprocessable Entity when validation fails
            if (!ModelState.IsValid)
                return new UnprocessableEntityObjectResult(ModelState);

            var posterEntity = _mapper.Map<Entity.Poster>(poster);
            posterEntity.MovieId = movieId;

            var posterId = await _posterRepo.AddAsync(posterEntity);
            poster.Id = posterId;

            return CreatedAtRoute("GetPoster", new { posterId = posterId, movieId = movieId },
                _mapper.Map<Dto.Poster>(posterEntity)
                );
        }
    }
}
