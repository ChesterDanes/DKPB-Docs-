using BLL.DTOModels.RequestsDTO;
using BLL.DTOModels.ResponseDTO;
using BLL.ServiceInterfaces.Interfaces;
using DAL;
using Microsoft.EntityFrameworkCore;
using Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL_EF
{
    public class ProductGroupService : IProductGroupService
    {
        private readonly WebstoreContext _context;

        public ProductGroupService(WebstoreContext context)
        {
            _context = context;
        }

        // Metoda do pobierania grup produktów z możliwością filtrowania po parentId oraz sortowania
        public async Task<IEnumerable<ProductGroupResponseDTO>> GetProductGroupsAsync(
            bool topGroups = false,
            int? groupId = null,
            string sortBy = "Name",
            bool ascending = true)
        {
            var query = _context.ProductGroups.AsQueryable();

            string path = "";

            if (topGroups)
            {
                query = query.Where(pg => pg.ParentId == null);
            }

            if (groupId.HasValue)
            {
                
                var group = await query.Where(pg => pg.ID==groupId).Select(pg => new ProductGroupResponseDTO
                (
                    pg.ID,
                    pg.Name,
                    pg.ParentId,
                    path
                )).ToListAsync();

                path = group.First().Name;

                while (group.First().ParentId!=null)
                {
                    group = await query.Where(pg => pg.ID == group.First().ParentId).Select(pg => new ProductGroupResponseDTO
                        (
                            pg.ID,
                            pg.Name,
                            pg.ParentId,
                            path
                        )).ToListAsync();
                    path = group.First().Name+"/"+path;
                }

                query = query.Where(pg => pg.ID == groupId);
            }

            if (ascending)
            {
                query = sortBy switch
                {
                    "Name" => query.OrderBy(pg => pg.Name),
                    "ID" => query.OrderBy(pg => pg.ID),
                    _ => query.OrderBy(pg => pg.Name),
                };
            }
            else
            {
                query = sortBy switch
                {
                    "Name" => query.OrderByDescending(pg => pg.Name),
                    "ID" => query.OrderByDescending(pg => pg.ID),
                    _ => query.OrderByDescending(pg => pg.Name),
                };
            }

            // Pobieranie danych i mapowanie na DTO
            var groups = await query
                .Select(pg => new ProductGroupResponseDTO
                (
                    pg.ID,
                    pg.Name,
                    pg.ParentId,
                    path
                ))
                .ToListAsync();

            return groups;
        }

        // Metoda do dodawania nowej grupy produktów
        public async Task<ProductGroupResponseDTO> AddProductGroupAsync(ProductGroupRequestDTO groupRequest)
        {
            var newGroup = new ProductGroup
            {
                Name = groupRequest.Name,
                ParentId = groupRequest.ParentId
            };

            _context.ProductGroups.Add(newGroup);
            await _context.SaveChangesAsync();

            return new ProductGroupResponseDTO
            (
                newGroup.ID,
                newGroup.Name,
                newGroup.ParentId,
                null
            );
                
        }
    }
}
