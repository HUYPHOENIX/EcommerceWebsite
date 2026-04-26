using BussinessLogic.Entities;
using BussinessLogic.Interfaces;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class OrderRepository : IOrderRepository
{
    private readonly AppDbContext _Context;
    public OrderRepository(AppDbContext context)
    {
        _Context = context;
    }

    public async Task<int> CreateOrderAsync(Order order)
    {
        _Context.Orders.Add(order);
        await _Context.SaveChangesAsync();

        return order.Id;
    }

    public async Task<Order?> GetOrderByIdAsync(int id)
    {
        return await _Context.Orders
        .Include(ListItem => ListItem.OrderItems).FirstOrDefaultAsync(ListItem => ListItem.Id == id);
    }
}