
using BussinessLogic.Entities;
namespace BussinessLogic.Interfaces{
public interface IOrderRepository
{
    Task<int> CreateOrderAsync(Order order);
    Task<Order?> GetOrderByIdAsync(int id);
}}