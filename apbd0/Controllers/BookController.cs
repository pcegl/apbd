using apbd0.Models.DTOs;
using Kolokwium.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace apbd0.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BookController : ControllerBase
{
    private readonly IBookRepository _bookRepository;
    public BookController(IBookRepository prescriptionRepository)
    {
        _bookRepository = prescriptionRepository;
    }
    [HttpGet("{id}/genres")]
    public async Task<IActionResult> GetPrescription(int id)
    {
        var prescription = await _bookRepository.GetBook(id);
            
        return Ok(prescription);
    }
    
    [HttpPost]
    public async Task<IActionResult> AddAnimal(NewBookGenre newBook)
    {
        await _bookRepository.AddNewBook(newBook);

        return Created(Request.Path.Value ?? "api/books", newBook);
    }
}