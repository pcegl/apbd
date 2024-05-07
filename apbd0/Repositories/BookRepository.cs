using apbd0.Models.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace Kolokwium.Repositories;

public class BookRepository : IBookRepository
{
    private readonly IConfiguration _configuration;
    public BookRepository(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task<bool> DoesBookExist(int id)
    {
        var query = "SELECT 1 FROM books WHERE ID = @ID";

        await using SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("Default"));
        await using SqlCommand command = new SqlCommand();

        command.Connection = connection;
        command.CommandText = query;
        command.Parameters.AddWithValue("@ID", id);

        await connection.OpenAsync();

        var res = await command.ExecuteScalarAsync();

        return res is not null;
    }
    
    public async Task<BookGenre> GetBook(int id)
    {
	    var query = @"SELECT 
							b.PK as IDBook, b.title as Title, g.name as Name
						FROM books_genres bg
						JOIN books b ON bg.FK_book = b.PK
						JOIN genres g ON g.PK = bg.FK_genre
						WHERE b.PK = @ID";
	    
	    await using SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("Default"));
	    await using SqlCommand command = new SqlCommand();
	    
	    command.Connection = connection;
	    command.CommandText = query;
	    command.Parameters.AddWithValue("@ID", id);
	    
	    await connection.OpenAsync();

	    var reader = await command.ExecuteReaderAsync();
	    
	    var idBookOrdinal = reader.GetOrdinal("IDBook");
	    var titleOrdinal = reader.GetOrdinal("Title");
	    var nameOrdinal = reader.GetOrdinal("Name");

	    BookGenre bookGenre = null;

	    while (await reader.ReadAsync())
	    {
		    if (bookGenre is not null)
		    {
			    bookGenre.Genres.Add(new Genre()
			    {
				    Name = reader.GetString(nameOrdinal),
			    });
		    }
		    else
		    {
			    bookGenre = new BookGenre()
			    {
				    Id = reader.GetInt32(idBookOrdinal),
				    Title = reader.GetString(titleOrdinal),
				    Genres = new List<Genre>()
				    {
					    new Genre()
					    {
						    Name = reader.GetString(nameOrdinal),
					    }
				    }
			    };
		    }
	    }

	    if (bookGenre is null) throw new Exception();
        
		    return bookGenre;
	}
    
    public async Task AddNewBook(NewBookGenre newBook)
    {
	    var insert = @"INSERT INTO books VALUES(@title);
					   SELECT @@IDENTITY AS ID;";
	    
	    await using SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("Default"));
	    await using SqlCommand command = new SqlCommand();
	    
	    command.Connection = connection;
	    command.CommandText = insert;
	    
	    command.Parameters.AddWithValue("@title", newBook.Title);
	    
	    
	    await connection.OpenAsync();

	    var transaction = await connection.BeginTransactionAsync();
	    command.Transaction = transaction as SqlTransaction;
	    
	    try
	    {
		    var id = await command.ExecuteScalarAsync();
    
		    foreach (var genre in newBook.Genres)
		    {
			    command.Parameters.Clear();
			    command.CommandText = "INSERT INTO books_genres VALUES(@@IDENTITY, @nr)";
			    command.Parameters.AddWithValue("@nr", genre);

			    await command.ExecuteNonQueryAsync();
		    }

		    await transaction.CommitAsync();
	    }
	    catch (Exception)
	    {
		    await transaction.RollbackAsync();
		    throw;
	    }
    }

    

}