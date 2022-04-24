using FilmBuff.Enums;
using FilmBuff.Models.Settings;
using FilmBuff.Models.TMDB;
using FilmBuff.Services.Interfaces;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;
using System.Runtime.Serialization.Json;

namespace FilmBuff.Services
{
    public class TMDBMovieService : IRemoteMovieService
    {
        private readonly AppSettings _appsettings;
        private readonly IHttpClientFactory _httpClient;

        public TMDBMovieService(IOptions<AppSettings> appsettings, IHttpClientFactory httpClient)
        {
            _appsettings = appsettings.Value;
            _httpClient = httpClient;
        }

        public async Task<ActorDetail> ActorDetailAsync(int id)
        {
            //Setup a default instance of MovieSearch
            ActorDetail actorDetail = new();

            //Assemble the full request uri string
            var query = $"{_appsettings.TMDBSettings.BaseUrl}/person/{id}";
            var queryParams = new Dictionary<string, string>()
            {
                {"api_key", _appsettings.FilmBuffSettings.TmDbApiKey },
                {"language", _appsettings.TMDBSettings.QueryOptions.Language }
            };

            var requestUri = QueryHelpers.AddQueryString(query, queryParams);

            //Crerate a client and execute the request
            var client = _httpClient.CreateClient();
            var request = new HttpRequestMessage(HttpMethod.Get, requestUri);
            var response = await client.SendAsync(request);

            //Return the MovieSearch object
            if (response.IsSuccessStatusCode)
            {
                using var responseStream = await response.Content.ReadAsStreamAsync();

                var dcjs = new DataContractJsonSerializer(typeof(ActorDetail));
                actorDetail = (ActorDetail)dcjs.ReadObject(responseStream);
            }

            return actorDetail;
        }

        public async Task<MovieDetail> MovieDetailAsync(int id)
        {
            //Setup a default instance of MovieSearch
            MovieDetail movieDetail = new();

            //Assemble the full request uri string
            var query = $"{_appsettings.TMDBSettings.BaseUrl}/movie/{id}";
            var queryParams = new Dictionary<string, string>()
            {
                {"api_key", _appsettings.FilmBuffSettings.TmDbApiKey },
                {"language", _appsettings.TMDBSettings.QueryOptions.Language },
                {"append_to_response", _appsettings.TMDBSettings.QueryOptions.AppendToResponse }
            };

            var requestUri = QueryHelpers.AddQueryString(query, queryParams);

            //Crerate a client and execute the request
            var client = _httpClient.CreateClient();
            var request = new HttpRequestMessage(HttpMethod.Get, requestUri);
            var response = await client.SendAsync(request);

            //Return the MovieSearch object
            if (response.IsSuccessStatusCode)
            {
                var dcjs = new DataContractJsonSerializer(typeof(MovieDetail));

                using var responseStream = await response.Content.ReadAsStreamAsync();
                movieDetail = dcjs.ReadObject(responseStream) as MovieDetail;
            }

            return movieDetail;
        }

        public async Task<MovieSearch> SearchMoviesAsync(MovieCategory category, int count)
        {
            //Setup a default instance of MovieSearch
            MovieSearch movieSearch = new();

            //Assemble the full request uri string
            var query = $"{_appsettings.TMDBSettings.BaseUrl}/movie/{category}";
            var queryParams = new Dictionary<string, string>()
            {
                {"api_key", _appsettings.FilmBuffSettings.TmDbApiKey },
                {"language", _appsettings.TMDBSettings.QueryOptions.Language },
                {"page", _appsettings.TMDBSettings.QueryOptions.Page }
            };

            var requestUri = QueryHelpers.AddQueryString(query, queryParams);

            //Crerate a client and execute the request
            var client = _httpClient.CreateClient();
            var request = new HttpRequestMessage(HttpMethod.Get, requestUri);
            var response = await client.SendAsync(request);

            //Return the MovieSearch object
            if (response.IsSuccessStatusCode)
            {
                var dcjs = new DataContractJsonSerializer(typeof(MovieSearch));

                using var responseStream = await response.Content.ReadAsStreamAsync();
                movieSearch = (MovieSearch)dcjs.ReadObject(responseStream);
                movieSearch.results = movieSearch.results.Take(count).ToArray();
                movieSearch.results.ToList().ForEach(r => r.poster_path = $"{_appsettings.TMDBSettings.BaseImagePath}/{_appsettings.FilmBuffSettings.DefaultPosterSize}/{r.poster_path}");
            }

            return movieSearch;
        }
    }
}
