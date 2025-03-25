using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.EntityFrameworkCore;
using Tuna.Models;

var builder = WebApplication.CreateBuilder(args);

// allows our api endpoints to access the database through Entity Framework Core
builder.Services.AddNpgsql<TunaBouzoukiDbContext>(builder.Configuration["TunaBouzoukiDbConnectionString"]);

// Set the JSON serializer options
builder.Services.Configure<JsonOptions>(options =>
{
    options.SerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
});

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend",
        policy => policy.WithOrigins("http://localhost:3000") // Allow requests from frontend
                        .AllowAnyHeader()
                        .AllowAnyMethod());
});

var app = builder.Build();

app.UseStaticFiles();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(
        options => // UseSwaggerUI is called only in Development.
        {
            options.InjectStylesheet("/TunaBouzouki/styling.css");
        });
}

app.UseHttpsRedirection();

// ********* ARTIST ENDPOINTS **********

// GET Artists
app.MapGet("artist", (TunaBouzoukiDbContext db) =>
{
    try
    {
        var AS = db.Artists
        .Include(a => a.Songs)
        .ThenInclude(g => g.Genres)
        .ToList();
        return Results.Ok(AS);
    }
    catch
    {
        return Results.NotFound("No artists found");
    }
});

// GET Artist by Id
app.MapGet("artist/{id}", (TunaBouzoukiDbContext db, int id) =>
{
    try
    {
        var AS = db.Artists
        .Include(a => a.Songs)
        .FirstOrDefault(a => a.Id == id);
        return Results.Ok(AS);
    }
    catch
    {
        return Results.NotFound("No artist found");
    }
});

// CREATE Artist
app.MapPost("artist", (TunaBouzoukiDbContext db, Artist artist) =>
{
    db.Artists.Add(artist);
    db.SaveChanges();
    return Results.Created($"artist/{artist.Id}", artist);
});

// UPDATE Artist
app.MapPut("artist/{id}", (int id, TunaBouzoukiDbContext db, Artist updatedArtist) =>
{
    var existingArtist = db.Artists.FirstOrDefault(a => a.Id == id);

    if (existingArtist == null)
    {
        return Results.NotFound($"Artist with ID {id} not found.");
    }

    existingArtist.Name = updatedArtist.Name;
    existingArtist.Age = updatedArtist.Age;
    existingArtist.Bio = updatedArtist.Bio;

    db.SaveChanges();
    return Results.Ok(existingArtist);
});



// ********* GENRE ENDPOINTS **********

// GET Genres
app.MapGet("genre", (TunaBouzoukiDbContext db) =>
{
    try
    {
        var GS = db.Genres
        .Include(s => s.Songs)
        .ToList();
        return Results.Ok(GS);
    }
    catch
    {
        return Results.NotFound("Check your work");
    }
});

// GET Genre by Id
app.MapGet("genre/{id}", (TunaBouzoukiDbContext db, int id) =>
{
    try
    {
        var GS = db.Genres
        .Include(s => s.Songs)
        .FirstOrDefault(g => g.Id == id);
        return Results.Ok(GS);
    }
    catch
    {
        return Results.NotFound("Couldn't find that shit");
    }
});

// CREATE Genre
app.MapPost("genre", (TunaBouzoukiDbContext db, Genre newGenre) =>
{
    db.Genres.Add(newGenre);
    db.SaveChanges();
    return Results.Created($"genre/{newGenre.Id}", newGenre);
});

// UPDATE Genre
app.MapPut("genre/{id}", (int id, TunaBouzoukiDbContext db, Genre updatedGenre) =>
{
    var existingGenre = db.Genres.FirstOrDefault(g => g.Id == id);

    if (existingGenre == null)
    {
        return Results.NotFound($"Genre with ID {id} not found.");
    }

    existingGenre.Description = updatedGenre.Description;

    db.SaveChanges();
    return Results.Ok(existingGenre);
});

// DELETE Genre
app.MapDelete("genre/{id}", (int id, TunaBouzoukiDbContext db) =>
{
    var genreToDelete = db.Genres
        .Include(g => g.Songs) // Include Songs to clear join table links
        .FirstOrDefault(g => g.Id == id);

    if (genreToDelete == null)
    {
        return Results.NotFound($"Genre with ID {id} not found.");
    }

    // Remove links from the join table first
    genreToDelete.Songs.Clear();

    db.Genres.Remove(genreToDelete);
    db.SaveChanges();

    if (genreToDelete.Songs.Any())
    {
        return Results.BadRequest("Cannot delete genre that is still associated with songs.");
    }
    else
    {
        return Results.NoContent();
    }
});



// ********* SONG ENDPOINTS **********

// GET Songs
app.MapGet("song", (TunaBouzoukiDbContext db) =>
{
    try
    {
        var SG = db.Songs
        .Include(s => s.Artist)
        .Include(s => s.Genres)
        .ToList();
        return Results.Ok(SG);
    }
    catch
    {
        return Results.NotFound("Not found");
    }
});

// GET Song by Id
app.MapGet("song/{id}", (TunaBouzoukiDbContext db, int id) =>
{
    try
    {
        var SG = db.Songs
        .Include(s => s.Artist)
        .Include(g => g.Genres)
        .FirstOrDefault(s => s.Id == id);
        return Results.Ok(SG);
    }
    catch
    {
        return Results.NotFound("Fix your code");
    }
});

// CREATE Song
app.MapPost("song", (TunaBouzoukiDbContext db, Song newSong) =>
{
    // Replace any untracked genres with their actual tracked versions
    var genreIds = newSong.Genres.Select(g => g.Id).ToList();
    var existingGenres = db.Genres.Where(g => genreIds.Contains(g.Id)).ToList();

    // Replace the Genres list with the actual tracked genre entities
    newSong.Genres = existingGenres;

    db.Songs.Add(newSong);
    db.SaveChanges();
    return Results.Created($"song/{newSong.Id}", newSong);
});

// UPDATE Song
app.MapPut("song/{id}", (int id, TunaBouzoukiDbContext db, Song updatedSong) =>
{
    var existingSong = db.Songs
        .Include(s => s.Genres)
        .FirstOrDefault(s => s.Id == id);

    if (existingSong == null)
    {
        return Results.NotFound($"Song with ID {id} not found.");
    }

    // Update basic properties
    existingSong.Title = updatedSong.Title;
    existingSong.Album = updatedSong.Album;
    existingSong.Length = updatedSong.Length;
    existingSong.ArtistId = updatedSong.ArtistId;

    // Handle many-to-many relationship updates (Genres)
    if (updatedSong.Genres != null)
    {
        var genreIds = updatedSong.Genres.Select(g => g.Id).ToList();
        var matchedGenres = db.Genres.Where(g => genreIds.Contains(g.Id)).ToList();
        existingSong.Genres = matchedGenres;
    }

    db.SaveChanges();
    return Results.Ok(existingSong);
});

// DELETE Song
app.MapDelete("song/{id}", (int id, TunaBouzoukiDbContext db) =>
{
    var songToDelete = db.Songs
        .Include(s => s.Genres) // Include Genres to handle join table cleanup
        .FirstOrDefault(s => s.Id == id);

    if (songToDelete == null)
    {
        return Results.NotFound($"Song with ID {id} not found.");
    }

    // Clear genre links to avoid orphaned join records
    songToDelete.Genres.Clear();

    db.Songs.Remove(songToDelete);
    db.SaveChanges();

    return Results.NoContent();
});



app.Run();
