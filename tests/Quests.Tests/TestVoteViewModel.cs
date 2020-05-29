using Newtonsoft.Json;
using Quests.DomainModels.ViewModels;
using System;

namespace Quests.Tests
{
    public class TestVoteViewModel : VoteViewModel
    {
        [JsonIgnore]
        internal Uid Id { get; set; }
        
        [JsonIgnore]
        public Uid QuestionId { get; set; }
        
        [JsonIgnore]
        public Uid AnswerId { get; set; }
    }
}
