using System.Threading.Tasks;
using DataLayer.Models;

namespace RepositoryLayer.Interfaces
{
    public interface IUserRepository
    {
        Task CreateUserAsync(Registration user);
        Task<Registration> AuthenticateAsync(string username, string password);
        Task<bool> UserExistsAsync(string username);
        Task<bool> EmailExistsAsync(string email);
        Task<Registration> GetUserByUsernameAsync(string username);
    }
}
