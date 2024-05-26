using DLS_Catalog_Service.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DLS_Catalog_Service.Repositories
{
    public interface ICatalogRepository
    {
        Task<IEnumerable<Catalog>> GetAllCatalogsAsync();
        Task<Catalog> GetCatalogByIdAsync(int id);
        Task<Catalog> CreateCatalogAsync(Catalog catalog);
        Task<bool> UpdateCatalogAsync(int id, Catalog catalog);
        Task<bool> DeleteCatalogAsync(int id);
    }
}