using Api.Dto;
using AutoMapper;
using DataAccess.Repository;
using Microsoft.AspNetCore.Mvc;
using Entity;
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

        [HttpPost("create")]        
        public async Task<IActionResult> CreateMovie([FromBody] MovieForCreation movieForCreation)
        {
            if (movieForCreation == null)
                return BadRequest();

            if (!ModelState.IsValid)
                return new UnprocessableEntityObjectResult(ModelState);
            var movieEntity = _mapper.Map<Movie>(movieForCreation);
            var movieId = await _movieRepo.AddAsync(movieEntity);
            movieEntity.Id = movieId;
            return CreatedAtRoute("GetMovie", new { movieId = movieId}, _mapper.Map<Movie>(movieEntity));
        }

        [HttpGet]
        [Route("getallmovies")]
        public async Task<IActionResult> GetAllMovies() =>
            Ok(await _movieRepo.GetAllAsync());

        [HttpGet("{movieId}", Name = "GetMovie")]        
        public async Task<IActionResult> GetMovie([FromQuery] int movieId) =>
            Ok(await _movieRepo.GetByIdAsync(movieId));

        /// <summary>
        /// https://localhost:44300/api/movies/5 --> This is how the url should be to call this api
        /// </summary>
        /// <param name="movieId"></param>
        /// <param name="movieToUpdate"></param>
        /// <returns></returns>
        [HttpPut("{movieId}")]
        public async Task<IActionResult> Put(int movieId, [FromBody] MovieForUpdate movieToUpdate)
        {
            if (movieToUpdate == null)
                return BadRequest();

            var movieEntity = await _movieRepo.GetByIdAsync(movieId);
            if (movieEntity == null)
                return NotFound();

            await _movieRepo.UpdateAsync(_mapper.Map<MovieForUpdate, Movie>(movieToUpdate));

            return Ok(_mapper.Map<MovieForUpdate>(movieEntity));
        }

        [HttpDelete("{movieId}")]
        public async Task<IActionResult> Delete(int movieId)
        {
            if (movieId <= 0)
                return BadRequest();

            var movieEntity = await _movieRepo.GetByIdAsync(movieId);

            if (movieEntity == null)
                return NotFound();

            await _movieRepo.DeleteAsync(movieId);
            return NoContent();
        }

    }
}
