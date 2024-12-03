using System.Collections.Generic;
using System.Threading.Tasks;
using API_Modul295.Models;

namespace API_Modul295.Services
{
    public interface IOrderService
    {
        Task<IEnumerable<Order>> GetOrdersAsync();
    }
}