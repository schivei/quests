using Newtonsoft.Json.Converters;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace Quests.DomainModels
{
    [Newtonsoft.Json.JsonConverter(typeof(StringEnumConverter))]
    [JsonConverter(typeof(JsonStringEnumConverter))]
    [DataContract]
    public enum VoteType
    {
        [Display(Name = "Positive")]
        [EnumMember(Value = "positive")]
        [DataMember(Name = "positive")]
        Positive,
        
        [Display(Name = "Negative")]
        [EnumMember(Value = "negative")]
        [DataMember(Name = "negative")]
        Negative
    }
}
