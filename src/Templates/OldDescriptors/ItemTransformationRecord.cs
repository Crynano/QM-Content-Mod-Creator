using MGSC;
using System.Collections.Generic;

namespace QM_ImporterAPI
{
    public class ItemTransformationRecord : ConfigTableRecord
    {
        public List<ItemQuantity> OutputItems { get; set; }
    }
}