using Dapper;
using DataAccess.ConnectionFactory;
using Entity;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace DataAccess.Repository
{
    public class PosterRepository : IPosterRepository
    {
        private readonly IDatabaseConnectionProvider _connectionProvider;

        public PosterRepository(IDatabaseConnectionProvider connectionProvider)
        {
            _connectionProvider = connectionProvider;
        }
        public async Task<int> AddAsync(Poster entity)
        {
            using (var conn = _connectionProvider.CreateConnection())
            {
                var posterId = -1;
                var param = new DynamicParameters();
                param.Add("@MovieId", entity.MovieId);
                param.Add("@Name", entity.Name);
                param.Add("@Bytes", entity.Bytes);
                param.Add("@Id", 0, DbType.Int32, ParameterDirection.Output);
                await conn.ExecuteAsync("dbo.CreatePoster", param, commandType: CommandType.StoredProcedure);
                posterId = param.Get<int>("@Id");
                return posterId;
            }
        }

        public Task<int> DeleteAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<IReadOnlyList<Poster>> GetAllAsync()
        {
            throw new NotImplementedException();
        }

        public Task<Poster> GetByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<int> UpdateAsync(Poster entity)
        {
            throw new NotImplementedException();
        }
    }
}
