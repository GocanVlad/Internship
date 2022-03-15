﻿// <auto-generated />
using MovieStore.Contexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace MovieStore.Migrations
{
    [DbContext(typeof(MovieContext))]
    [Migration("20211110102107_InitialMigration")]
    partial class InitialMigration
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("ProductVersion", "5.0.11")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("CityInfo.API.Entities.Movie", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Genre")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.HasKey("Id");

                    b.ToTable("Movies");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            Genre = "Adventure",
                            Name = "Transformers"
                        },
                        new
                        {
                            Id = 2,
                            Genre = "Comedy",
                            Name = "Family Guy"
                        },
                        new
                        {
                            Id = 3,
                            Genre = "Documentary",
                            Name = "Colectiv"
                        });
                });

            modelBuilder.Entity("CityInfo.API.Entities.Rating", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Description")
                        .HasMaxLength(200)
                        .HasColumnType("nvarchar(200)");

                    b.Property<int>("MovieId")
                        .HasColumnType("int");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.HasKey("Id");

                    b.HasIndex("MovieId");

                    b.ToTable("Ratings");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            Description = "Parents are urged to give parental guidance. This film may contain some material parents might not like for their young children.",
                            MovieId = 1,
                            Name = "PG"
                        },
                        new
                        {
                            Id = 2,
                            Description = "Score based on 100 viewers ratings",
                            MovieId = 1,
                            Name = "7.8"
                        },
                        new
                        {
                            Id = 3,
                            Description = "Intended for children ages 14 and older in the company of an adult. Possibly contains intensely suggestive dialogue, strong coarse language, intense sexual situations or intense violence.",
                            MovieId = 2,
                            Name = "TV-14"
                        },
                        new
                        {
                            Id = 4,
                            Description = "Score based on 1000 viewers ratings",
                            MovieId = 2,
                            Name = "8.8"
                        },
                        new
                        {
                            Id = 5,
                            Description = "May contain violence, nudity, sensuality, language, adult activities or other elements beyond a PG rating, but doesn’t reach the restricted R category.",
                            MovieId = 3,
                            Name = "PG-13"
                        },
                        new
                        {
                            Id = 6,
                            Description = "Score based on 1M viewers ratings",
                            MovieId = 3,
                            Name = "9.2"
                        });
                });

            modelBuilder.Entity("CityInfo.API.Entities.Rating", b =>
                {
                    b.HasOne("CityInfo.API.Entities.Movie", "Movie")
                        .WithMany("Ratings")
                        .HasForeignKey("MovieId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Movie");
                });

            modelBuilder.Entity("CityInfo.API.Entities.Movie", b =>
                {
                    b.Navigation("Ratings");
                });
#pragma warning restore 612, 618
        }
    }
}
