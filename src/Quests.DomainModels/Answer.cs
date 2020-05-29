using Dgraph4Net.Annotations;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

#nullable enable

namespace Quests.DomainModels
{
    [DgraphType(nameof(Answer))]
    [DataContract]
    public class Answer : ACommonText
    {
        [JsonProperty("votes"), DataMember(Name = "votes")]
        [PredicateReferencesTo(typeof(Vote)), ReversePredicate]
        [MinLength(0)]
        public virtual List<Vote> Votes { get; set; } = new List<Vote>();
    }
}

#nullable restore
