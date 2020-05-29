using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace Quests.DomainModels.ViewModels
{
    public class QuestionViewModel
    {
        [JsonProperty("formatted_text")]
        [MaxLength(2048)]
        [Required(AllowEmptyStrings = false)]
        public string FormattedText { get; set; }

        [JsonProperty("user")]
        [Required(AllowEmptyStrings = false)]
        [MinLength(3), MaxLength(32)]
        [RegularExpression("^([a-z][a-z0-9]+)(\\.[a-z0-9]+)?$")]
        public string User { get; set; }
    }
}
