using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MovieStore.Models;


using MovieStore.Services;
using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;

namespace MovieStore.Controllers
{
    [ApiController]
    [Route("api/movies/{movieId}/ratings")]

    public class RatingController : ControllerBase
    {

        private readonly IMailService _mailService;
        private readonly IMovieRepository _movieRepository;
        private readonly IMapper _mapper;

        public RatingController(
            IMailService mailService, IMovieRepository movieRepository, IMapper mapper)
        {

            _mailService = mailService ?? 
                throw new ArgumentNullException(nameof(mailService));
            _movieRepository = movieRepository ??
                throw new ArgumentNullException(nameof(_movieRepository));
            _mapper = mapper ??
                throw new ArgumentNullException(nameof(mapper));
        }


        [HttpGet]
        public IActionResult GetRating(int movieId)
        {

                if(!_movieRepository.MovieExists(movieId))
                {

                    return NotFound();
                }

                var ratingForMoive = _movieRepository.GetRatingForMovie(movieId);

                return Ok(_mapper.Map<IEnumerable<RatingDto>>(ratingForMoive));

        }

        [HttpGet("{id}",Name = "GetRating")]
        public IActionResult GetRatings(int movieId, int id)
        {
            if (!_movieRepository.MovieExists(movieId))
                return NotFound();

            var rating = _movieRepository.GetRatingForMovie(movieId,id);

            if (rating == null)
                return NotFound();

            return Ok(_mapper.Map<RatingDto>(rating));

        }

        [HttpPost]
        public IActionResult CreationRating(int movieId,
            [FromBody] RatingForCreationDto rating)
        {

            if(rating.Description == rating.Name)
            {
                ModelState.AddModelError(
                    "Description",
                    "The provided description should be different from the name");
            }

            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (!_movieRepository.MovieExists(movieId))
                return NotFound();

            var finalRating = _mapper.Map<Entities.Rating>(rating);

            _movieRepository.AddRatingForMovie(movieId, finalRating);

            _movieRepository.Save();

            var createdRatingToReturn = _mapper
                .Map<Models.RatingDto>(finalRating);

            return CreatedAtRoute(
                "GetRating",
                new {movieId, id=createdRatingToReturn.Id },
                createdRatingToReturn);
        } 

        [HttpPut("{id}")]
        public IActionResult UpdateRating(int movieId, int id,
            [FromBody] RatingForUpdateDto rating)
        {
            if (rating.Description == rating.Name)
                ModelState.AddModelError(
                    "Description",
                    "The provided description should be diffrent from the name.");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (!_movieRepository.MovieExists(movieId))
                return NotFound();

            var ratingEntity = _movieRepository
                .GetRatingForMovie(movieId, id);

            if (ratingEntity == null)
                return NotFound();

            _mapper.Map(rating, ratingEntity);



            _movieRepository.Save();

            return NoContent();
        }

        [HttpPatch("{id}")]
        public IActionResult PartiallyUpdateRating(int movieId, int id,
            [FromBody] JsonPatchDocument<RatingForUpdateDto> patchDoc)
        {
            if (!_movieRepository.MovieExists(movieId))
                return NotFound();

            var ratingEntity = _movieRepository
                .GetRatingForMovie(movieId, id);
            if (ratingEntity == null)
                return NotFound();

            var ratingToPatch = _mapper
                .Map<RatingForUpdateDto>(ratingEntity);

            patchDoc.ApplyTo(ratingToPatch, ModelState);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (ratingToPatch.Description == ratingToPatch.Name)
                ModelState.AddModelError(
                    "Description",
                    "The provided description should be different form the name.");

            if (!TryValidateModel(ratingToPatch))
                return BadRequest(ModelState);


            _mapper.Map(ratingToPatch, ratingEntity);


            _movieRepository.Save();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteRating(int movieId, int id)
        {

            if (!_movieRepository.MovieExists(movieId))
                return NotFound();

            var ratingEntity = _movieRepository
                .GetRatingForMovie(movieId, id);
            if (ratingEntity == null)
                return NotFound();

            _movieRepository.DeleteRating(ratingEntity);

            _movieRepository.Save();

            _mailService.Sent("Rating deleted.",
                $"Rating {ratingEntity.Name} with id {ratingEntity.Id} was deleted");
            
            return NoContent();
        }
    }
}
