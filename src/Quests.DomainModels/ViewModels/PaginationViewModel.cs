using System.Collections.Generic;

namespace Quests.DomainModels.ViewModels
{
    public class PaginationViewModel<T>
    {
        public ICollection<T> Items { get; set; } = new List<T>();
        public ulong Total { get; set; } = 0;
    }
}
