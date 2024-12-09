using System.Collections.Generic;
using System.Threading.Tasks;
using API_Modul295.Models;

namespace API_Modul295.Services
{
    public interface IOrderService
    {
        Task<IEnumerable<Order>> GetOrdersAsync();
        Task<Order> GetOrderByIdAsync(int id);
        Task<List<OrderDto>> GetOrdersByPriorityAsync(string priority);
        Task<Order> CreateOrderAsync(OrderCreateRequest request);
        Task<bool> MarkOrderAsDeleted(int id);
    }
}