using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace BookStore__Management_system.Data
{
    public class CartItem
    {
        public int CartId { get; set; }
        [Required]
        [ForeignKey(nameof(Books.Id))]
        public int BookId { get; set; }

        [ForeignKey(nameof(User.UserId))]
        public string UserId { get; set; }
        public int Quantity { get; set; }

        public Books Book { get; set; }
        public System.DateTime DateCreated { get; set; }

        public float price { get; set; }

        public float subtotal { get; set; }
    }
}
