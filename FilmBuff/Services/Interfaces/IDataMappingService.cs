using FilmBuff.Models.Database;
using FilmBuff.Models.TMDB;

namespace FilmBuff.Services.Interfaces
{
    public interface IDataMappingService
    {
        Task<Movie> MapMovieDetailAsync(MovieDetail movie);
        ActorDetail MapActorDetail(ActorDetail actor);
    }
}
