using BLL.DTOModels.RequestsDTO;
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
    public class ProductGroupServiceMongoDb : IProductGroupService
    {
        private readonly MongoDbContext _context;

        public ProductGroupServiceMongoDb(MongoDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ProductGroupResponseDTO>> GetProductGroupsAsync(bool topGroups = false, int? groupId = null, string sortBy = "Name", bool ascending = true)
        {
            var filterBuilder = Builders<ProductGroupDocument>.Filter;
            FilterDefinition<ProductGroupDocument> filter = FilterDefinition<ProductGroupDocument>.Empty;

            if (topGroups)
                filter = filterBuilder.Eq(g => g.ParentId, null);
            else if (groupId.HasValue)
                filter = filterBuilder.Eq(g => g.ParentId, groupId);

            var groups = await _context.ProductGroups.Find(filter).ToListAsync();

            // Sortowanie
            groups = sortBy.ToLower() switch
            {
                "name" => ascending ? groups.OrderBy(g => g.Name).ToList() : groups.OrderByDescending(g => g.Name).ToList(),
                _ => groups
            };

            return groups.Select(g => new ProductGroupResponseDTO(
                g.ID,
                g.Name,
                g.ParentId,
                g.Path
            ));
        }

        public async Task<ProductGroupResponseDTO> AddProductGroupAsync(ProductGroupRequestDTO groupRequest)
        {
            var lastGroup = await _context.ProductGroups.Find(_ => true)
                .SortByDescending(g => g.ID)
                .FirstOrDefaultAsync();

            int newId = (lastGroup?.ID ?? 0) + 1;

            var newGroup = new ProductGroupDocument
            {
                ID = newId,
                Name = groupRequest.Name,
                ParentId = groupRequest.ParentId,
                Path = await GeneratePathAsync(groupRequest.Name, groupRequest.ParentId)
            };

            await _context.ProductGroups.InsertOneAsync(newGroup);

            return new ProductGroupResponseDTO(
                newGroup.ID,
                newGroup.Name,
                newGroup.ParentId,
                newGroup.Path
            );
        }

        private async Task<string> GeneratePathAsync(string groupName, int? parentId)
        {
            if (parentId == null)
            {
                return groupName;
            }
            else
            {
                var parentGroup = await _context.ProductGroups.Find(g => g.ID == parentId.Value).FirstOrDefaultAsync();
                return parentGroup != null ? $"{parentGroup.Path}/{groupName}" : groupName;
            }
        }
    }
}
