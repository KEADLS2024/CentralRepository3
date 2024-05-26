using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.DataAnnotations;

namespace DLS_Catalog_Service.Model
{
    /// <summary>
    /// Represents a catalog.
    /// </summary>
    public class Catalog
    {
        /// <summary>
        /// Gets or sets the identifier of the catalog.
        /// </summary>
        [BsonId]
        [BsonRepresentation(BsonType.Int32)]
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the name of the catalog.
        /// </summary>
        [BsonElement("name")]
        [Required(ErrorMessage = "Catalog name is required.")]
        [StringLength(100, ErrorMessage = "Catalog name cannot exceed 100 characters.")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the year of the catalog.
        /// </summary>
        [BsonElement("year")]
        [Range(1900, 9999, ErrorMessage = "Year must be between 1900 and 9999.")]
        public int Year { get; set; }
    }
}