using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using WdtA2Api.Core.Repository;

using WdtModels.ApiModels;

namespace WdtA2Api.Core.Repository
{
    public interface IUserRepository : IRepository<User>
    {
    }
}
