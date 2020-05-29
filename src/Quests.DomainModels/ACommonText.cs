using Dgraph4Net;
using Dgraph4Net.Annotations;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

#nullable enable

namespace Quests.DomainModels
{
    [DataContract]
    public abstract class ACommonText : AEntity
    {
        [JsonProperty("formatted_text")]
        [DataMember(Name = "formatted_text")]
        [StringPredicate]
        [Required(AllowEmptyStrings = false)]
        public virtual string FormattedText { get; set; } = string.Empty;

        [JsonProperty("text")]
        [DataMember(Name = "text")]
        [StringPredicate(Fulltext = true, Token = StringToken.Term, Trigram = true)]
        [Required(AllowEmptyStrings = false)]
        public virtual string PureText { get; set; } = string.Empty;
    }
}

#nullable restore
