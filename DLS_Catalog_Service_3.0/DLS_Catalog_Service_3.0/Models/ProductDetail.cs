using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.DataAnnotations;

namespace DLS_Catalog_Service.Model
{
    /// <summary>
    /// Represents detailed information about a product.
    /// </summary>
    public class ProductDetail
    {
        /// <summary>
        /// Gets or sets the identifier of the product detail.
        /// </summary>
        [BsonId]
        [BsonRepresentation(BsonType.Int32)]
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the identifier of the product to which the detail belongs.
        /// </summary>
        [BsonElement("productId")]
        [Required(ErrorMessage = "Product ID is required.")]
        public int ProductId { get; set; }

        /// <summary>
        /// Gets or sets the description of the product.
        /// </summary>
        [BsonElement("description")]
        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters.")]
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the weight of the product.
        /// </summary>
        [BsonElement("weight")]
        [Range(0.1, 1000.0, ErrorMessage = "Weight must be between 0.1 and 1000.0 kilograms.")]
        public double Weight { get; set; }

        /// <summary>
        /// Gets or sets the dimensions of the product.
        /// </summary>
        [BsonElement("dimensions")]
        [StringLength(200, ErrorMessage = "Dimensions cannot exceed 200 characters.")]
        public string Dimensions { get; set; }
    }
}