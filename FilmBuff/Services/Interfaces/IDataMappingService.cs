using FilmBuff.Models.Database;
using FilmBuff.Models.TMDB;

namespace FilmBuff.Services.Interfaces
{
    public interface IDataMappingService
    {
        public Task<Movie> MapMovieDetailAsync(MovieDetail movie);
        public ActorDetail MapActorDetail(ActorDetail actor);
    }
}
