using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace BookStore__Management_system.Data
{
    public class Cart
    {

        public int   CartId { get; set; }
        [Required]
        [ForeignKey(nameof(Books.Id))]
        public int BookId { get; set; }
        public string BookTitle { get; set; }
        public string BookAuthor { get; set; }
        public int Quantity { get; set; }

        public System.DateTime DateCreated { get; set; }

        public float price { get; set; }
        public float subtotal { get; set; }
    }

    public class CartView
    {
        [Key]
        public int ViewId { get; set; }
        public List<Cart> Items { get; set; }
        [Precision(18, 2)]
        public float Total { get; set; }
    }

}
