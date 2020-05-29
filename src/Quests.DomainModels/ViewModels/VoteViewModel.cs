using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Quests.DomainModels.ViewModels
{
    public class VoteViewModel
    {
        [JsonProperty("user")]
        [Required(AllowEmptyStrings = false)]
        [MinLength(3), MaxLength(32)]
        [RegularExpression("^([a-z][a-z0-9]+)(\\.[a-z0-9]+)?$")]
        public string User { get; set; }

        [JsonProperty("type")]
        [Newtonsoft.Json.JsonConverter(typeof(StringEnumConverter))]
        [System.Text.Json.Serialization.JsonConverter(typeof(JsonStringEnumConverter))]
        public VoteType Type { get; set; }
    }

    public class VoteViewModelCreate : Vote
    {
        [JsonProperty("~votes")]
        public Uid AnswerId { get; set; }
    }
}
