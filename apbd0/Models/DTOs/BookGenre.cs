namespace apbd0.Models.DTOs;

public class BookGenre
{
    public int Id { get; set; }
    public string Title { get; set; }
    public List<Genre> Genres { get; set; } = null!;
}

public class Genre
{
    public string Name { get; set; }
}