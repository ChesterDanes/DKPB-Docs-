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
            // Pobranie wszystkich grup z bazy do pamięci (bez wywoływania BuildFullPath w LINQ)
            var allGroups = await _context.ProductGroups.ToListAsync();

            // Słownik dla szybszego dostępu do grup
            var groupDict = allGroups.ToDictionary(pg => pg.ID);

            // Wybieranie odpowiednich grup
            var selectedGroups = allGroups.AsQueryable();

            if (topGroups)
            {
                selectedGroups = selectedGroups.Where(pg => pg.ParentId == null);
            }

            if (groupId.HasValue)
            {
                selectedGroups = selectedGroups.Where(pg => pg.ID == groupId || pg.ParentId == groupId);
            }

            // Zamiana na listę przed wywołaniem BuildFullPath (unikanie błędu CS8110)
            var selectedGroupsList = selectedGroups.ToList();

            // Mapowanie do DTO z pełną ścieżką
            var dtoList = selectedGroupsList.Select(pg => new ProductGroupResponseDTO
            (
                pg.ID,
                pg.Name,
                pg.ParentId,
                BuildFullPath(pg, groupDict) // Teraz działa poprawnie!
            )).ToList();

            // Sortowanie
            dtoList = sortBy switch
            {
                "Name" => ascending ? dtoList.OrderBy(dto => dto.Path).ToList() : dtoList.OrderByDescending(dto => dto.Path).ToList(),
                "ID" => ascending ? dtoList.OrderBy(dto => dto.ID).ToList() : dtoList.OrderByDescending(dto => dto.ID).ToList(),
                _ => dtoList.OrderBy(dto => dto.Path).ToList()
            };

            return dtoList;
        }

        // Przeniesiona metoda BuildFullPath
        private string BuildFullPath(ProductGroup group, Dictionary<int, ProductGroup> groupDict)
        {
            var pathSegments = new List<string>();
            while (group != null)
            {
                pathSegments.Insert(0, group.Name);
                group = group.ParentId.HasValue ? groupDict.GetValueOrDefault(group.ParentId.Value) : null;
            }
            return string.Join("/", pathSegments);
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
