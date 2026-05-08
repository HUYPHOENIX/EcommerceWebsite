using BussinessLogic.Entities;
using BussinessLogic.IRepository;
using Infrastructure.Data;

namespace Infrastructure.Repositories;

public class OrderRepository : IOrderRepository
{
    private readonly AppDbContext _Context;
    public OrderRepository(AppDbContext context)
    {
        _Context = context;
    }


    public async Task<Order> CreateOrderAsync(Order order)
    {
        _Context.Orders.Add(order);
        await _Context.SaveChangesAsync();

        return order;
    }
}