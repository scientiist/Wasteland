using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Conarium.Extension
{
    public static class MonoGameExtensions 
    {
        public static Rectangle GetSpriteFrame(this Rectangle[] animation, float animationTime) 
        {
            int animLength = animation.Length;
            return animation[(int)(animationTime % animLength)];
        }

        #region Vector2 Extensions

        public static float DistanceTo(this Vector2 a, Vector2 b) => (a-b).Length();

        public static Vector2 Unit(this Vector2 input)
        {
            var copy = new Vector2(input.X, input.Y);
            copy.Normalize();
            return copy;
        }

        public static Vector2 Lerp(this Vector2 a, Vector2 b, float alpha) 
            => new Vector2(
                Maths.LerpF(a.X, b.X, alpha),
                Maths.LerpF(a.Y, b.Y, alpha));


        public static Vector2 RoundTo(this Vector2 original, int decimalplaces) 
            => new Vector2(
                (float)Math.Round(original.X, decimalplaces),
                (float)Math.Round(original.Y, decimalplaces));

        #endregion

        public static float GetDelta(this GameTime gametime) 
            => (float)gametime.ElapsedGameTime.TotalSeconds;

    }
}
