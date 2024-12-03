// Services/OrderService.cs
using System.Collections.Generic;
using System.Threading.Tasks;
using API_Modul295.Models;
using API_Modul295.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq;

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
            return await _context.Orders
                .Where(o => !o.IsDeleted)
                .ToListAsync();
        }

        public async Task<Order> GetOrderByIdAsync(int id)
        {
            return (await _context.Orders
                .Where(o => o.OrderID == id && !o.IsDeleted)
                .FirstOrDefaultAsync())!;
        }

        public async Task<IEnumerable<Order>> GetOrdersByPriorityAsync(string priority)
        {
            string lowerPriority = priority.ToLower();

            return await _context.Orders
                .Where(o => o.Priority.ToLower() == lowerPriority && !o.IsDeleted)
                .ToListAsync();
        }

        public async Task<Order> CreateOrderAsync(OrderCreateRequest request)
        {
            // Validierung der Eingabedaten (optional, kann auch im Controller erfolgen)
            if (string.IsNullOrEmpty(request.CustomerName) ||
                string.IsNullOrEmpty(request.Email) ||
                string.IsNullOrEmpty(request.Priority) ||
                request.ServiceID <= 0)
            {
                throw new ArgumentException("Ungültige Eingabedaten.");
            }

            // Überprüfen, ob die angegebene Priorität gültig ist
            if (!PriorityLevels.All.Contains(request.Priority, StringComparer.OrdinalIgnoreCase))
            {
                throw new ArgumentException($"Ungültige Priorität. Gültige Werte sind: {string.Join(", ", PriorityLevels.All)}.");
            }

            // Überprüfen, ob die angegebene ServiceID existiert
            var serviceExists = await _context.Services.AnyAsync(s => s.ServiceID == request.ServiceID);
            if (!serviceExists)
            {
                throw new ArgumentException($"Die Dienstleistung mit der ID {request.ServiceID} existiert nicht.");
            }

            // Erstellen eines neuen Auftrags
            var newOrder = new Order
            {
                CustomerName = request.CustomerName,
                Email = request.Email,
                Phone = request.Phone,
                Priority = request.Priority,
                ServiceID = request.ServiceID,
                Status = "Offen",
                IsDeleted = false,
                DateCreated = DateTime.UtcNow
            };

            // Auftrag zur Datenbank hinzufügen
            _context.Orders.Add(newOrder);
            await _context.SaveChangesAsync();

            return newOrder;
        }
    }
}