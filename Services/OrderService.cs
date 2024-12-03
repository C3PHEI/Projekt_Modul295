using System.Collections.Generic;
using System.Threading.Tasks;
using API_Modul295.Models;
using API_Modul295.Data;
using Microsoft.EntityFrameworkCore;

namespace API_Modul295.Services
{
    public class OrderService : IOrderService
    {
        private readonly ApiDbContext _context;

        public OrderService(ApiDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Order>> GetOrdersAsync()
        {
            // Geschäftslogik hier implementieren
            return await _context.Orders
                .Where(o => !o.IsDeleted) // Nur nicht gelöschte Aufträge
                .ToListAsync();
        }
    }
}