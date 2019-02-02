using WdtA2Api.Core.Repository;

using WdtModels.ApiModels;

namespace WdtA2Api.Data.Repository
{
    public class FaqRepository : Repository<Faq>, IFaqRepository
    {
        public FaqRepository(WdtA2ApiContext context)
            : base(context)
        {
        }

        public WdtA2ApiContext WdWdtA2ApiContext => Context as WdtA2ApiContext;
    }
}
