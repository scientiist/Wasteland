using System;
using Conarium.Graphics;
using Conarium.UI;
using Microsoft.Xna.Framework;

namespace Wasteland.Client.Menus
{

	public interface IWindowRunner
	{
		bool Open {get;set;}
	}
    public class SettingsWindow : GameComponent, IWindowRunner
    {
		public bool Open {get;set;}

		public WindowNode Panel;

		public SettingsWindow(Game game) : base(game)
		{
			Panel = new WindowNode("Settings", 500, 400);
		}


		public override void Update(GameTime gameTime)
		{
			base.Update(gameTime);
		}

		public void Draw()
		{
			var GFX = GraphicsService.Get();
			//GFX.
		}

		public override void Initialize()
		{
			
		}
	}
}
