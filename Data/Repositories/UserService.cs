using certificated_unemi.Data.Context;
using certificated_unemi.Interfaces.repositories;
using certificated_unemi.Models.users;
using Microsoft.EntityFrameworkCore;

namespace certificated_unemi.Data.Repositories
{
    public class UserService : IUserService
    {
        private readonly AppDbContext _context;

        public UserService(AppDbContext context)
        {
            _context = context;
        }
        public async Task<User> AuthenticateAsync(string email, string password)
        {
            try
            {
                var user = await _context.Users
             .FirstOrDefaultAsync(u => u.Email == email);

                if (user == null || !VerifyPassword(user.PasswordHash, password))
                {
                    return null; // Retorna null si el usuario no existe o la contraseña no coincide
                }

                return user;
            }
            catch (Exception ex)
            {

                throw new ArgumentException(ex.Message);
            }
            // Busca al usuario por correo

        }

        private bool VerifyPassword(string passwordHash, string password)
        {

            return passwordHash == password;
        }
    }
}
