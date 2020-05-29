using Dgraph4Net;
using Dgraph4Net.Annotations;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

#nullable enable

namespace Quests.DomainModels
{
    [DgraphType(nameof(Vote))]
    [DataContract]
    public class Vote : AEntity
    {
        [JsonProperty("type"), Required, DataMember(Name = "type")]
        [StringPredicate(Token = StringToken.Exact)]
        [Newtonsoft.Json.JsonConverter(typeof(StringEnumConverter))]
        [System.Text.Json.Serialization.JsonConverter(typeof(JsonStringEnumConverter))]
        public virtual VoteType VoteType { get; set; } = VoteType.Positive;
    }
}

#nullable restore
