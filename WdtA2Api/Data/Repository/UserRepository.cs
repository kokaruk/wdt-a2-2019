using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;

using WdtA2Api.Core.Repository;

using WdtModels.ApiModels;

namespace WdtA2Api.Data.Repository
{
    public class UserRepository : Repository<User>, IUserRepository
    {
        public UserRepository(WdtA2ApiContext context)
            : base(context)
        {
        }

        public WdtA2ApiContext WdWdtA2ApiContext => Context as WdtA2ApiContext;
    }
}
