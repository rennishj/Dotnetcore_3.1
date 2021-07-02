using Dapper;
using DataAccess.ConnectionFactory;
using Model;
using System;
using System.Collections.Generic;
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
                var result = await conn.ExecuteAsync("dbo.CreateMovie", entity);
                return result;
            }
        }

        public Task<int> DeleteAsync(int id)
        {
            throw new NotImplementedException();
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
                var result = await conn.QueryAsync<Movie>("dbo.GetMovieById");
                return result.FirstOrDefault();
            }
        }

        public Task<int> UpdateAsync(Movie entity)
        {
            throw new NotImplementedException();
        }
    }
}
