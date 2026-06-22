using System.Collections.Generic;

namespace MGSC
{
    public class ItemTransformationRecord : ConfigTableRecord
    {
        public List<ItemQuantity> OutputItems { get; set; }
    }
}