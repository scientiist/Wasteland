using System;
using System.Collections.Generic;
using Conarium;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Wasteland.Common
{
	public class Player : PhysicsEntity
	{
		public List<Player> Friendlies {get;set;}
		public bool PVPEnabled {get;set;}


		public Player(Guid networkUUID) : base(networkUUID)
		{
			Friendlies = new List<Player>();
		}

		public override Vector2 NextPosition {get;set;}
		public override Vector2 Velocity {get;set;}
		public override Vector2 Friction {get;set;}

		public override float Mass => 1;


		public override void Update(GameTime gt)
		{
			var Input = InputService.Get();


			if (Input.GetKey(Keys.W))

			base.Update(gt);
		}
	}
}