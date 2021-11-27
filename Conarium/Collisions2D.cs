using System;
using Conarium.Extension;
using Microsoft.Xna.Framework;

namespace Conarium
{

    public interface IBoundingBoxEntity
    {
        Vector2 Velocity { get; }
        Vector2 Position { get; }

        Rectangle BoundingBox { get; }

        Vector2 BoundingTopLeft { get; }
        Vector2 BoundingCenter { get; }
    }
	public interface IBoundingSphereEntity
	{
		Vector2 Position {get;}
		Sphere BoundingSphere {get;}

		Vector2 Velocity {get;}
	}

    public interface ITileMap
    {
        bool IsCellSolid(int x, int y);

        int MapWidth { get; }
        int MapHeight { get; }
    }

    public struct Ray2D
    {
        public Ray2D(Vector2 origin, Vector2 direction)
        {
            Origin = origin;
            Direction = direction;
            Length = Direction.Length();
        }

        public Ray2D(Vector2 origin, Vector2 direction, float length)
        {
            Origin = origin;
            Direction = direction.Unit() * length;
            Length = length;
        }

        public Vector2 Origin;
        public Vector2 Direction;

        public float Length;

		public Vector2 UnitDirection => Direction.Unit();
    }

	public struct Sphere
	{
		public Vector2 Position;
		public float Radius;

	}
	

    public struct RaycastResult
    {

    }
    public static class CollisionService
    {

		//public 

		// Accurate And Fast DDA Algorithm
		// https://lodev.org/cgtutor/raycasting.html
        public static bool RayIntersectsTilemap(Ray2D ray, ITileMap tilemap)
        {
            Vector2 vRayUnitStepSize = new Vector2(
                (float)Math.Sqrt(1 + (ray.Direction.Y / ray.Direction.X) * (ray.Direction.Y / ray.Direction.X)),
                (float)Math.Sqrt(1 + (ray.Direction.X / ray.Direction.Y) * (ray.Direction.X / ray.Direction.Y)));


            Vector2 mapCheck = ray.Origin;
            mapCheck.Floor();

            float vRayLengthX = 0;
            float vRayLengthY = 0;

            int vStepX = 0, vStepY = 0;

            if (ray.Direction.X < 0)
            {
                vStepX = -1;
                vRayLengthX = (ray.Origin.X - mapCheck.X) * vRayUnitStepSize.X;
            }
            else
			{
				vStepX = 1;
				vRayLengthX = ((mapCheck.X + 1) - ray.Origin.X) * vRayUnitStepSize.X;
			}
                

            if (ray.Direction.Y < 0)
			{
				vStepY = -1;
				vRayLengthY = (ray.Origin.Y - mapCheck.Y) * vRayUnitStepSize.Y;
			}
            else
			{
				vStepY = 1;
				vRayLengthY = ((mapCheck.Y + 1) - ray.Origin.Y) * vRayUnitStepSize.Y;
			}


			bool tileFound = false;
			float maxDistance = 100.0f;
			float distance = 0.0f;
			while (!tileFound && distance < maxDistance)
			{
				// "walk" in shorter direction
				if (vRayLengthX < vRayLengthY)
				{
					mapCheck.X += vStepX;
					distance = vRayLengthX;
					vRayLengthX += vRayUnitStepSize.X;
				}
				else
				{
					mapCheck.Y += vStepY;
					distance = vRayLengthY;
					vRayLengthY += vRayUnitStepSize.Y;
				}
				int mapX = (int)mapCheck.X;
				int mapY = (int)mapCheck.Y;

				// TODO: add bounds to make sure we dont exceed 2d array boundaries

				if (tilemap.IsCellSolid(mapX, mapY))
				{
					tileFound = true;
				}
			}
            Vector2 intersection;


			if (tileFound)
			{
				intersection = ray.Origin + (ray.UnitDirection * distance);
			}
            return true;

        }

        public static bool RayIntersectsRect(Ray2D ray, Rectangle rectangle, out RaycastResult result)
        {
            return true;
        }

        public static bool AABBOverlaps(Rectangle rect1, Rectangle rect2)
        {
            return false;
        }

        public static void SolveAABBEntities(IBoundingBoxEntity e1, IBoundingBoxEntity e2)
        {

        }

		public static void SolveSphereEntities(params IBoundingSphereEntity[] entities)
		{
			
		}

		public static bool SpheresOverlap(Sphere a, Sphere b)
		{
			var dist = Math.Abs(
				(a.Position.X-b.Position.X)*(a.Position.X-b.Position.X)
				+ (a.Position.Y-b.Position.Y)*(a.Position.Y-b.Position.Y));
			var radii = a.Radius + b.Radius;

			return dist < radii;
		}
    }
}