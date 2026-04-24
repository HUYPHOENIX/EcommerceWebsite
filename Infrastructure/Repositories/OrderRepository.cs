using BussinessLogic.Entities;
using BussinessLogic.Interfaces;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class OrderRepository : IOrderRepository
{
    private readonly AppDbContext _appDbContext;
    public OrderRepository(AppDbContext appDbContext)
    {
        _appDbContext = appDbContext;
    }

    public async Task<int> CreateOrderAsync(Order order)
    {
        _appDbContext.Orders.Add(order);
        await _appDbContext.SaveChangesAsync();

        return order.Id;
    }

    public async Task<Order?> GetOrderByIdAsync(int id)
    {
        return await _appDbContext.Orders
        .Include(o => o.OrderItems).FirstOrDefaultAsync(o => o.Id == id);
    }
}