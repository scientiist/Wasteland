using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;

namespace SkeletalAnimation
{
    public class SkeletonAnimation
    {
        [ContentSerializer(FlattenContent = true, CollectionItemName = "KeyframeAnimation")]
        public KeyframeAnimation[] JointAnimations { get; set; }
    }
}
