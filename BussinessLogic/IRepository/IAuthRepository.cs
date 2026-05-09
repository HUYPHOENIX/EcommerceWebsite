using BussinessLogic.Entities;

namespace BussinessLogic.IRepository
{
    public interface IAuthRepository
    {
        Task<User> FindByEmailAsync(string email);
        Task<User> CreateUserAsync(User user, string password);
        Task<bool> ValidatePasswordAsync(User user, string password);
        Task AddToRoleAsync(User user, string role);
        Task<IList<string>> GetRolesAsync(User user);
        
    }
}