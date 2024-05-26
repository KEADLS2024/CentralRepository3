using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Net;

namespace OrderService.Models;

public class OrderTable
{
    [Key] // Marks 'OrderId' as the primary key.
    public int OrderId { get; set; }

    [Required(ErrorMessage = "Order date is required.")]
    public DateTime OrderDate { get; set; }

    [Required(ErrorMessage = "Total amount is required.")]
    [Column(TypeName = "decimal(18,2)")]
    [Range(0.01, 10000000.00, ErrorMessage = "Total amount must be between 0.01 and 10,000,000.00.")]
    public decimal TotalAmount { get; set; }

    [Required(ErrorMessage = "Customer ID is required.")]
    [Range(1, int.MaxValue, ErrorMessage = "Customer ID must be a positive number.")]
    [ForeignKey("Customer")]
    public int CustomerId { get; set; }

    [Required(ErrorMessage = "Delivery address ID is required.")]
    [Range(1, int.MaxValue, ErrorMessage = "Delivery address ID must be a positive number.")]
    [ForeignKey("DeliveryAddress")]
    public int DeliveryAddressId { get; set; }

    public DateTime? DeletedAt { get; set; }

    //public ICollection<OrderItem> OrderItems { get; set; } // An Order can have many OrderItems.

}