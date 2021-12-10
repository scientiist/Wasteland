using Microsoft.Xna.Framework;

namespace Conarium.Datatypes
{
	/// <summary>
	/// Renderable 2D Transformations - position, scale, rotation
	/// </summary>
	public struct Transformation
	{
		public static readonly Transformation Identity = new Transformation();
		public Vector2 Position {get;set;}
		public Vector2 Scale {get;set;}

		public float Rotation {get;set;}


		public static Transformation Compose(Transformation a, Transformation b)
		{
			Transformation result = new Transformation();
			Vector2 transformedPosition = a.TransformVector(b.Position);
			result.Position = transformedPosition;
			result.Rotation = a.Rotation + b.Rotation;
			result.Scale = a.Scale * b.Scale;
			return result;
		}

		public static Transformation Lerp(Transformation key1, Transformation key2, float amount)
		{
			Transformation result = new Transformation();
			result.Position = Vector2.Lerp(key1.Position, key2.Position, amount);
			result.Scale = Vector2.Lerp(key1.Scale, key2.Scale, amount);
			result.Rotation = MathHelper.Lerp(key1.Rotation, key2.Rotation, amount);
			return result;
		}

		public Transformation Lerp(Transformation key, float amount) => Lerp(this, key, amount);

		public Vector2 TransformVector(Vector2 point)
		{
			Vector2 result = Vector2.Transform(point, Matrix.CreateRotationZ(Rotation));
			result *= Scale;
			result *= Position;
			return result;
		}
	}
}