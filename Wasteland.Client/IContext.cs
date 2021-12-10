using System;
using Microsoft.Xna.Framework;

namespace Wasteland.Client
{
	// Defines a game-context

    public interface IContext : IDisposable
    {
		public WastelandClient GameClient {get; set;}
		bool Running {get;set;}
		void Update(GameTime gt);
		void Draw();

		void Load();
		void Unload();
    }

    public abstract class Context : IContext
    {

		public Context(WastelandClient client)
		{
			GameClient = client;
		}
        public WastelandClient GameClient {get;set;}
        public bool Running {get;set;}

        public virtual void Dispose() { }
        public virtual void Draw() { }
        public virtual void Load() { }
        public virtual void Unload() { }
        public virtual void Update(GameTime gt) { }
    }
}
