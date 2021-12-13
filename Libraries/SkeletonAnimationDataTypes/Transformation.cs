using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace SkeletalAnimation
{
    public struct Transformation
    {
        public Vector2 Position;
        public Vector2 Scale;
        public float Rotation;

        private static Transformation _identity;
        public static Transformation Identity { get { return _identity; } }

        static Transformation()
        {
            _identity = new Transformation();
            _identity.Scale = Vector2.One;
        }

        public static Transformation Compose(Transformation key1, Transformation key2)
        {
            Transformation result = new Transformation();
            Vector2 transformedPosition = key1.TransformPoint(key2.Position);
            result.Position = transformedPosition;
            result.Rotation = key1.Rotation + key2.Rotation;
            result.Scale = key1.Scale * key2.Scale;
            return result;
        }

        public static void Lerp(ref Transformation key1, ref Transformation key2, float amount, ref Transformation result)
        {
            result.Position = Vector2.Lerp(key1.Position, key2.Position, amount);
            result.Scale = Vector2.Lerp(key1.Scale, key2.Scale, amount);
            result.Rotation = MathHelper.Lerp(key1.Rotation, key2.Rotation, amount);
        }

        public Vector2 TransformPoint(Vector2 point)
        {
            Vector2 result = Vector2.Transform(point, Matrix.CreateRotationZ(Rotation));
            result *= Scale;
            result += Position;
            return result;
        }

        public Transformation Translate(Vector2 translation)
        {
            Transformation result = this;
            result.Position += translation;
            return result;
        }
    }
}
