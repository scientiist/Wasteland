using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;

namespace SkeletalAnimation
{
    public class KeyframeAnimation
    {
        [ContentSerializer(ElementName = "Keyframes", CollectionItemName = "Keyframe")]
        public List<Keyframe> Keyframes { get; set; }

        /// <summary>
        /// returns the length in Frames of the animation
        /// </summary>
        public int Length { get; set; }

        /// <summary>
        /// Number of frames per second
        /// </summary>
        public float Framerate { get; set; }

        public KeyframeAnimation()
        {
            Keyframes = new List<Keyframe>();
        }
    }
}
