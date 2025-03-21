using BLL.DTOModels.ResponseDTO;
using BLL.DTOModels.RequestsDTO;
using DAL;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BLL.ServiceInterfaces.Interfaces;
using Model;

namespace BLL.Services
{
    public class OrderServiceDB : IOrderService
    {
        private readonly WebstoreContext _context;

        public OrderServiceDB(WebstoreContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<OrderResponseDTO>> GetOrdersAsync(
            string orderIdFilter = null,
            bool? isPaidFilter = null,
            string sortBy = "ID",
            bool ascending = true)
        {
            var query = _context.Orders.AsQueryable();

            // Filtracja po ID zamówienia
            if (!string.IsNullOrEmpty(orderIdFilter))
            {
                int orderId = int.Parse(orderIdFilter);
                query = query.Where(o => o.ID == orderId);
            }

            // Filtracja po statusie płatności
            if (isPaidFilter.HasValue)
            {
                query = query.Where(o => o.IsPaid == isPaidFilter.Value);
            }

            // Sortowanie
            if (sortBy == "ID")
            {
                query = ascending ? query.OrderBy(o => o.ID) : query.OrderByDescending(o => o.ID);
            }
            else if (sortBy == "Value")
            {
                query = ascending ? query.OrderBy(o => o.Value) : query.OrderByDescending(o => o.Value);
            }

            // Pobranie zamówień
            var orders = await query
                .Select(o => new OrderResponseDTO(o.ID, o.Value, o.IsPaid, o.Date))
                .ToListAsync();

            return orders;
        }

        public async Task<OrderPositionResponseDTO> GetOrderPositionAsync(int orderId)
        {
            // Pobranie pozycji zamówienia
            var orderPositions = await _context.OrderPositions
                .Where(op => op.OrderID == orderId)
                .Select(op => new OrderPositionResponseDTO(
                    op.Product.Name,
                    op.Product.Price,
                    op.Amount,
                    op.Amount * op.Product.Price))
                .ToListAsync();

            // Zakładając, że chcemy zwrócić pierwszą pozycję (jeśli jest)
            return orderPositions.FirstOrDefault();
        }

        public async Task<OrderResponseDTO> GenerateOrderAsync(int userId)
        {
            // Pobieramy produkty z koszyka użytkownika
            var basketItems = await _context.BasketPositions
                .Where(bp => bp.UserID == userId)
                .ToListAsync();

            if (basketItems.Count == 0)
            {
                throw new Exception("Koszyk jest pusty, nie można utworzyć zamówienia.");
            }

            // Pobieramy produkty na podstawie ProductID
            var productIds = basketItems.Select(bp => bp.ProductID).Distinct().ToList();
            var products = await _context.Products
                .Where(p => productIds.Contains(p.ID))
                .ToDictionaryAsync(p => p.ID, p => p.Price); // Słownik ID -> Price

            // Obliczenie wartości zamówienia
            decimal totalValue = basketItems.Sum(bp =>
            {
                // Pobieramy cenę produktu na podstawie ProductID
                if (products.TryGetValue(bp.ProductID, out var price))
                {
                    return bp.Amount * price;
                }
                return 0m; // Jeśli nie znaleziono produktu, traktujemy go jako 0
            });

            // Tworzenie nowego zamówienia
            var newOrder = new Order
            {
                UserID = userId,
                Date = DateTime.Now,
                Value = totalValue,
                IsPaid = false
            };

            _context.Orders.Add(newOrder);
            await _context.SaveChangesAsync();

            // Dodanie pozycji zamówienia
            foreach (var item in basketItems)
            {
                var orderPosition = new OrderPosition
                {
                    OrderID = newOrder.ID,
                    ProductID = item.ProductID,
                    Amount = item.Amount
                };
                _context.OrderPositions.Add(orderPosition);
            }

            await _context.SaveChangesAsync();

            // Usunięcie produktów z koszyka po utworzeniu zamówienia
            _context.BasketPositions.RemoveRange(basketItems);
            await _context.SaveChangesAsync();

            // Zwrócenie zamówienia
            return new OrderResponseDTO(newOrder.ID, newOrder.Value, newOrder.IsPaid, newOrder.Date);
        }


        public async Task<bool> PayOrderAsync(int orderId, decimal amountPaid)
        {
            var order = await _context.Orders
                .FirstOrDefaultAsync(o => o.ID == orderId);

            if (order == null)
            {
                throw new Exception("Zamówienie nie istnieje.");
            }

            // Sprawdzamy, czy zapłacono pełną wartość
            if (amountPaid < order.Value)
            {
                throw new Exception("Wprowadzona kwota jest mniejsza od wartości zamówienia.");
            }

            // Aktualizacja statusu płatności
            order.IsPaid = true;
            await _context.SaveChangesAsync();

            return true;
        }
    }
}
