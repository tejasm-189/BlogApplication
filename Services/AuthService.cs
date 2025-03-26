using Blazored.LocalStorage;
using System.Threading.Tasks;

namespace BlogApplication.Services
{
    public class AuthService
    {
        private readonly ILocalStorageService _localStorage;

        public AuthService(ILocalStorageService localStorage)
        {
            _localStorage = localStorage;
        }

        public async Task<string> GetCurrentUserId()
        {
            return await _localStorage.GetItemAsync<string>("userid");
        }

        public async Task<bool> IsUserAdmin()
        {
            var role = await _localStorage.GetItemAsync<string>("role");
            return role == "Admin";
        }
    }
}