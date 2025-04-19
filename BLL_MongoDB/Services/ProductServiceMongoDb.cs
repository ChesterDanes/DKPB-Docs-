using BLL.ServiceInterfaces.Interfaces;
using BLL.DTOModels.RequestsDTO;
using BLL.DTOModels.ResponseDTO;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BLL.DTOModels;
using BLL_MongoDb.Context;

public class ProductServiceMongoDb : IProductService
{
    private readonly IMongoCollection<ProductDocument> _products;

    public ProductServiceMongoDb(MongoDbContext context)
    {
        _products = context.Products;
    }

    public async Task<List<ProductResponseDTO>> GetProductsAsync(string nameFilter = null, string groupNameFilter = null, int? groupIdFilter = null, bool includeInactive = false, string sortBy = "Name", bool ascending = true)
    {
        var filterBuilder = Builders<ProductDocument>.Filter;
        var filters = new List<FilterDefinition<ProductDocument>>();

        if (!includeInactive)
            filters.Add(filterBuilder.Eq(p => p.IsActive, true));

        if (!string.IsNullOrEmpty(nameFilter))
            filters.Add(filterBuilder.Regex(p => p.Name, new MongoDB.Bson.BsonRegularExpression(nameFilter, "i")));

        if (!string.IsNullOrEmpty(groupNameFilter))
            filters.Add(filterBuilder.Eq(p => p.GroupName, groupNameFilter));

        if (groupIdFilter.HasValue)
            filters.Add(filterBuilder.Eq(p => p.GroupId, groupIdFilter));

        var filter = filters.Any() ? filterBuilder.And(filters) : filterBuilder.Empty;

        var sortDefinition = ascending
            ? Builders<ProductDocument>.Sort.Ascending(sortBy)
            : Builders<ProductDocument>.Sort.Descending(sortBy);

        var products = await _products.Find(filter).Sort(sortDefinition).ToListAsync();

        return products.Select(p => new ProductResponseDTO(
            id: int.Parse(p.Id.GetHashCode().ToString()), // UWAGA: Mongo Id jest stringiem. Musimy jakoś zamienić - tu HashCode jako szybkie rozwiązanie
            name: p.Name,
            price: p.Price,
            image: p.Image,
            isActive: p.IsActive,
            groupName: p.GroupName
        )).ToList();
    }

    public async Task AddProductAsync(ProductRequestDTO productRequest)
    {
        var product = new ProductDocument
        {
            Id = MongoDB.Bson.ObjectId.GenerateNewId().ToString(),
            Name = productRequest.Name,
            Price = productRequest.Price,
            Image = productRequest.Image,
            IsActive = productRequest.IsActive,
            GroupId = productRequest.GroupID,
            GroupName = null // Tu trzeba będzie ustawić, jeśli masz osobną kolekcję Grup
        };

        await _products.InsertOneAsync(product);
    }

    public async Task DeactivateProductAsync(int productId)
    {
        var filter = Builders<ProductDocument>.Filter.Where(p => p.Id.GetHashCode() == productId);
        var update = Builders<ProductDocument>.Update.Set(p => p.IsActive, false);
        await _products.UpdateOneAsync(filter, update);
    }

    public async Task ActivateProductAsync(int productId)
    {
        var filter = Builders<ProductDocument>.Filter.Where(p => p.Id.GetHashCode() == productId);
        var update = Builders<ProductDocument>.Update.Set(p => p.IsActive, true);
        await _products.UpdateOneAsync(filter, update);
    }

    public async Task DeleteProductAsync(int productId)
    {
        var filter = Builders<ProductDocument>.Filter.Where(p => p.Id.GetHashCode() == productId);
        await _products.DeleteOneAsync(filter);
    }

    public async Task AddToCartAsync(int productId, int userId, int amount)
    {
        var filter = Builders<ProductDocument>.Filter.Where(p => p.Id.GetHashCode() == productId);
        var update = Builders<ProductDocument>.Update.Push(p => p.CartItems, new CartItemDocument
        {
            UserId = userId,
            Amount = amount
        });

        await _products.UpdateOneAsync(filter, update);
    }

    public async Task UpdateProductAmountInCartAsync(int productId, int userId, int amount)
    {
        var filter = Builders<ProductDocument>.Filter.And(
            Builders<ProductDocument>.Filter.Where(p => p.Id.GetHashCode() == productId),
            Builders<ProductDocument>.Filter.ElemMatch(p => p.CartItems, ci => ci.UserId == userId)
        );

        var update = Builders<ProductDocument>.Update.Set(p => p.CartItems[-1].Amount, amount);

        await _products.UpdateOneAsync(filter, update);
    }

    public async Task RemoveFromCartAsync(int productId, int userId)
    {
        var filter = Builders<ProductDocument>.Filter.Where(p => p.Id.GetHashCode() == productId);
        var update = Builders<ProductDocument>.Update.PullFilter(p => p.CartItems, ci => ci.UserId == userId);

        await _products.UpdateOneAsync(filter, update);
    }
}
