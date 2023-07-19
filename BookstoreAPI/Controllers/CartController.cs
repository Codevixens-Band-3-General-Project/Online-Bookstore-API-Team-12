using AutoMapper;
using BookStore__Management_system.Data;
using BookStore__Management_system.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace BookStore__Management_system.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class CartController : ControllerBase
    {
        private readonly BookStoreContext _db;
        private readonly ILogger logger;

        public CartController(ILogger logger)
        {
            this.logger = logger;
        }
        
        [HttpPost("add-to-cart/{id:int}")]
        public async Task<ActionResult> AddToCart(int id, int quantity)
        {
            try
            {
                // Add a book to the shopping cart
                var book = await _db.Books.FindAsync(id);

                if (book == null)
                {
                    return NotFound();
                }


                //Checks if a book already exists in cart
                var existingCartItem = await _db.CartItems.SingleOrDefaultAsync(c => c.CartId == id && c.BookId == id);

                if (existingCartItem != null)
                {
                    //If book already exists update quantity
                    existingCartItem.Quantity += quantity;

                    await _db.SaveChangesAsync();
                    return Ok($"Quantity of Book-ID {id}, Title -{book.BookTitle} has been updated to {existingCartItem.Quantity} in your cart");
                }
                else
                {
                    var cartItem = new CartItem
                    {
                        CartId = id,
                        BookId = id,
                        Quantity = quantity,

                    };

                    // Save the cart item to the database
                    _db.CartItems.Add(cartItem);
                    await _db.SaveChangesAsync();
                    return Ok($"Book-ID {id} Title-{book.BookTitle} (Quantity: {quantity}) has been added to cart");
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Failed to add the book with ID: {id} to the cart.");
                return StatusCode(StatusCodes.Status500InternalServerError, $"Failed to add the book with ID: {id} to the cart. Please try again later.");
            }
        }

        // POST: /book/delete-from-cart/{id}
        [HttpPost("delete-from-cart/{id:int}")]
        
        public async Task<ActionResult> DeleteFromCart(int id, int quantity = 1)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                // Retrieve the cart items for the user
                var cartItems = await _db.CartItems
                    .Where(c => c.UserId == userId && c.BookId == id)
                    .ToListAsync();

                if (cartItems.Count == 0)
                {
                    return NotFound($"Book-ID {id} not in cart");
                }

                // Remove the specified quantity from the cart items
                foreach (var cartItem in cartItems)
                {
                    if (cartItem.Quantity <= quantity)
                    {
                        _db.CartItems.Remove(cartItem);
                    }
                    else
                    {
                        cartItem.Quantity -= quantity;
                        _db.CartItems.Update(cartItem);
                    }
                }

                await _db.SaveChangesAsync();

                return Ok($"Book-ID {id} has been removed from cart");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Failed to delete the book with ID: {id} to the cart.");
                return StatusCode(StatusCodes.Status500InternalServerError, $"Failed to delete the book with ID: {id} from the cart. Please try again later.");
            }

        }

        // GET: /book/view-cart
        [HttpGet("view-cart")]
        
        public ActionResult<IEnumerable<Cart>> ViewCart()
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                var cartItems = _db.CartItems
                    .Where(c => c.UserId == userId)
                    .Include(c => c.Book)
                    .ToList();

                var cartItemViews = cartItems.Select(c => new Cart
                {
                    CartId = c.CartId,
                    Quantity = c.Quantity,
                    BookTitle = c.Book.BookTitle,
                    BookAuthor = c.Book.BookAuthor,
                    price = c.Book.Price
                }).ToList();

                float total = cartItems.Sum(c => c.Quantity * c.Book.Price);

                var cartView = new CartView
                {
                    Items = cartItemViews,
                    Total = total
                };


                return Ok(cartView);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to view the cart.");
                return StatusCode(StatusCodes.Status500InternalServerError, "Failed to view the cart. Please try again later.");
            }
        }
    }
}







