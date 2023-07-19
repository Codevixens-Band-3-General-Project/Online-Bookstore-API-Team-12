using AutoMapper;
using BookStore__Management_system.Data;
using BookStore__Management_system.Models;
using BookStore__Management_system.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookStore__Management_system.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BooksController : ControllerBase
    {
        private readonly IBookRepository _bookRepository;
        private readonly AuthService authService;
        private readonly IMapper mapper;
        private readonly ILogger logger;

        public BooksController(IBookRepository bookRepository, AuthService authService, IMapper mapper, ILogger logger)
        {
            _bookRepository = bookRepository;
            this.authService = authService;
            this.mapper = mapper;
            this.logger = logger;
        }

        [HttpGet("")]
        public async Task<IActionResult> GetAllBooks()
        {
            try
            {
                var books = await _bookRepository.GetAllBookAsync();
                return Ok(books);
            }
            catch (Exception error)
            {
                logger.LogError(error.Message);
                return StatusCode(500);
            }
            
        }

        [HttpGet("search")]
        public async Task<IActionResult> SearchBooks([FromQuery] string searchTerm)
        {
            try
            {
                var books = await _bookRepository.SearchBooksAsync(searchTerm);

                if (books != null)
                {
                    return Ok(books);
                }

                return NotFound();
            }
            catch (Exception error)
            {
                logger.LogError(error.Message);
                return StatusCode(500);
            }
            
        }

        [HttpGet("FindBook/{id}")]
        public async Task<IActionResult> GetBookById([FromRoute] int id)
        {
            try
            {
                var book = await _bookRepository.GetBookByIdAsync(id);

                if (book != null)
                {
                    return Ok(book);
                }

                return NotFound();
            }
            catch (Exception error)
            {
                logger.LogError(error.Message);
                return StatusCode(500);
            }

        }
        [HttpPost("AddBook")]
        [Authorize(Roles="Administrator")]
        public async Task<IActionResult> AddNewBook([FromBody] BookModel bookModel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    
                    var Id = await _bookRepository.AddBookAsync(bookModel);
                    return CreatedAtAction(nameof(GetBookById), new { id = Id, controller = "books" }, $"Book Added Successfully.\nYour Book Id Is : {Id}");

                }

                return BadRequest(ModelState);
            }
            catch (Exception error)
            {
                logger.LogError(error.Message);
                return StatusCode(500);
            }
            
    }
    [HttpPut("UpdateBook/{id}")]
      
        public async Task<IActionResult> UpdateBook([FromBody] BookModel bookModel, [FromRoute] int id)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    await _bookRepository.UpdateBookAsync(id, bookModel);
                    return Ok("Book Updated Successfully....");
                }
                    
                return BadRequest(ModelState);
            }
            catch (Exception error)
            {
                logger.LogError(error.Message);
                return StatusCode(500);
            }
           
        }
        
        [HttpDelete("DeleteBook/{id}")]
        public async Task<IActionResult> DeleteBook([FromRoute] int id)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    await _bookRepository.DeleteBookAsync(id);
                    return Ok("Book Deleted Successfully....");
                }

                return BadRequest(ModelState);
            }
            catch (Exception error)
            {
                logger.LogError(error.Message);
                return StatusCode(500);
            }
        }

        //[HttpPost("add-to-cart/{id:int}")]
        //public async Task<IActionResult> AddToCart([FromRoute] int id)
        //{
        //    try
        //    {
               
        //        var book = await _bookRepository.GetBookByIdAsync(id);
        //        if (book == null)
        //        {
        //            return NotFound(); // Return NotFound if the book doesn't exist
        //        }

              

        //        return Ok("Book added to cart successfully.");
        //    }
        //    catch (Exception ex)
        //    {
               
        //        return BadRequest("Error occurred while adding the book to the cart: " + ex.Message);
        //    }
        //}

    }
}