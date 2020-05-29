using Dgraph4Net;

namespace Quests.Repositories
{
    public abstract class ACommonStore
    {
        protected ACommonStore(Dgraph4NetClient client) => Client = client;

        protected Dgraph4NetClient Client { get; }
    }
}
