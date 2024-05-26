using UserService.Models;
using UserService.Interfaces;
using UserService.Data;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace UserService.Managers
{
    public class AddressManager : IAddressManager
    {
        private readonly MyDbContext _dbContext; 

        public AddressManager(MyDbContext myDbContext)
        {
            _dbContext = myDbContext;
        }

        public async Task<IEnumerable<Address>> GetAllAddressesAsync()
        {
            return await _dbContext.Addresses.ToListAsync();
        }

        public async Task<Address> GetAddressByIdAsync(int id)
        {
            return await _dbContext.Addresses.FindAsync(id);
        }

        public async Task<bool> UpdateAddressAsync(Address address)
        {
            _dbContext.Entry(address).State = EntityState.Modified;
            await _dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<Address> CreateAddressAsync(Address address)
        {
            _dbContext.Addresses.Add(address);
            await _dbContext.SaveChangesAsync();
            return address;
        }

        public async Task<bool> DeleteAddressAsync(int id)
        {
            // Find adressen baseret på ID'en
            var address = await _dbContext.Addresses.FindAsync(id);

            // Hvis adressen ikke findes, returneres false
            if (address == null)
            {
                return false;
            }

            // Fjern adressen fra DbSet
            _dbContext.Addresses.Remove(address);

            // Gem ændringerne i databasen
            await _dbContext.SaveChangesAsync();

            // Returner true for at indikere, at adressen blev slettet korrekt
            return true;
        }

        public async Task<bool> AddressExistsAsync(int addressId)
        {
            return await _dbContext.Addresses.AnyAsync(e => e.AddressId == addressId);
        }
    }
}
