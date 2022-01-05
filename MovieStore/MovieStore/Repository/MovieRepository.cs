using MovieStore.Contexts;
using MovieStore.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MovieStore.Services
{
    public class MovieRepository : IMovieRepository
    {
        private readonly MovieContext _context;

        public MovieRepository(MovieContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public IEnumerable<Movie> GetMovies()
        {
            return _context.Movies.ToList();
        }

        public Movie GetMovie(int movieId, bool includeRating)
        {
            if(includeRating)
                return _context.Movies.Include(c => c.Ratings)
                .Where(c => c.Id == movieId).FirstOrDefault();

            return _context.Movies
                    .Where(c => c.Id == movieId).FirstOrDefault();
        }

        public Movie GetMovie(int movieId)
        {
            return _context.Movies
                    .Where(c => c.Id == movieId).FirstOrDefault();
        }

        public Rating GetRatingForMovie(int movieId, int ratingId)
        {
            return _context.Ratings
                .Where(p => p.MovieId == movieId && p.Id == ratingId).FirstOrDefault();
        }

        public IEnumerable<Rating> GetRatingForMovie(int movieId)
        {
            return _context.Ratings
                .Where(p => p.MovieId == movieId).ToList();
        }

        public bool MovieExists(int movieId) 
        {
            return _context.Movies.Any(c =>c.Id == movieId);
        }

        public void AddRatingForMovie(int movieId, Rating rating)
        {
            var movie = GetMovie(movieId, false);
            movie.Ratings.Add(rating);
        }

        public bool Save()
        {
            return (_context.SaveChanges() >= 0);
        }

        public void DeleteRating(Rating rating)
        {
            _context.Ratings.Remove(rating);
        }
        public void DeleteMovie(Movie movie)
        {
            _context.Movies.Remove(movie);
        }

        public void AddMovie(Movie movie)
        {
            _context.Movies.Add(movie);
        }

    }
}
