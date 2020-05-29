using Dgraph4Net.Annotations;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

#nullable enable

namespace Quests.DomainModels
{
    [DgraphType(nameof(Question))]
    [DataContract]
    public class Question : ACommonText
    {
        [JsonProperty("answers"), DataMember(Name = "answers")]
        [PredicateReferencesTo(typeof(Answer)), ReversePredicate, MinLength(0), Required(AllowEmptyStrings = false)]
        public virtual List<Answer> Answers { get; set; } = new List<Answer>();
    }
}

#nullable restore
