namespace apbd0.Models.DTOs;

public class NewBookGenre
{
    public string Title { get; set; }
    public IEnumerable<int> Genres { get; set; } = new List<int>();

}