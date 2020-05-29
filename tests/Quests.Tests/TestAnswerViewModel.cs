using Newtonsoft.Json;
using Quests.DomainModels.ViewModels;
using System;

namespace Quests.Tests
{
    public class TestAnswerViewModel : AnswerViewModel
    {
        [JsonIgnore]
        internal string Text { get; set; }
        
        [JsonIgnore]
        internal Uid Id { get; set; }
        
        [JsonIgnore]
        public Uid QuestionId { get; set; }
    }
}
