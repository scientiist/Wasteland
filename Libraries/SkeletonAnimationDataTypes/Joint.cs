using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace SkeletalAnimation
{
    public struct Joint
    {
        public int ParentId;
        public Vector2 Offset;
        public string TextureName;
        public Vector2 TextureOrigin;
    }
}
