using System.Collections.Generic;

namespace MGSC
{
    public class ItemTransformationRecord
    {
        public IEnumerable<OutputItem> OutputItems { get; set; }
        public string Id { get; set; }
    }

    public class OutputItem
    {
        public string ItemId { get; set; }
        public int Count { get; set; }
    }
}