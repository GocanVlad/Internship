using AutoMapper;
using MovieStore.Models;
using MovieStore.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MovieStore.Entities;
using Microsoft.AspNetCore.JsonPatch;

namespace MovieStore.Controllers
{
    [ApiController]
    [Route("api/movies")]
    public class MovieController : ControllerBase
    {
        private readonly IMovieRepository _movieRepository;
        private readonly IMapper _mapper;
        private readonly IMailService _mailService;

        public MovieController(IMovieRepository movieRepository, IMapper mapper, IMailService mailService)
        {
            _movieRepository = movieRepository ??
                throw new ArgumentNullException(nameof(_movieRepository));
            _mapper = mapper ??
                throw new ArgumentNullException(nameof(mapper));
            _mailService = mailService ??
                throw new ArgumentNullException(nameof(mailService)); 

        }

        [HttpGet]
        public IActionResult GetMovies()
        {
            var movieEntities = _movieRepository.GetMovies();

            return Ok(_mapper.Map<IEnumerable<MovieDto>>(movieEntities));
        }

        [HttpGet("{id}")]
        public IActionResult GetMovie(int id)
        {
            var movie = _movieRepository.GetMovie(id);

            if (movie == null)
                return NotFound();

            return Ok(_mapper.Map<MovieDto>(movie));

        }

        [HttpPost]
        public IActionResult CreateMovie([FromBody] MovieForCreationDto movie)
        {
            if (movie.Name == movie.Genre)
                ModelState.AddModelError(
                    "Description",
                    "The provided name should be different from the genre");

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var finalMovie = _mapper.Map<Entities.Movie>(movie);
            _movieRepository.AddMovie(finalMovie);

            _movieRepository.Save();

            var createdMovieToReturn = _mapper
                .Map<Models.MovieDto>(finalMovie);

            return Ok(createdMovieToReturn);

        }
        [HttpDelete("{id}")]
        public IActionResult DeleteMovie(int id)
        {

            var movieEntity = _movieRepository.GetMovie(id);

            _movieRepository.DeleteMovie(movieEntity);
            _movieRepository.Save();

            _mailService.Sent("Movie deleted.",
                $"Movie {movieEntity.Name} with id {movieEntity.Id} was deleted");

            return NoContent();
        }

        [HttpPut("{id}")]
        public IActionResult UpdateMovie(int id,
            [FromBody] MovieForCreationDto movie)
        {
            if (movie.Genre == movie.Name)
                ModelState.AddModelError(
                    "Description",
                    "The provided description should be diffrent from the name.");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (!_movieRepository.MovieExists(id))
                return NotFound();

            var movieEntity = _movieRepository
                .GetMovie(id);

            _mapper.Map(movie, movieEntity);

            _movieRepository.Save();

            return NoContent();
        }

    }
}
