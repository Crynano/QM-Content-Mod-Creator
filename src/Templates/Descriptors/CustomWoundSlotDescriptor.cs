using MGSC;
using QM_ImporterAPI.Services.Helpers;
using QM_ImporterAPI.Services.Importing;
using UnityEngine;

namespace QM_ImporterAPI.Templates.Descriptors
{
    public class CustomWoundSlotDescriptor : CustomBaseDescriptor
    {
        public CustomWoundSlotDescriptor() : base()
        {
        
        }
    
        public Vector2 SlotPosition {get; set;} = Vector2.zero;

        public string NormalIconPath {get; set;} = string.Empty;

        public string WoundedIconPath {get; set;} = string.Empty;

        public string FixatedIconPath {get; set;} = string.Empty;

        public string AmputatedIconPath {get; set;} = string.Empty;
    
        public WoundSlotDescriptor GetOriginal()
        {
            WoundSlotDescriptor original = ScriptableObject.CreateInstance<WoundSlotDescriptor>();

            original._slotPosition = this.SlotPosition;
            original._normalIcon = QuasimorphHelper.GetPropertyFromList<WoundSlotRecord, WoundSlotDescriptor>(NormalIconPath, "NormalIcon", MGSC.Data.WoundSlots) as Sprite ?? AssetImporter.LoadNewSprite(NormalIconPath);
            original._woundedIcon = QuasimorphHelper.GetPropertyFromList<WoundSlotRecord, WoundSlotDescriptor>(WoundedIconPath, "WoundedIcon", MGSC.Data.WoundSlots) as Sprite ?? AssetImporter.LoadNewSprite(WoundedIconPath);
            original._fixatedIcon = QuasimorphHelper.GetPropertyFromList<WoundSlotRecord, WoundSlotDescriptor>(FixatedIconPath, "FixatedIcon", MGSC.Data.WoundSlots) as Sprite ?? AssetImporter.LoadNewSprite(FixatedIconPath);
            original._amputatedIcon = QuasimorphHelper.GetPropertyFromList<WoundSlotRecord, WoundSlotDescriptor>(AmputatedIconPath, "AmputatedIcon", MGSC.Data.WoundSlots) as Sprite ?? AssetImporter.LoadNewSprite(AmputatedIconPath);
        
            return original;
        }
    }
}