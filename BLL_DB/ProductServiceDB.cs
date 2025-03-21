using BLL.DTOModels.ResponseDTO;
using BLL.DTOModels.RequestsDTO;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BLL.DTOModels;
using BLL.ServiceInterfaces.Interfaces;
using DAL;
using Model;

namespace BLL_DB.Services
{
    public class ProductServiceDB : IProductService
    {
        private readonly WebstoreContext _context;

        // Konstruktor przyjmujący kontekst bazy danych
        public ProductServiceDB(WebstoreContext context)
        {
            _context = context;
        }

        // Pobieranie listy produktów
        public async Task<List<ProductResponseDTO>> GetProductsAsync(
            string nameFilter = null,
            string groupNameFilter = null,
            int? groupIdFilter = null,
            bool includeInactive = false,
            string sortBy = "Name",
            bool ascending = true)
        {
            var query = _context.Products.AsQueryable();

            // Filtracja produktów
            if (!string.IsNullOrEmpty(nameFilter))
                query = query.Where(p => p.Name.Contains(nameFilter));

            if (!string.IsNullOrEmpty(groupNameFilter))
                query = query.Where(p => p.ProductGroup.Name.Contains(groupNameFilter));

            if (groupIdFilter.HasValue)
                query = query.Where(p => p.GroupID == groupIdFilter.Value);

            if (!includeInactive)
                query = query.Where(p => p.IsActive);

            // Sortowanie produktów
            query = ascending ? query.OrderBy(p => EF.Property<object>(p, sortBy)) : query.OrderByDescending(p => EF.Property<object>(p, sortBy));

            // Pobranie wyników
            var products = await query.Select(p => new ProductResponseDTO(
                p.ID,
                p.Name,
                p.Price,
                p.Image,
                p.IsActive,
                p.ProductGroup.Name
            )).ToListAsync();

            return products;
        }

        // Dodanie produktu
        public async Task AddProductAsync(ProductRequestDTO productRequest)
        {
            var product = new Product
            {
                Name = productRequest.Name,
                Price = productRequest.Price,
                GroupID = productRequest.GroupID,
                IsActive = productRequest.IsActive,
                Image = productRequest.Image
            };

            _context.Products.Add(product);
            await _context.SaveChangesAsync();
        }

        // Dezaktywowanie produktu
        public async Task DeactivateProductAsync(int productId)
        {
            var product = await _context.Products.FindAsync(productId);
            if (product != null)
            {
                product.IsActive = false;
                await _context.SaveChangesAsync();
            }
        }

        // Aktywowanie produktu
        public async Task ActivateProductAsync(int productId)
        {
            var product = await _context.Products.FindAsync(productId);
            if (product != null)
            {
                product.IsActive = true;
                await _context.SaveChangesAsync();
            }
        }

        // Usunięcie produktu
        public async Task DeleteProductAsync(int productId)
        {
            var product = await _context.Products.FindAsync(productId);
            if (product != null)
            {
                _context.Products.Remove(product);
                await _context.SaveChangesAsync();
            }
        }

        // Dodanie produktu do koszyka
        public async Task AddToCartAsync(int productId, int userId, int amount)
        {
            var product = await _context.Products.FindAsync(productId);
            var user = await _context.Users.FindAsync(userId);

            if (product != null && user != null)
            {
                var basketPosition = new BasketPosition
                {
                    ProductID = productId,
                    UserID = userId,
                    Amount = amount
                };

                _context.BasketPositions.Add(basketPosition);
                await _context.SaveChangesAsync();
            }
        }

        // Aktualizacja ilości produktu w koszyku
        public async Task UpdateProductAmountInCartAsync(int productId, int userId, int amount)
        {
            var basketPosition = await _context.BasketPositions
                .FirstOrDefaultAsync(bp => bp.ProductID == productId && bp.UserID == userId);

            if (basketPosition != null)
            {
                basketPosition.Amount = amount;
                await _context.SaveChangesAsync();
            }
        }

        public async Task RemoveFromCartAsync(int productId, int userId)
        {
            try
            {
                // Sprawdzanie, czy produkt o podanym ID znajduje się w koszyku użytkownika
                var basketPosition = await _context.BasketPositions
                    .FirstOrDefaultAsync(bp => bp.ProductID == productId && bp.UserID == userId);

                if (basketPosition == null)
                {
                    // Logowanie, jeśli nie znaleziono produktu w koszyku
                    Console.WriteLine($"Produkt o ID {productId} nie został znaleziony w koszyku użytkownika o ID {userId}.");
                    return; // Można rzucić wyjątek, zwrócić odpowiedni kod lub po prostu zakończyć operację
                }

                // Usuwanie pozycji z koszyka
                _context.BasketPositions.Remove(basketPosition);
                await _context.SaveChangesAsync();

                // Logowanie usunięcia
                Console.WriteLine($"Produkt o ID {productId} został pomyślnie usunięty z koszyka użytkownika o ID {userId}.");
            }
            catch (DbUpdateException ex)
            {
                Console.WriteLine("DbUpdateException: " + ex.Message);
                Console.WriteLine("Inner Exception: " + ex.InnerException?.Message);
            }
        }
    }
}
