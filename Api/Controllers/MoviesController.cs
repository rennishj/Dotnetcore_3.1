using Api.Dto;
using AutoMapper;
using DataAccess.Repository;
using Microsoft.AspNetCore.Mvc;
using Model;
using System;
using System.Threading.Tasks;

namespace Api.Controllers
{
    //https://codewithmukesh.com/blog/dapper-in-aspnet-core/
    [Route("api/movies")]
    [ApiController]
    public class MoviesController : ControllerBase
    {
        private readonly IMoviesRepository _movieRepo;
        private readonly IMapper _mapper;

        public MoviesController(IMoviesRepository repo, IMapper mapper)
        {
            _movieRepo = repo ?? throw new ArgumentNullException(nameof(repo));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        [HttpPost]
        public async Task<IActionResult> CreateMovie([FromBody] MovieForCreation movieForCreation)
        {
            if (movieForCreation == null)
                return BadRequest();

            if (!ModelState.IsValid)
                return new UnprocessableEntityObjectResult(ModelState);
            var movieEntity = _mapper.Map<Movie>(movieForCreation);
            var movieId = await _movieRepo.AddAsync(movieEntity);
            movieEntity.Id = movieId;
            return CreatedAtRoute("movie", new { movieId = movieId}, _mapper.Map<Movie>(movieEntity));
        }

        [HttpGet]
        [Route("getallmovies")]
        public async Task<IActionResult> GetAllMovies() =>
            Ok(await _movieRepo.GetAllAsync());

        [HttpGet]
        [Route("movie")]
        public async Task<IActionResult> GetMovie([FromQuery] int movieId) =>
            Ok(await _movieRepo.GetByIdAsync(movieId));

    }
}
