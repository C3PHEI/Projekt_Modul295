﻿using System.Collections.Generic;
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
            return await _context.Orders
                .Where(o => !o.IsDeleted)
                .ToListAsync();
        }

        public async Task<Order> GetOrderByIdAsync(int id)
        {
            return await _context.Orders
                .Where(o => o.OrderID == id && !o.IsDeleted)
                .FirstOrDefaultAsync();
        }
    }
}