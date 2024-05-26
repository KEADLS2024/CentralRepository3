using DLS_Catalog_Service.Model;

namespace DLS_Catalog_Service.Repositories;

public interface IProductDetailRepository
{
    Task<IEnumerable<ProductDetail>> GetAllAsync();
    Task<ProductDetail> GetByIdAsync(int id);
    Task<ProductDetail> CreateAsync(ProductDetail productDetail);
    Task<bool> UpdateAsync(int id, ProductDetail productDetail);
    Task<bool> DeleteAsync(int id);
}