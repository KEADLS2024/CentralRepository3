using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.DataAnnotations;

namespace DLS_Catalog_Service.Model
{
    /// <summary>
    /// Represents a product in the catalog.
    /// </summary>
    public class Product
    {
        /// <summary>
        /// Gets or sets the identifier of the product.
        /// </summary>
        [BsonId]
        [BsonRepresentation(BsonType.Int32)]
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the name of the product.
        /// </summary>
        [BsonElement("name")]
        [Required(ErrorMessage = "Product name is required.")]
        [StringLength(150, ErrorMessage = "Product name cannot exceed 150 characters.")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the identifier of the category to which the product belongs.
        /// </summary>
        [BsonElement("categoryId")]
        [Required(ErrorMessage = "Category ID is required.")]
        public int CategoryId { get; set; }

        /// <summary>
        /// Gets or sets the price of the product.
        /// </summary>
        [BsonElement("price")]
        [Range(0.01, 10000.00, ErrorMessage = "Price must be between 0.01 and 10,000.00.")]
        public double Price { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the product is available.
        /// </summary>
        [BsonElement("available")]
        public bool Available { get; set; }
    }
}