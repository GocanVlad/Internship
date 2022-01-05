using MovieStore.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MovieStore.Services
{
    public interface IMovieRepository
    {
        IEnumerable<Movie> GetMovies();
        Movie GetMovie(int movieId, bool includeRatings);
        IEnumerable<Rating> GetRatingForMovie(int movieId);
        Rating GetRatingForMovie(int movieId, int ratingId);
        bool MovieExists(int movieId);
        void AddRatingForMovie(int movieId, Rating rating);
        bool Save();
        void DeleteRating(Rating rating);
        void DeleteMovie(Movie movie);
        void AddMovie(Movie movie);
        Movie GetMovie(int movieId);
    }
}
