using Microsoft.EntityFrameworkCore;
using Tuna.Models;

public class TunaBouzoukiDbContext : DbContext
{
    public DbSet<Artist> Artists { get; set; }
    public DbSet<Song> Songs { get; set; }
    public DbSet<Genre> Genres { get; set; }

    public TunaBouzoukiDbContext(DbContextOptions<TunaBouzoukiDbContext> options) : base(options)
    {

    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
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
            new Song { Id = 1, Title = "Barbara Allen", ArtistId = 5, Album = "English Fold Classics", Length = 3 }
        });
    }
}