// seed.js
db = db.getSiblingDB('DLS');

db.Catalogs.insertMany([
    { Id: "1", Name: "Catalog1----REALSEED" },
    { Id: "2", Name: "Catalog2" },
    { Id: "3", Name: "Catalog3" }
]);

db.Categories.insertMany([
    { Id: "1", Name: "Category1" },
    { Id: "2", Name: "Category2" },
    { Id: "3", Name: "Category3" }
]);

db.Products.insertMany([
    { Id: "1", Name: "Product1", CategoryId: "1" },
    { Id: "2", Name: "Product2", CategoryId: "1" },
    { Id: "3", Name: "Product3", CategoryId: "2" },
    { Id: "4", Name: "Product4", CategoryId: "3" }
]);

db.ProductDetails.insertMany([
    { Id: "1", ProductId: "1", Description: "Detail1" },
    { Id: "2", ProductId: "2", Description: "Detail2" },
    { Id: "3", ProductId: "3", Description: "Detail3" },
    { Id: "4", ProductId: "4", Description: "Detail4" }
]);
