using Api.Dto;
using AutoMapper;
using DataAccess.Repository;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace Api.Controllers
{
    [Route("api/movies/posters")]
    [ApiController]
    public class PostersController : ControllerBase
    {
        private readonly IPosterRepository _posterRepo;
        private readonly IMoviesRepository _moviesRepo;
        private readonly IMapper _mapper;

        public PostersController(IPosterRepository posterRepo, IMoviesRepository moviesRepo, IMapper mapper)
        {
            _posterRepo = posterRepo;
            _moviesRepo = moviesRepo;
            _mapper = mapper;
        }

        [HttpGet("{posterId}", Name = "GetPoster")]
        public async Task<IActionResult> GetPoster(int posterId)
        {
            if (posterId <= 0 )
                return BadRequest();
            var posterEntity = await _posterRepo.GetByIdAsync(posterId);
            
            if (posterEntity == null)
                return NotFound();

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

            var movieEntity = await _moviesRepo.GetByIdAsync(movieId);

            if (movieEntity == null)
                throw new Exception($"Movie with Id:{movieId} si not found");
            if(poster.Bytes == null)
                GenerateRandomPosterData(poster, movieEntity);

            var posterEntity = _mapper.Map<Entity.Poster>(poster);
            posterEntity.MovieId = movieId;

            var posterId = await _posterRepo.AddAsync(posterEntity);
            poster.Id = posterId;

            return CreatedAtRoute("GetPoster", new { posterId = posterId, movieId = movieId },
                _mapper.Map<Dto.Poster>(posterEntity)
                );
        }

        private PosterForCreation GenerateRandomPosterData(PosterForCreation poster, Entity.Movie movie)
        {
            var random = new Random();

            //500KB
            var generatedBytes = new byte[524288];
            random.NextBytes(generatedBytes);
            poster.Bytes = generatedBytes;
            poster.Name = $"{movie.Title} poster number {DateTime.UtcNow.Ticks}";
            return poster;
        }
    }
}
