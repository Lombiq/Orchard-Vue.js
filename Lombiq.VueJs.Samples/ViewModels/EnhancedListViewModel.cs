using System.Collections.Generic;

namespace Lombiq.VueJs.Samples.ViewModels;

public class EnhancedListViewModel
{
    public int Page { get; set; }
    public IEnumerable<EnhancedListViewModelData> Data { get; set; }

    public class EnhancedListViewModelData
    {
        public int Number { get; set; }
        public string Date { get; set; }
        public string Day { get; set; }
        public int Random { get; set; }
    }
}
