using System.Threading.Tasks;

using WdtA2Api.Core;
using WdtA2Api.Core.Repository;
using WdtA2Api.Data.Repository;

namespace WdtA2Api.Data
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly WdtA2ApiContext _context;

        public UnitOfWork(WdtA2ApiContext context)
        {
            _context = context;
            Faq = new FaqRepository(_context);
            Room = new RoomRepository(_context);
            User = new UserRepository(_context);
        }

        public IFaqRepository Faq { get; }
        public IRoomRepository Room { get; }
        public IUserRepository User { get; }
        public async Task<int> CompleteAsync() => await _context.SaveChangesAsync();
        public void Dispose() => _context.Dispose();        
    }
}
