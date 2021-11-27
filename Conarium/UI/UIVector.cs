using Conarium.Extension;
using Microsoft.Xna.Framework;

namespace Conarium.UI
{
    public struct UIVector
    {
        public UIVector(Vector2 pixel, Vector2 scale)
        {
            PixelX = pixel.X;
			PixelY = pixel.Y;
			ScaleX = scale.X;
			ScaleY = scale.Y;
        }

        public float PixelX;
		public float PixelY;
		public float ScaleX;
		public float ScaleY;


		public static UIVector operator  +(UIVector a, UIVector b)
        {
			return new UIVector(
				a.PixelX + b.PixelX,
				a.PixelY + b.PixelY,
				a.ScaleX + b.ScaleX,
				a.ScaleY + b.ScaleY
			);
        }

		public Vector2 Scale
		{
			get
			{
				return new Vector2(ScaleX, ScaleY);
			}
			set
			{
				ScaleX = value.X;
				ScaleY = value.Y;
			}
		}

		public Vector2 Pixels
		{
			get { return new Vector2(PixelX, PixelY); }
			set
			{
				ScaleX = value.X;
				ScaleY = value.Y;
			}
		}


		public UIVector(float pixelX, float pixelY, float scaleX = 0, float scaleY = 0)
		{
			PixelX = pixelX;
			PixelY = pixelY;
			ScaleX = scaleX;
			ScaleY = scaleY;
		}

		public static UIVector FromScale(float x, float y)
		{
			return new UIVector(0, 0, x, y);
		}

		public static UIVector FromPixels(int x, int y)
		{
			return new UIVector(x, y, 0, 0);
		}

		public UIVector Lerp(UIVector b, float t)
		{
			return UIVector.Lerp(this, b, t);
		}

		public static UIVector Lerp(UIVector a, UIVector b, float t)
		{
			return new UIVector(
				Maths.LerpF(a.Pixels.X, b.Pixels.X, t),
				Maths.LerpF(a.Pixels.Y, b.Pixels.Y, t),
				Maths.LerpF(a.Scale.X, b.Scale.X, t),
				Maths.LerpF(a.Scale.Y, b.Scale.Y, t)
			);
		}
    }
}