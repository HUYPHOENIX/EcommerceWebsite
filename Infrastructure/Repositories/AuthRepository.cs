using BussinessLogic.Entities;
using BussinessLogic.IRepository;
using Microsoft.AspNetCore.Identity;

namespace Infrastructure.Repositories
{
    public class AuthRepository : IAuthRepository
    {
        private readonly UserManager<User> _userManager;

        public AuthRepository(UserManager<User> userManager)
        {
            _userManager = userManager;
        }

        public async Task<User> FindByEmailAsync(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentException("Email không được trống");
            
            return await _userManager.FindByEmailAsync(email);
        }

        public async Task<User> CreateUserAsync(User user, string password)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            var result = await _userManager.CreateAsync(user, password);
            
            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                throw new InvalidOperationException($"Tạo user bị lỗi: {errors}");
            }

            return user;
        }

        public async Task<bool> ValidatePasswordAsync(User user, string password)
        {
            if (user == null || string.IsNullOrWhiteSpace(password))
                return false;

            return await _userManager.CheckPasswordAsync(user, password);
        }

        public async Task AddToRoleAsync(User user, string role)
        {
            if (user == null || string.IsNullOrWhiteSpace(role))
                throw new ArgumentException("Không có user hoặc role không đúng.");

            var result = await _userManager.AddToRoleAsync(user, role);
            
            if (!result.Succeeded)
                throw new InvalidOperationException($"Không thể add role: {role}");
        }

        public async Task<IList<string>> GetRolesAsync(User user)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            return await _userManager.GetRolesAsync(user);
        }
    }
}
