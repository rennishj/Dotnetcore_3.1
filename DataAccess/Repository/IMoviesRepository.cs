using Entity;

namespace DataAccess.Repository
{
    public interface IMoviesRepository : IRepository<Movie> {  }
    public interface IPosterRepository : IRepository<Poster> { }

}
