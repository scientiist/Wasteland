using System;
using System.Collections.Generic;
using Conarium.Datatypes;
using Microsoft.Xna.Framework;

namespace Wasteland.Common.Game
{
	public class Tile
	{

	}

    public class Gameworld
    {
		public List<Entity> Entities {get; private set;}
		
		public string Name {get; private set;}

		public Gameworld()
		{
			Entities = new List<Entity>();
		}


		public virtual void Update(GameTime gt)
		{
			foreach(var entity in Entities)
				entity.Update(gt);
		}
		public virtual void Draw()
		{
			foreach(var entity in Entities)
				entity.Draw();
		}
    }
}
