using Microsoft.VisualStudio.TestTools.UnitTesting;
using DLS_Catalog_Service.Model;
using DLS_Catalog_Service.Tests.UnitTest_Model;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace DLS_Catalog_Service.Tests.UnitTest_Model
{
    [TestClass]
    public class CategoryTests
    {
        private Category CreateCategory(string name, int catalogId, bool isDeleted = false)
        {
            return new Category
            {
                Name = name,
                CatalogId = catalogId,
                IsDeleted = isDeleted
            };
        }

        private bool TryValidateModel(Category category, out ICollection<ValidationResult> results)
        {
            var context = new ValidationContext(category, serviceProvider: null, items: null);
            results = new List<ValidationResult>();
            return Validator.TryValidateObject(category, context, results, true);
        }

        // Positive Test Cases for Name
        [TestMethod]
        [DataRow("Electronics")]
        [DataRow("Books")]
        [DataRow("Aa")] // Minimum Length (2 characters)
        [DataRow("Category_1-2")] // Name with valid characters (letters, numbers, spaces, hyphens, underscores)
        [DataRow("aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa")] // 100 'a's Maximum
        [DataRow("aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa")] // 99 'a's just below
        [DataRow("aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa")] // 50 'a's middle of range
        public void Category_WithValidName_ShouldPassValidation(string name)
        {
            var category = CreateCategory(name, 1);
            var result = TryValidateModel(category, out var validationResults);
            Assert.IsTrue(result);
            Assert.AreEqual(0, validationResults.Count);
        }

        // Negative Test Cases for Name
        [TestMethod]
        [DataRow("")]
        [DataRow(" ")]
        [DataRow(null)]
        // [DataRow("A")] // Just Below Minimum (1 character)
        [DataRow("Invalid*Name")] // Invalid Characters
        [DataRow("aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa")] // 101 'a's Just Over Maximum (101 characters)
        [DataRow("Category@123")] // Special characters
        [DataRow("   ")] // Whitespaces
        public void Category_WithInvalidName_ShouldFailValidation(string name)
        {
            var category = CreateCategory(name, 1);
            var result = TryValidateModel(category, out var validationResults);
            Assert.IsFalse(result);
            Assert.IsTrue(validationResults.Count > 0);
        }
    }
}