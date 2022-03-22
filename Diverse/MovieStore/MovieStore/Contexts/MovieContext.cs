using MovieStore.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MovieStore.Contexts
{
    public class MovieContext :DbContext 
    {
        public DbSet<Movie> Movies { get; set; }
        public DbSet<Rating> Ratings { get; set; }

        public MovieContext(DbContextOptions<MovieContext> options):base(options)
        {
            Database.EnsureCreated();
        }
    }
}
