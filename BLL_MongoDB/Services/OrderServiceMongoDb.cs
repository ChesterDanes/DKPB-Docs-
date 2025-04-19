using BLL.DTOModels.ResponseDTO;
using BLL.ServiceInterfaces.Interfaces;
using BLL_MongoDb.Context;
using BLL_MongoDb.Documents;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BLL_MongoDb.Services
{
    public class OrderServiceMongoDb : IOrderService
    {
        private readonly MongoDbContext _context;

        public OrderServiceMongoDb(MongoDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<OrderResponseDTO>> GetOrdersAsync(string orderIdFilter = null, bool? isPaidFilter = null, string sortBy = "ID", bool ascending = true)
        {
            var filterBuilder = Builders<OrderDocument>.Filter;
            var filters = new List<FilterDefinition<OrderDocument>>();

            if (!string.IsNullOrEmpty(orderIdFilter) && int.TryParse(orderIdFilter, out int parsedOrderId))
            {
                filters.Add(filterBuilder.Eq(o => o.ID, parsedOrderId));
            }

            if (isPaidFilter.HasValue)
            {
                filters.Add(filterBuilder.Eq(o => o.IsPaid, isPaidFilter.Value));
            }

            var combinedFilter = filters.Any() ? filterBuilder.And(filters) : filterBuilder.Empty;

            var orders = await _context.Orders.Find(combinedFilter).ToListAsync();

            // Sortowanie
            orders = sortBy.ToLower() switch
            {
                "value" => ascending ? orders.OrderBy(o => o.Value).ToList() : orders.OrderByDescending(o => o.Value).ToList(),
                "date" => ascending ? orders.OrderBy(o => o.Date).ToList() : orders.OrderByDescending(o => o.Date).ToList(),
                _ => ascending ? orders.OrderBy(o => o.ID).ToList() : orders.OrderByDescending(o => o.ID).ToList(),
            };

            return orders.Select(o => new OrderResponseDTO(
                o.ID,
                o.Value,
                o.IsPaid,
                o.Date
            ));
        }

        public async Task<OrderPositionResponseDTO> GetOrderPositionAsync(int orderId)
        {
            var order = await _context.Orders.Find(o => o.ID == orderId).FirstOrDefaultAsync();
            if (order == null || order.Positions == null || !order.Positions.Any())
                return null;

            // Dla uproszczenia zwracamy pierwszą pozycję zamówienia
            var position = order.Positions.First();

            return new OrderPositionResponseDTO(
                position.ProductName,
                position.Price,
                position.Amount,
                position.Value
            );
        }

        public async Task<OrderResponseDTO> GenerateOrderAsync(int userId)
        {
            var lastOrder = await _context.Orders.Find(_ => true)
                .SortByDescending(o => o.ID)
                .FirstOrDefaultAsync();

            int newId = (lastOrder?.ID ?? 0) + 1;

            var newOrder = new OrderDocument
            {
                ID = newId,
                UserID = userId,
                Value = 0,  // Na początek puste zamówienie
                IsPaid = false,
                Date = DateTime.UtcNow,
                Positions = new List<OrderPositionDocument>()
            };

            await _context.Orders.InsertOneAsync(newOrder);

            return new OrderResponseDTO(
                newOrder.ID,
                newOrder.Value,
                newOrder.IsPaid,
                newOrder.Date
            );
        }

        public async Task<bool> PayOrderAsync(int orderId, decimal amountPaid)
        {
            var order = await _context.Orders.Find(o => o.ID == orderId).FirstOrDefaultAsync();

            if (order == null || order.IsPaid)
                return false;

            var update = Builders<OrderDocument>.Update
                .Set(o => o.IsPaid, true)
                .Set(o => o.Value, amountPaid);

            var result = await _context.Orders.UpdateOneAsync(o => o.ID == orderId, update);

            return result.ModifiedCount > 0;
        }
    }
}
