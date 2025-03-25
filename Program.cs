using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.EntityFrameworkCore;

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
        return Results.Ok(db.Artists.FirstOrDefault(a => a.Id == id));
    }
    catch
    {
        return Results.NotFound("No artist found");
    }
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

// ********* SONG ENDPOINTS **********

// GET Songs
app.MapGet("song", (TunaBouzoukiDbContext db) =>
{
    try
    {
        var SG = db.Songs
        .Include(s => s.Genres)
        .ToList();
        return Results.Ok(SG);
    }
    catch
    {
        return Results.NotFound("Not found");
    }
});

app.MapGet("song/{id}", (TunaBouzoukiDbContext db, int id) =>
{
    try
    {
        var SG = db.Songs
        .Include(g => g.Genres)
        .FirstOrDefault(s => s.Id == id);
        return Results.Ok(SG);
    }
    catch
    {
        return Results.NotFound("Fix your code");
    }
});

app.Run();
