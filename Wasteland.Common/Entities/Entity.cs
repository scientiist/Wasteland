using System;
using Conarium.Datatypes;
using Conarium;
using Microsoft.Xna.Framework;

namespace Wasteland.Common
{

	public enum EntityType
	{
		Player,
		PeerPlayer,
		Zombie,
		MutatedZombie,
		ZombieNoLegs,

	}

    public interface INetworkEntity
    {
        Guid EntityUUID {get;}
        bool ServerAuthoritative {get;} // whether or not this entity is controlled locally
        // this will more or less only apply to the local player entity, afaik
    }
    // implements physics related properties and methods
    public interface IPhysicsEntity
    {
        Vector2 Position {get;set;}
        Vector2 NextPosition {get;set;}
        Vector2 Velocity {get;set;}
        Vector2 Friction {get;set;}
        float Mass {get;}
    }


    // entities w/ this interface will run server-sided physics
    public interface IServerPhysicsObserver : IPhysicsEntity
    {
        void ServerPhysicsTick(float ticc);
    }
    // client side for this one
    public interface IClientPhysicsObserver : IPhysicsEntity
    {
        void ClientPhysicsTick(float ticc);
    }

    public abstract class Entity : INetworkEntity
    {
		public int Health {get;set;}
		public int MaxHealth {get;set;}

        public Guid EntityUUID {get; set;}
        public virtual bool ServerAuthoritative {get;set;}

        public Vector2 Position {get; set;}
        public virtual Vector2 BoundingBox {get;}
        public Rotation Direction {get; set;}

        public float Age {get; private set;}
        public bool Dead {get;set;}


		public Entity(Guid networkUUID)
		{
			EntityUUID = networkUUID;
			Direction = Rotation.Zero;
		}

        public virtual void Update(GameTime gt)
        {

        }


        public virtual void Draw()
        {
			var gfx = GraphicsService.Get();


			gfx.Circle(Color.Red, Position, 8, 16, 1);
			gfx.Line(Color.White, Position, 24, Direction, 2);
        }

    }

    public abstract class PhysicsEntity : Entity, IPhysicsEntity
    {
		public PhysicsEntity(Guid networkUUID) : base(networkUUID)
		{

		}

		public abstract Vector2 NextPosition { get; set; }
        public abstract Vector2 Velocity { get; set; }
        public abstract Vector2 Friction { get; set; }
        public abstract float Mass { get; }

    }
}