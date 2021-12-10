using System;
using Conarium.Datatypes;
using Conarium.Extension;
using Conarium;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Wasteland.Common.Entities
{
	// Vehicle decoration/obstacles
	// leave static? (i.e non-movable)
	public class Vehicle : Entity
	{
		public override Vector2 BoundingBox => new Vector2(10, 20);
		// wheel related junk
		public float MaxTurnRadius {get;set;}
		public float WheelDirection {get;set;}

		public float FrontAxleCenterOffset {get;set;}
		public float FrontAxleLength {get;set;}
		public float RearAxleCenterOffset {get;set;}
		public float RearAxleLength {get;set;}

		public Vehicle(Guid networkUUID) : base(networkUUID)
		{

		}

		public override void Draw()
		{

			
			Vector2 wheelDimensions = new Vector2(4, 8);
			var gfx = GraphicsService.Get();
			// draw car wheels

			var reference = new Transformation{
				Position = this.Position,
				Rotation = this.Direction.Degrees,
				Scale = new Vector2(1, 1)
			};
			

			// front right wheel
			/*{
				var travelLength = new Vector2(FrontAxleCenterOffset, -FrontAxleLength);
				var dirVec = Direction.ToUnitVector()*travelLength.Length();
				// calculate center of wheel
				var position = Position + dirVec;

				gfx.Rect(Color.Gray, position, wheelDimensions*2, Direction.RotateDeg(WheelDirection), new Vector2(0.5f, 0.5f));
			}*/

			// rear left wheel

			// rear right wheel



			// draw car body
			// TODO: draw texture
			gfx.Rect(Color.WhiteSmoke, Position, BoundingBox*2, Direction, new Vector2(0.5f, 0.5f));
			// front left wheel
			{
				var travelLength = new Vector2(FrontAxleCenterOffset, FrontAxleLength);
				var dirVec = Direction.ToUnitVector()*travelLength.Length();

				var transformed = reference.TransformVector(travelLength);

				// calculate center of wheel
				var position = Position + dirVec;

				gfx.Rect(Color.Gray, position, wheelDimensions*2, Direction + WheelDirection, new Vector2(0.5f, 0.5f));
			}
			base.Draw();
		}

		public override void Update(GameTime gt)
		{

			var inp = InputService.Get();

			 // TODO: add arithmetic operators to Rotation.cs
			if (inp.GetKey(Keys.Left))
				Direction += gt.GetDelta()/10f;
			if (inp.GetKey(Keys.Right))
				Direction -= gt.GetDelta()/10f;

			//WheelDirection = Math.Clamp(WheelDirection, -MaxTurnRadius, MaxTurnRadius);
			base.Update(gt);
		}
	}
}