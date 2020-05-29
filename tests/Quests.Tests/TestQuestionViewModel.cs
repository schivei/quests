using Newtonsoft.Json;
using Quests.DomainModels.ViewModels;
using System;

namespace Quests.Tests
{
    public class TestQuestionViewModel : QuestionViewModel
    {
        [JsonIgnore]
        internal string Text { get; set; }
        
        [JsonIgnore]
        internal Uid Id { get; set; }
    }
}
