using BLL.DTOModels.ResponseDTO;
using BLL.ServiceInterfaces.Interfaces;
using DAL;
using Microsoft.EntityFrameworkCore;
using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL_EF
{
    public class OrderService:IOrderService
    {
        private readonly WebstoreContext _context;

        public OrderService(WebstoreContext context)
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
                if (int.TryParse(orderIdFilter, out var orderId))
                {
                    query = query.Where(o => o.ID == orderId);
                }
            }

            // Filtracja po statusie płatności
            if (isPaidFilter.HasValue)
            {
                query = query.Where(o => o.IsPaid == isPaidFilter.Value);
            }

            // Sortowanie
            if (ascending)
            {
                query = sortBy switch
                {
                    "OrderDate" => query.OrderBy(o => o.Date),
                    "TotalAmount" => query.OrderBy(o => o.OrderPositions.Sum(op => op.Price)),
                    _ => query.OrderBy(o => o.ID),
                };
            }
            else
            {
                query = sortBy switch
                {
                    "OrderDate" => query.OrderByDescending(o => o.Date),
                    "TotalAmount" => query.OrderByDescending(o => o.OrderPositions.Sum(op => op.Price)),
                    _ => query.OrderByDescending(o => o.ID),
                };
            }

            // Asynchroniczne pobieranie danych
            var orders = await query
                .Select(o => new OrderResponseDTO
                (
                    o.ID,
                    ((decimal)o.OrderPositions.Sum(op => op.Price*op.Amount)), // Upewnij się, że wynik nie będzie null
                    o.IsPaid,
                    o.Date
                ))
                .ToListAsync();

            return orders;
        }

        public async Task<OrderPositionResponseDTO> GetOrderPositionAsync(int orderId)
        {
            var orderPosition = await _context.OrderPositions
                .Where(op => op.OrderID == orderId)
                .Select(op => new OrderPositionResponseDTO
                (
                    op.Product.Name,
                    op.Price,
                    op.Amount,
                    op.Amount * op.Price
                ))
                .ToListAsync();

            if (orderPosition == null || !orderPosition.Any())
            {
                return null;
            }

            return orderPosition.First();
        }

        public async Task<OrderResponseDTO> GenerateOrderAsync(int userId)
        {
            var user = await _context.Users.FindAsync(userId);
            var basketItems = await _context.BasketPositions
                .Where(bp => bp.UserID == userId)
                .Include(bp => bp.Product)
                .ToListAsync();

            if (user == null || basketItems.Count == 0)
            {
                return null; // Brak użytkownika lub produktów w koszyku
            }

            var newOrder = new Order
            {
                UserID = userId,
                Date = DateTime.Now,
                IsPaid = false
            };

            _context.Orders.Add(newOrder);
            await _context.SaveChangesAsync();

            foreach (var item in basketItems)
            {
                var orderPosition = new OrderPosition
                {
                    OrderID = newOrder.ID,
                    ProductID = item.ProductID,
                    Amount = item.Amount,
                    Price = item.Product.Price
                };

                _context.OrderPositions.Add(orderPosition);
            }

            await _context.SaveChangesAsync();

            // Usuwamy pozycje z koszyka
            _context.BasketPositions.RemoveRange(basketItems);
            await _context.SaveChangesAsync();

            return new OrderResponseDTO
            (
                newOrder.ID,
                newOrder.OrderPositions.Sum(op => op.Amount),
                newOrder.IsPaid,
                newOrder.Date
            );
        }
        public async Task PayOrderAsync(int orderId, decimal amountPaid)
        {
            var order = await _context.Orders.FindAsync(orderId);
            var orderPosition = await _context.OrderPositions
                .Where(op => op.OrderID == orderId)
                .Select(op => new OrderPositionResponseDTO
                (
                    op.Product.Name,
                    op.Price,
                    op.Amount,
                    op.Amount * op.Price
                ))
                .ToListAsync();

            System.Diagnostics.Trace.WriteLine(orderPosition.Sum(op => (decimal)op.Price * op.Amount));
            //System.Diagnostics.Trace.WriteLine(order.OrderPositions.First().Amount);

            if (order != null && amountPaid >= orderPosition.Sum(op => (decimal)op.Price*op.Amount))
            {
                order.IsPaid = true;
                
                await _context.SaveChangesAsync();
            }
        }
    }
}
