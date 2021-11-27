using Conarium.Datatypes;
using Conarium.Services;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Conarium.UI
{
    public class ImageNode : BaseUIFunc
    {
        public ImageNode(string name) : base(name)
        {

        }
        public Texture2D Image {get;set;}
        public Color ImageColor {get;set;}

        public Rotation ImageRotation {get;set;}

        public Vector2 TextureOrigin {get;set;}
        public Rectangle ImageQuad {get;set;}

        public override Vector2 AbsolutePosition => Parent.AbsolutePosition;
        public override Vector2 AbsoluteSize => Parent.AbsoluteSize;
    
        public override void Draw()
        {
            var gfx = GraphicsService.Get();
            if (Image != null)
            {

                var sourceImageSize = new Vector2(Image.Width, Image.Height);
                gfx.Sprite(Image, AbsolutePosition, ImageQuad, ImageColor, ImageRotation, TextureOrigin, sourceImageSize/AbsoluteSize, SpriteEffects.None, 0);
            }
        }
    }
}