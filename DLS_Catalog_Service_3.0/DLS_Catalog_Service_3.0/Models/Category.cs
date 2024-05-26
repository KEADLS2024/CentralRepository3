using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace DLS_Catalog_Service.Model
{
    /// <summary>
    /// Represents a category in the catalog.
    /// </summary>
    public class Category
    {
        /// <summary>
        /// Gets or sets the identifier of the category.
        /// </summary>
        [BsonId]
        [BsonRepresentation(BsonType.Int32)]
        public int Id { get; set; }

        private const string ValidNamePattern = @"^[a-zA-Z0-9\s\-_]*$";

        /// <summary>
        /// Gets or sets the name of the category.
        /// </summary>
        [BsonElement("name")]
        [Required(ErrorMessage = "Category name is required.")]
        [StringLength(100, ErrorMessage = "Category name cannot exceed 100 characters.")]
        [RegularExpression(ValidNamePattern, ErrorMessage = "Category name contains invalid characters.")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the identifier of the catalog to which the category belongs.
        /// </summary>
        [BsonElement("catalogId")]
        [Required(ErrorMessage = "Catalog ID is required.")]
        public int CatalogId { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the category is deleted.
        /// </summary>
        [BsonElement("isDeleted")]
        public bool IsDeleted { get; set; } = false;  // Tombstone flag
    }
}