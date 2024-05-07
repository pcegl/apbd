using apbd0.Models.DTOs;

namespace Kolokwium.Repositories;

public interface IBookRepository
{
    Task<bool> DoesBookExist(int id);

    Task<BookGenre> GetBook(int id);
    Task AddNewBook(NewBookGenre newBook);

}