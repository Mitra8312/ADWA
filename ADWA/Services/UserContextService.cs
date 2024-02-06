
using ADWA.Db;
using ADWA.Models;

namespace ADWA.Services
{
    public class UsrSvc
    {
        private readonly ContextDb _context;

        public UsrSvc(ContextDb context)
        {
            _context = context;
        }

        public async Task AddUserAsync(User user)
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
        }
    }
}
