using Conarium.Datatypes;
using Conarium;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Conarium.UI
{
    public class NineSliceNode : BaseUIFunc
    {

        public NineSliceNode(string name) : base(name) {}

        public Texture2D Texture { get; set; }

        public Rectangle TopLeftCornerQuad { get; set; }
        public Rectangle BottomLeftCornerQuad { get; set; }
        public Rectangle TopRightCornerQuad { get; set; }
        public Rectangle BottomRightCornerQuad { get; set; }
        public Rectangle LeftSideQuad { get; set; }
        public Rectangle TopSideQuad { get; set; }
        public Rectangle RightSideQuad { get; set; }
        public Rectangle BottomSideQuad { get; set; }
        public Rectangle InteriorQuad { get; set; }

        public float TextureScale { get; set; }
        public Color TextureColor { get; set; }
        public new Vector2 AbsoluteSize => Parent.AbsoluteSize;
        public new Vector2 AbsolutePosition => Parent.AbsolutePosition;
        public new UIVector Position { get => Parent.Position; set { } }
        public new UIVector Size { get => Parent.Size; set { } }

        public override void Draw()
        {
            var GFX = GraphicsService.Get();
            // topleft
            GFX.Sprite(Texture, AbsolutePosition, TopLeftCornerQuad, TextureColor, Rotation.Zero, Vector2.Zero, TextureScale, SpriteEffects.None, 0);

            // left
            Vector2 TopLeftCornerOffset = new Vector2(TopLeftCornerQuad.Width * TextureScale, TopLeftCornerQuad.Height * TextureScale);
            Vector2 BottomLeftCornerOffset = new Vector2(BottomLeftCornerQuad.Width * TextureScale, BottomLeftCornerQuad.Height * TextureScale);
            Vector2 TopRightCornerOffset = new Vector2(TopRightCornerQuad.Width * TextureScale, TopRightCornerQuad.Height * TextureScale);
            Vector2 BottomRightCornerOffset = new Vector2(BottomRightCornerQuad.Width * TextureScale, BottomRightCornerQuad.Height * TextureScale);

            float SideLength = AbsoluteSize.Y - TopLeftCornerOffset.Y - BottomLeftCornerOffset.Y;
            GFX.Sprite(Texture, AbsolutePosition + new Vector2(0, TopLeftCornerOffset.Y), LeftSideQuad, TextureColor, Rotation.Zero, Vector2.Zero, new Vector2(TextureScale, SideLength / LeftSideQuad.Height), SpriteEffects.None, 0);
           
            //bottomleft
            GFX.Sprite(Texture, AbsolutePosition + new Vector2(0, SideLength + TopLeftCornerOffset.Y), BottomLeftCornerQuad, TextureColor, Rotation.Zero, Vector2.Zero, TextureScale, SpriteEffects.None, 0);

            //top

            float TopLength = AbsoluteSize.X - TopLeftCornerOffset.X - TopRightCornerOffset.X;
            GFX.Sprite(Texture, AbsolutePosition +new Vector2(TopLeftCornerOffset.X, 0), TopSideQuad, TextureColor, Rotation.Zero, Vector2.Zero, new Vector2(TopLength/TopSideQuad.Width, TextureScale), SpriteEffects.None, 0);

            // topright
            Vector2 TopRightOffset = new Vector2(TopLeftCornerOffset.X + TopLength, 0);
            GFX.Sprite(Texture, AbsolutePosition + TopRightOffset, TopRightCornerQuad, TextureColor, Rotation.Zero, Vector2.Zero, TextureScale, SpriteEffects.None, 0);

            // Right
            GFX.Sprite(Texture, AbsolutePosition + new Vector2(TopRightOffset.X, TopRightCornerOffset.Y), RightSideQuad, TextureColor, Rotation.Zero, Vector2.Zero, new Vector2(TextureScale, SideLength / RightSideQuad.Height), SpriteEffects.None, 0);

            // bottomright
            GFX.Sprite(Texture, AbsolutePosition + new Vector2(TopRightOffset.X, TopRightCornerOffset.Y+SideLength), BottomRightCornerQuad, TextureColor, Rotation.Zero, Vector2.Zero, TextureScale, SpriteEffects.None, 0);

            // bottom
            GFX.Sprite(Texture, AbsolutePosition + TopLeftCornerOffset + new Vector2(0, SideLength), BottomSideQuad, TextureColor, Rotation.Zero, Vector2.Zero, new Vector2(TopLength/BottomSideQuad.Width, TextureScale), SpriteEffects.None, 0);
        }


    }
}