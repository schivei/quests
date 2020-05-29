using Dgraph4Net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Quests.DomainModels.ViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Quests.Repositories.Extensions
{
    public static class TxnExtensions
    {
        public static async Task<PaginationViewModel<T>> PaginateWithVars<T>(this Txn txn, string param, string query, Dictionary<string, string> vars)
        {
            var resp = await txn.QueryWithVars(query, vars).ConfigureAwait(false);

            var data = JsonConvert.DeserializeObject<Dictionary<string, object[]>>(resp.Json.ToStringUtf8());

            var c = resp.Metrics.NumUids.ContainsKey("c") ? resp.Metrics.NumUids["c"] / 2 : 0;

            return new PaginationViewModel<T>
            {
                Items = JArray.FromObject(data[param]).ToObject<T[]>(),
                Total = data.ContainsKey("it") ? ulong.Parse(JArray.FromObject(data["it"])[0]["count"].ToString()) : c
            };
        }

        public static async Task<PaginationViewModel<T>> Paginate<T>(this Txn txn, string param, string query)
        {
            var resp = await txn.Query(query).ConfigureAwait(false);

            var data = JsonConvert.DeserializeObject<Dictionary<string, object[]>>(resp.Json.ToStringUtf8());

            var c = resp.Metrics.NumUids.ContainsKey("c") ? resp.Metrics.NumUids["c"] / 2 : 0;

            return new PaginationViewModel<T>
            {
                Items = JArray.FromObject(data[param]).ToObject<T[]>(),
                Total = data.ContainsKey("it") ? ulong.Parse(JArray.FromObject(data["it"])[0]["count"].ToString()) : c
            };
        }
    }
}
