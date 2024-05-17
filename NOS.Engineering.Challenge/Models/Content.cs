using System.ComponentModel.DataAnnotations;

namespace NOS.Engineering.Challenge.Models;

public class Content
{
    [Key]
    public Guid Id { get; }

    [Required]
    public string Title { get; }

    [Required]
    public string SubTitle { get; }

    [Required]
    public string Description { get; }

    [Required]
    public string ImageUrl { get; }

    [Required]
    public int Duration { get; }

    [Required]
    public DateTime StartTime { get; }

    [Required]
    public DateTime EndTime { get; }

    [Required]
    public IEnumerable<string> GenreList { get; private set; }

    public Content(Guid id, string title, string subTitle, string description, string imageUrl, int duration, DateTime startTime, DateTime endTime, IEnumerable<string> genreList)
    {
        Id = id;
        Title = title;
        SubTitle = subTitle;
        Description = description;
        ImageUrl = imageUrl;
        Duration = duration;
        StartTime = startTime;
        EndTime = endTime;
        GenreList = genreList;
    }

    public void AddGenres(IEnumerable<string> genres)
    {
        GenreList = GenreList.Concat(genres).Distinct().ToList();
    }

    public void RemoveGenres(IEnumerable<string> genres)
    {
        GenreList = GenreList.Except(genres).ToList();
    }
}