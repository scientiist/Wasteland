using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;
using System.Xml.Serialization;

namespace SkeletalAnimation
{
    public class Keyframe
    {
        public int FrameNumer { get; set; }

        [ContentSerializer(FlattenContent = true)]
        public Transformation Transform { get; set; }
    }
}
