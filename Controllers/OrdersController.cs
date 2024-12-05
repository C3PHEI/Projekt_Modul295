using Microsoft.AspNetCore.Mvc;
using API_Modul295.Services;
using API_Modul295.Models;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using API_Modul295.Data;
using System.Linq;
using System;

namespace API_Modul295.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderService _orderService;
        private readonly ApiDbContext _context;

        // Konstruktor für die Injektion von IOrderService und ApiDbContext
        public OrdersController(IOrderService orderService, ApiDbContext context)
        {
            _orderService = orderService;
            _context = context;
        }

        // GET: api/orders
        [HttpGet]
        public async Task<ActionResult<IEnumerable<OrderDto>>> GetOrders()
        {
            var orders = await _orderService.GetOrdersAsync();

            var orderDtos = orders.Select(o => new OrderDto
            {
                OrderID = o.OrderID,
                CustomerName = o.CustomerName,
                Email = o.Email,
                Phone = o.Phone,
                Priority = o.Priority,
                ServiceID = o.ServiceID,
                ServiceName = o.Service.ServiceName,
                Status = o.Status,
                DateCreated = o.DateCreated,
                DateModified = o.DateModified
            });

            return Ok(orderDtos);
        }

        // GET: api/orders/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<OrderDto>> GetOrderById(int id)
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

            var orderDto = new OrderDto
            {
                OrderID = order.OrderID,
                CustomerName = order.CustomerName,
                Email = order.Email,
                Phone = order.Phone,
                Priority = order.Priority,
                ServiceID = order.ServiceID,
                ServiceName = order.Service.ServiceName,
                Status = order.Status,
                DateCreated = order.DateCreated,
                DateModified = order.DateModified
            };

            return Ok(orderDto);
        }

        // GET: api/orders/priority/{priority}
        [HttpGet("priority/{priority}")]
        public async Task<ActionResult<IEnumerable<OrderDto>>> GetOrdersByPriority(string priority)
        {
            if (string.IsNullOrEmpty(priority))
            {
                return BadRequest("Die Priorität darf nicht leer sein.");
            }

            // Validierung der Prioritätswerte
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
        [HttpPost]
        public async Task<ActionResult<OrderDto>> CreateOrder([FromBody] OrderCreateRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var newOrder = await _orderService.CreateOrderAsync(request);

                var orderDto = new OrderDto
                {
                    OrderID = newOrder.OrderID,
                    CustomerName = newOrder.CustomerName,
                    Email = newOrder.Email,
                    Phone = newOrder.Phone,
                    Priority = newOrder.Priority,
                    ServiceID = newOrder.ServiceID,
                    ServiceName = newOrder.Service.ServiceName,
                    Status = newOrder.Status,
                    DateCreated = newOrder.DateCreated,
                    DateModified = newOrder.DateModified
                };

                return CreatedAtAction(nameof(GetOrderById), new { id = newOrder.OrderID }, orderDto);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Interner Serverfehler: {ex.Message}");
            }
        }

        // PUT: api/orders/{id}/status
        [HttpPut("{id}/status")]
        [Authorize]  // Authentifizierung erforderlich
        public IActionResult UpdateOrderStatus(int id, [FromBody] UpdateStatusRequest request)
        {
            var order = _context.Orders.SingleOrDefault(o => o.OrderID == id);

            if (order == null || order.IsDeleted)
                return NotFound("Order not found or marked as deleted.");

            order.Status = request.Status;
            order.DateModified = DateTime.UtcNow;

            _context.SaveChanges();
            return Ok(order);
        }

        // DELETE: api/orders/{id}
        [HttpDelete("{id}")]
        [Authorize]  // Authentifizierung erforderlich
        public IActionResult DeleteOrder(int id)
        {
            var order = _context.Orders.SingleOrDefault(o => o.OrderID == id);

            if (order == null || order.IsDeleted)
                return NotFound("Order not found or already deleted.");

            order.IsDeleted = true;
            _context.SaveChanges();

            return Ok("Order marked as deleted.");
        }
    }
}
