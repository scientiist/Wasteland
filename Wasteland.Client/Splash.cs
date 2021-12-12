using System;
using Conarium.Datatypes;
using Conarium.Extension;
using Conarium;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Wasteland.Client
{
    public class Splash : Context
    {
		public bool SplashActive => (SplashTimer > 0);
		public float SplashTimer { get; set; }

        public Splash(WastelandGameClient client) : base(client)
        {
			SplashTimer = 3;
        }

		Texture2D splash;
		Texture2D MonoGameLogo;
		Texture2D FedoraLogo;

		// called when game is ready to load data
		public override void Load()
		{
			splash = AssetService.Get().LoadTexture("Wasteland.Assets/csoft_splash.png");
			MonoGameLogo = AssetService.Get().LoadTexture("Wasteland.Assets/monogame_banner.png");
			FedoraLogo = AssetService.Get().LoadTexture("Wasteland.Assets/fedora_workstation_banner.png");
		}

        public override void Unload()
        {
            splash.Dispose();
			splash = null;
        }

        public override void Update(GameTime gt)
        {
			SplashTimer -= gt.GetDelta();

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
				(GFX.WindowSize.Y) / 2.0f);

			
			Vector2 SplashImgRealSize = new Vector2(3840, 2160);
			Vector2 origin = new Vector2(splash.Width / 2.0f, splash.Height / 2.0f);
			//new Vector2(SplashImgRealSize.X / 2.0f, SplashImgRealSize.Y / 2.0f);
			var scale = center/origin;

			float fadeOut = Math.Clamp(SplashTimer, 0, 1);
			
			Color drawColor = new Color(1, 1, 1, fadeOut);

			// draw conarium software logo from the center (ish)
			GFX.Sprite(
				texture: splash, 
				position: center, 
				quad: null, 
				color: drawColor, 
				rotation: Rotation.Zero, 
				origin: origin, 
				scale: scale, 
				efx: SpriteEffects.None, 
				layer: 1
			);


			// TODO: Make logo rendering "match" screen scale of main sprite

			// draw monogame logo at bottom left;

			var mgLogoScaleFrac = 0.5f;
			var mgLogoDimensions = new Vector2(MonoGameLogo.Width, MonoGameLogo.Height)/mgLogoScaleFrac;
			var bottomLeft = new Vector2(0, GFX.WindowSize.Y-mgLogoDimensions.Y);

			GFX.Sprite(
				texture: MonoGameLogo, 
				position: bottomLeft, 
				quad: null, 
				color: drawColor, 
				rotation: Rotation.Zero,
				origin: Vector2.Zero,
				scale: new Vector2(1/mgLogoScaleFrac, 1/mgLogoScaleFrac), 
				efx: SpriteEffects.None, 
				layer: 1
			);


			// draw fedora logo
			var fedoraLogoScaleFrac = 0.5f;
			var fedoraLogoDimensions = new Vector2(FedoraLogo.Width, FedoraLogo.Height)/fedoraLogoScaleFrac;
			var bottomRight = new Vector2(GFX.WindowSize.X-fedoraLogoDimensions.X, GFX.WindowSize.Y-fedoraLogoDimensions.Y);
			

			GFX.Sprite(
				texture: FedoraLogo,
				position: bottomRight,
				quad: null,
				color: drawColor,
				rotation: Rotation.Zero,
				origin: Vector2.Zero,
				scale: new Vector2(1/fedoraLogoScaleFrac, 1/fedoraLogoScaleFrac),
				efx: SpriteEffects.None,
				layer: 1
			);

			GFX.End();
		}

        public override void Dispose() { }
    }
}