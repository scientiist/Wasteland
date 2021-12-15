using System.IO;
using Conarium;
using Conarium.Datatypes;
using Conarium.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Wasteland.Common;

namespace Wasteland.Client.Menus
{
	public class MainMenu : Context
	{

		public SceneNode Scene {get;set;}
		public ListNode ButtonList {get;set;}

		Texture2D TitleTexture;

		ButtonNode MakeMenuButton(string name, string text)
		{
			ButtonNode node = new ButtonNode(name)
			{
				Size = new UIVector(0, 35, 1, 0),
				Parent = ButtonList,
			};
			var subtext = new TextNode("subtext")
			{
				Text = text,
				XAlignment = TextXAlignment.Center,
				YAlignment = TextYAlignment.Center,
				TextColor = Color.Black,
				Parent = node,
			};
			return node;
		}
		ButtonNode MakeImageButton(string name, Texture2D texture)
		{
			ButtonNode node = new ButtonNode(name)
			{
				Size = new UIVector(0, 35, 1, 0),
				Parent = ButtonList,
			};
			var image = new ImageNode("subtext")
			{
				Image = texture,
				Parent = node,
			};
			return node;
		}

		private void BuildMenu()
		{
			var background = new RectNode("background")
			{
				Size = UIVector.FromScale(1,1),
				Position = UIVector.FromScale(0,0),
				RectColor = Color.White,
				Parent = Scene,
			};
			var buttonBox = new RectNode("buttonbox")
			{
				Size = UIVector.FromScale(0.175f, 0.7f),
				Position = UIVector.FromScale(0.1f, 0.1f),
				Parent = background,
				RectColor = new Color(0.8f, 0.8f, 0.8f),
			};
			ButtonList = new ListNode("buttonlist")
			{
				Parent = buttonBox,
			};

			var PlaySingleplayerButton = MakeMenuButton("play-local", "Play Singleplayer");

			PlaySingleplayerButton.OnPressed += (s,e) => 
				GameClient.CurrentContext = new SingleplayerMenu(GameClient);


			var PlaySplitscreenButton = MakeMenuButton("play-splits", "Play Local Multiplayer");
			var PlayOnlineButton = MakeMenuButton("play-online", "Play Online");
			PlayOnlineButton.OnPressed += (s,e) => GameClient.ConnectToServer("localhost:42069");
				//GameClient.CurrentContext = new OnlineMenu(GameClient);

			

			var ConfigurationButton = MakeMenuButton("configuration", "Configuration");
			var HelpButton = MakeMenuButton("help", "Help");
			var ExitButton = MakeMenuButton("exit", "Exit Game");
			ExitButton.OnPressed += (s,e) => GameClient.Exit();

			var SteamWorkshopIconButton = MakeMenuButton("exit", "Steam Store Page");
			var ItchIconButton = MakeMenuButton("exit", "Itch.io Homepage");
			var DonateCryptoIconButton = MakeMenuButton("exit", "Donate via Bitcoin");

			

			var bottomBox = new RectNode("bottomBox")
			{
				Position = new UIVector(new Vector2(0,-20), new Vector2(0,1)),
				Size = new UIVector(new Vector2(0, 20), new Vector2(1, 0)),
				RectColor = Color.Black,
				Parent = background,
			};

			var versioning = new TextNode("versioning")
			{
				Font = GraphicsService.Get().Fonts.Arial10,
				YAlignment = TextYAlignment.Center,
				Text = $"Wasteland Online version 0.1 Internal Development Only, Not for display to the public!",
				TextColor = Color.White,
				Parent = bottomBox,
			};
			var copyright = new TextNode("copyright")
			{
				Font = GraphicsService.Get().Fonts.Arial10,
				YAlignment = TextYAlignment.Center,
				XAlignment = TextXAlignment.Right,
				Text = GameStrings.CopyrightNotice,
				TextColor = Color.White,
				Parent = bottomBox,
			};
		}



		public MainMenu(WastelandGameClient client) : base(client)
		{

			
		}

		

		public override void Load()
		{
			Scene = new SceneNode("root");
			TitleTexture = AssetService.Get().LoadTexture(Path.Combine("Wasteland.Assets", "title.png"));
			BuildMenu();
		}

		public override void Unload()
		{
			Scene = null;
		}

		public override void Draw()
		{
			Scene?.Draw();
			
			var Graphics = GraphicsService.Get();
			Graphics.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied, SamplerState.PointClamp, DepthStencilState.Default);
			var imageSize = new Vector2(TitleTexture.Width, TitleTexture.Height);
			var screenCenter = new Vector2(Graphics.WindowSize.X/2, 200);
			Graphics.Sprite(TitleTexture, screenCenter, null, Color.White, Rotation.Zero, imageSize/2, 8, SpriteEffects.None, 0);
			Graphics.End();
		}
		public override void Update(GameTime gt)
		{
			Scene?.Update(gt);
		}
	}
}