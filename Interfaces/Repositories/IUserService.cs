using certificated_unemi.Models.users;

namespace certificated_unemi.Interfaces.repositories
{
    public interface IUserService
    {
        Task<User> AuthenticateAsync(string email, string password);

    }
}
