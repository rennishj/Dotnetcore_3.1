using Dapper;
using DataAccess.ConnectionFactory;
using Entity;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace DataAccess.Repository
{
    public class MoviesRepository : IMoviesRepository
    {
        //https://medium.com/geekculture/using-dapper-and-sqlkata-in-net-core-for-high-performance-application-716d5fd43210
        private readonly IDatabaseConnectionProvider _connectionProvider;

        public MoviesRepository(IDatabaseConnectionProvider connectionProvider)
        {
            _connectionProvider = connectionProvider;
        }
        public async Task<int> AddAsync(Movie entity)
        {                    
            using (var conn = _connectionProvider.CreateConnection())
            {
                var movieId = -1;
                var param = new DynamicParameters();
                param.Add("@Title", entity.Title);
                param.Add("@Description", entity.Description);
                param.Add("@ReleaseDate", entity.ReleaseDate);
                param.Add("@Genre", entity.Genre);
                param.Add("@Director", entity.Director);
                param.Add("@Id", 0, DbType.Int32, ParameterDirection.Output);
                await conn.ExecuteAsync("dbo.CreateMovie",  param, commandType: CommandType.StoredProcedure);
                movieId = param.Get<int>("@Id");
                return movieId;
            }
        }

        public async Task<int> DeleteAsync(int id)
        {
            using (var conn = _connectionProvider.CreateConnection())
            {
                var param = new
                {
                    MovieId   = id
                };
                await conn.ExecuteAsync("dbo.DeleteMovieById", param, commandType: CommandType.StoredProcedure);
                return 1;
            }
        }

        public async Task<IReadOnlyList<Movie>> GetAllAsync()
        {
            using (var conn =  _connectionProvider.CreateConnection())
            {   
                var result = await conn.QueryAsync<Movie>("dbo.GetAllMovies");
                return result.ToList();
            }
        }

        public async Task<Movie> GetByIdAsync(int id)
        {
            using (var conn = _connectionProvider.CreateConnection())
            {
                var param = new 
                {
                    MovieId = id
                };                
                var result = await conn.QueryAsync<Movie>("dbo.GetMovieById", param, commandType: CommandType.StoredProcedure);
                return result.FirstOrDefault();
            }
        }

        public async Task<int> UpdateAsync(Movie movie)
        {
            using (var conn = _connectionProvider.CreateConnection())
            {
                var param = new
                {
                    MovieId = movie.Id,
                    Title = movie.Title,
                    Description = movie.Description,
                    Genre = movie.Genre,
                    ReleaseDate = movie.ReleaseDate,
                    Director = movie.Director
                };
                await conn.ExecuteAsync("dbo.UpdateMovieById", param, commandType: CommandType.StoredProcedure);
                return 1;
            }

        }
    }
}
