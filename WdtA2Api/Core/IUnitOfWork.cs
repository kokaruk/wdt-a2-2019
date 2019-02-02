using System;
using System.Threading.Tasks;

using WdtA2Api.Core.Repository;
using WdtA2Api.Data.Repository;

namespace WdtA2Api.Core
{
    public interface IUnitOfWork : IDisposable
    {
        IFaqRepository Faq { get; }
        IRoomRepository Room { get; }
        IUserRepository User { get; }
        Task<int> CompleteAsync();
    }
}
