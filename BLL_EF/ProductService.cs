using BLL.DTOModels.ResponseDTO;
using BLL.DTOModels;
using BLL.ServiceInterfaces.Interfaces;
using DAL;
using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace BLL_EF
{
    public class ProductService : IProductService
    {
        private readonly WebstoreContext _context;
        public ProductService(WebstoreContext context)
        {
            _context = context;
        }

        public async Task<List<ProductResponseDTO>> GetProductsAsync(
            string? nameFilter = null,
            string? groupNameFilter = null,
            int? groupIdFilter = null,
            bool includeInactive = false,
            string sortBy = "Name",
            bool ascending = true)
        {
            var query = _context.Products
                .AsNoTracking()
                .Include(p => p.ProductGroup)
                .AsQueryable();

            if (!string.IsNullOrEmpty(nameFilter))
                query = query.Where(p => p.Name.Contains(nameFilter));

            if (!string.IsNullOrEmpty(groupNameFilter))
                query = query.Where(p => p.ProductGroup.Name.Contains(groupNameFilter));

            if (groupIdFilter.HasValue)
                query = query.Where(p => p.GroupID == groupIdFilter.Value);

            if (!includeInactive)
                query = query.Where(p => p.IsActive);

            query = sortBy switch
            {
                "Name" => ascending ? query.OrderBy(p => p.Name) : query.OrderByDescending(p => p.Name),
                "Price" => ascending ? query.OrderBy(p => p.Price) : query.OrderByDescending(p => p.Price),
                "GroupName" => ascending ? query.OrderBy(p => p.ProductGroup.Name) : query.OrderByDescending(p => p.ProductGroup.Name),
                _ => query.OrderBy(p => p.Name)
            };

            var products = await query.ToListAsync();

            return products.Select(p => new ProductResponseDTO(
                p.ID,
                p.Name,
                p.Price,
                p.Image,
                p.IsActive,
                p.ProductGroup.Name
            )).ToList();
        }

        public async Task AddProductAsync(ProductRequestDTO productRequest)
        {
            var newProduct = new Product
            {
                Name = productRequest.Name,
                Price = productRequest.Price,
                Image = productRequest.Image,
                IsActive = true,
                GroupID = productRequest.GroupID,
            };

            _context.Products.Add(newProduct);
            await _context.SaveChangesAsync();
        }

        public async Task DeactivateProductAsync(int productId)
        {
            var product = await _context.Products.FindAsync(productId);
            if (product != null)
            {
                product.IsActive = false;
                await _context.SaveChangesAsync();
            }
        }

        public async Task ActivateProductAsync(int productId)
        {
            var product = await _context.Products.FindAsync(productId);
            if (product != null)
            {
                product.IsActive = true;
                await _context.SaveChangesAsync();
            }
        }

        public async Task DeleteProductAsync(int productId)
        {
            var product = await _context.Products.FindAsync(productId);
            if (product != null)
            {
                _context.Products.Remove(product);
                await _context.SaveChangesAsync();
            }
        }

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
            var basketPosition = await _context.BasketPositions
                .FirstOrDefaultAsync(bp => bp.ProductID == productId && bp.UserID == userId);

            if (basketPosition != null)
            {
                _context.BasketPositions.Remove(basketPosition);
                await _context.SaveChangesAsync();
            }
        }
    }
}
