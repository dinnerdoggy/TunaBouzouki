using Microsoft.EntityFrameworkCore;
using Tuna.Models;

public class TunaBouzoukiDbContext : DbContext
{
    public DbSet<Artist> Artists { get; set; }
    public DbSet<Song> Songs { get; set; }
    public DbSet<Genre> Genres { get; set; }

    public TunaBouzoukiDbContext(DbContextOptions<TunaBouzoukiDbContext> context) : base(context)
    {

    }

    //public TunaBouzoukiDbContext()
    //{
    //}


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Song>()
            .HasMany(s => s.Genres)
            .WithMany(g => g.Songs)
            .UsingEntity(j => j.HasData(
                new {GenresId = 5, SongsId = 1},
                new {GenresId = 2, SongsId = 2},
                new {GenresId = 3, SongsId = 3},
                new {GenresId = 4, SongsId = 4},
                new {GenresId = 1, SongsId = 5}
                ));

        modelBuilder.Entity<Artist>().HasData(new Artist[]
        {
            new Artist { Id = 1, Name = "Cannibal Corpse", Age = 36, Bio = "Heavy Deathmetal Band from Buffalo, New York" },
            new Artist { Id = 2, Name = "Infant Annihilator", Age = 13, Bio = "Technical Deathcore band based on the internet. The band members live in different parts of the world" },
            new Artist { Id = 3, Name = "Battles", Age = 23, Bio = "Experimental loop artist band from NYC" },
            new Artist { Id = 4, Name = "Animal Collective", Age = 27, Bio = "Alt-indie psychedelic" },
            new Artist { Id = 5, Name = "Shirley Collins", Age = 70, Bio = "English Folk Singer" },
        });

        modelBuilder.Entity<Song>().HasData(new Song[]
        {
            new Song { Id = 1, Title = "Barbara Allen", ArtistId = 5, Album = "English Fold Classics", Length = 316 },
            new Song { Id = 2, Title = "Behold the Kingdom of the Wretched Undying", ArtistId = 2, Album = "The Elysian Grandeval Galèriarch", Length = 1741 },
            new Song { Id = 3, Title = "A Loop So Nice / They Played it Twice", ArtistId = 3, Album = "Juice B Crypts", Length = 523 },
            new Song { Id = 4, Title = "Prester John", ArtistId = 4, Album = "Time Skiffs", Length = 629 },
            new Song { Id = 5, Title = "Scourge of Iron", ArtistId = 1, Album = "Torture", Length = 444 }
        });

        modelBuilder.Entity<Genre>().HasData(new Genre[]
        {
            new Genre { Id = 1, Description = "Deathmetal" },
            new Genre { Id = 2, Description = "Deathcore" },
            new Genre { Id = 3, Description = "Avant-Garde" },
            new Genre { Id = 4, Description = "Indie" },
            new Genre { Id = 5, Description = "Folk" },
        });
    }
}
