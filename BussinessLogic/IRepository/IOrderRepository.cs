
using BussinessLogic.Entities;
namespace BussinessLogic.IRepository{
public interface IOrderRepository
{
    Task<Order> CreateOrderAsync(Order order);
}}