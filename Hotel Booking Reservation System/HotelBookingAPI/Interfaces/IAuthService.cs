using HotelBookingAPI.Models;

namespace HotelBookingAPI.Interfaces
{
    public interface IAuthService
    {
        Task<string> RegisterUserAsync(RegisterModel model);
        Task<string> LoginUserAsync(LoginModel model);
        Task<string> AssignRoleAsync(string email, string roleName);
        Task<bool> CheckUserExistsAsync(string email);
        Task<object> GetCurrentUserAsync(string userId);

    }
}
