using Falabara.Domain.Entities;
using Falabara.Infrastructure.Data;

namespace Falabara.Infrastructure.Repositories
{
    public class UserRepository
    {
        private readonly AppDbContext _context;

        public UserRepository(AppDbContext context)
        {
            _context = context;
        }

        public User? GetByEmail(string email)
        {
            return _context.Users.FirstOrDefault(u => u.Email == email);
        }

        public void Add(User usuario)
        {
            _context.Users.Add(usuario);
            _context.SaveChanges();
        }

        public bool EmailExiste(string email)
        {
            return _context.Users.Any(u => u.Email == email);
        }
    }
}
