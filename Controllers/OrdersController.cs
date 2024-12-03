using Microsoft.AspNetCore.Mvc;
using API_Modul295.Services;
using API_Modul295.Models;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace API_Modul295.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrdersController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        // GET: api/orders
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Order>>> GetOrders()
        {
            var orders = await _orderService.GetOrdersAsync();
            return Ok(orders);
        }
        
        // GET: api/orders/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<Order>> GetOrderById(int id)
        {
            if (id <= 0)
            {
                return BadRequest("Die angegebene ID ist ungültig.");
            }

            var order = await _orderService.GetOrderByIdAsync(id);

            if (order == null)
            {
                return NotFound($"Kein Auftrag mit der ID {id} gefunden.");
            }

            return Ok(order);
        }
        
        // GET: api/orders/priority/{priority}
        [HttpGet("priority/{priority}")]
        public async Task<ActionResult<IEnumerable<Order>>> GetOrdersByPriority(string priority)
        {
            if (string.IsNullOrEmpty(priority))
            {
                return BadRequest("Die Priorität darf nicht leer sein.");
            }

            // Validierung der Prioritätswerte (optional)
            if (!PriorityLevels.All.Contains(priority, StringComparer.OrdinalIgnoreCase))
            {
                return BadRequest($"Ungültige Priorität. Gültige Werte sind: {string.Join(", ", PriorityLevels.All)}.");
            }

            var orders = await _orderService.GetOrdersByPriorityAsync(priority);

            if (orders == null || !orders.Any())
            {
                return NotFound($"Keine Aufträge mit der Priorität '{priority}' gefunden.");
            }

            return Ok(orders);
        }
        
        // POST: api/orders
        //TODO Validierung (E-Mail/Phone)
        [HttpPost]
        public async Task<ActionResult<Order>> CreateOrder([FromBody] OrderCreateRequest request)
        {
            try
            {
                if (request == null)
                {
                    return BadRequest("Die Anfrage darf nicht leer sein.");
                }

                // Optional: Validierung der Eingabedaten im Controller
                if (string.IsNullOrEmpty(request.CustomerName) ||
                    string.IsNullOrEmpty(request.Email) ||
                    string.IsNullOrEmpty(request.Priority) ||
                    request.ServiceID <= 0)
                {
                    return BadRequest("Bitte füllen Sie alle erforderlichen Felder aus.");
                }

                // Überprüfen, ob die Priorität gültig ist
                if (!PriorityLevels.All.Contains(request.Priority, StringComparer.OrdinalIgnoreCase))
                {
                    return BadRequest(
                        $"Ungültige Priorität. Gültige Werte sind: {string.Join(", ", PriorityLevels.All)}.");
                }

                var newOrder = await _orderService.CreateOrderAsync(request);

                // Rückgabe des erstellten Auftrags mit HTTP 201 Created
                return CreatedAtAction(nameof(GetOrderById), new { id = newOrder.OrderID }, newOrder);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                // Allgemeine Fehlerbehandlung
                return StatusCode(500, $"Interner Serverfehler: {ex.Message}");
            }
        }
    }
}