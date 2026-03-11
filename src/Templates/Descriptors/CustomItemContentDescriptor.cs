using UnityEngine;

namespace QM_ImporterAPI.Templates.Descriptors
{
    public class CustomItemContentDescriptor : CustomBaseDescriptor
    {
        public CustomItemContentDescriptor() : base() { }
        public ImageProperties ImageProperties { get; set; }
    }

    [SerializeField]
    public class ImageProperties
    {
        public string IconSpriteIdOrPath { get; set; }
        public string SmallIconSpriteIdOrPath { get; set; }
        public string ShadowOnFloorSpriteIdOrPath { get; set; }
    }
}