using System;
using Microsoft.Xna.Framework;

namespace Wasteland.Common.Entities
{
	public class Zombie : PhysicsEntity
	{
		public Zombie(Guid networkUUID) : base(networkUUID)
		{
		}

		public override Vector2 NextPosition { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
		public override Vector2 Velocity { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
		public override Vector2 Friction { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }

		public override float Mass => throw new System.NotImplementedException();
	}

	public class MutatedZombie : Zombie
	{
		public MutatedZombie(Guid networkUUID) : base(networkUUID)
		{
		}

	}

	public class ZombieNoLegs : Zombie
	{
		public ZombieNoLegs(Guid networkUUID) : base(networkUUID)
		{
		}
	}


}