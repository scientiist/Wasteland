using System;
using Conarium.Datatypes;
using Conarium.Extension;
using Conarium.Services;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Wasteland.Client
{
    public class Splash : Context
    {
		public bool SplashActive => (SplashTimer > 0);
		public float SplashTimer { get; set; }

        public Splash(IGameClient client) : base(client)
        {
			SplashTimer = 3;
        }

		Texture2D splash;


		// called when game is ready to load data
		public override void Load()
		{
			splash = AssetService.Get().LoadTexture("Wasteland.Assets/csoft_splash.png");
		}

        public override void Unload()
        {
            splash.Dispose();
			splash = null;
        }

        public override void Update(GameTime gt)
        {
			SplashTimer -= gt.GetDelta();


			//if (SplashTimer < 0)
			//{
				//GameClient.CurrentContext = new MainMenu(GameClient);
			//}
        }

        public override void Draw()
        {

			var GFX = GraphicsService.Get();

			GFX.Begin(
				SpriteSortMode.Immediate, 
				BlendState.AlphaBlend, 
				SamplerState.PointClamp);


			Vector2 center = new Vector2(
				GFX.WindowSize.X / 2.0f, 
				GFX.WindowSize.Y / 2.0f);

			Vector2 origin = new Vector2(splash.Width / 2.0f, splash.Height / 2.0f);
			var scale = center/origin;

			float fadeOut = Math.Clamp(SplashTimer, 0, 1);

			Color drawColor = new Color(1, 1, 1, fadeOut);
			GFX.Sprite(splash, center, null, drawColor, Rotation.Zero, origin, scale, SpriteEffects.None, 1);

			GFX.End();
		}

        public override void Dispose() { }
    }
}