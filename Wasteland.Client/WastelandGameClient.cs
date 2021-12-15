using System.Threading.Tasks;
using System;
using System.IO;
using Conarium;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using TiledCS;
using Wasteland.Client.Menus;
using Wasteland.Server;
using Wasteland.Common;

namespace Wasteland.Client
{

	// NOTE: GameClient refers to actual instances of game sessions.
	//	
	//
	//
	/// <summary>
	/// Base Game Class implementation for Wasteland.
	/// </summary>
	public class WastelandGameClient : Game
	{
		public readonly static string CurrentVersion = "0.1";


		#region Conarium Services
		// Conarium Engine Services
		// Singleton instances that provide core functionality
		public AssetService AssetService { get; private set; }
		public GraphicsService GraphicsService { get; private set; }
		public InputService InputService { get; private set; }
		#endregion

		public Settings Settings			{ get; init; } // why isn't init highlighted?
		public GameClient SessionInstance 	{ get; init; }
		public Camera2D Camera				{ get; init; }
		public FPSTracker FPSTracker 		{ get; private set; }
		public GameConsole GameConsole		{ get; private set; }
		public SettingsWindow SettingsMenu	{ get; private set; }

		// TODO: contexts need transition effects
		// then Splash can be a context :)
		protected Splash Splash;
		private GraphicsDeviceManager GraphicsDeviceManager;

		#region Context Manager
		/// <summary>
		/// Current game logic context. 
		/// Contexts are mutually-exclusive gamestates that run relevant logic.
		/// Updating the property automatically calls 
		/// Unload() and Dispose() on the old context,
		/// and runs Load() on the new context.
		/// </summary>
		/// <value></value>
		public IContext CurrentContext
		{
			get => currContext;
			set
			{
				currContext?.Unload();
				currContext?.Dispose();
				currContext = value;
				currContext.Running = true;
				currContext.Load();
			}
		}
		private IContext currContext;
		#endregion


		// Window interfaces. can have multiple open simultaneously
		// and usually at any point during the game. (i.e non-contextual)


		public WastelandGameClient()
		{


			Content.RootDirectory = "Content";
			IsMouseVisible = true;

			Window.AllowUserResizing = true;
			Window.AllowAltF4 = true;

			GraphicsDeviceManager = new GraphicsDeviceManager(this)
			{
				PreferredBackBufferWidth = 1280,
				PreferredBackBufferHeight = 720,
				SynchronizeWithVerticalRetrace = false,
				IsFullScreen = false,
				PreferredDepthStencilFormat = DepthFormat.Depth24Stencil8
			};

			Camera = new Camera2D(this);


			Splash = new Splash(this);

			Settings = Settings.LoadSettings(this, "settings.xml");

			// TODO: Load settings
			SetFullscreen(false);
			SetVSync(false);
			GraphicsDeviceManager.ApplyChanges();
		}


		protected virtual void LoadAndApplySettings()
		{

		}

		public void ConnectToServer(string address)
		{
			HeadlessServer server = new HeadlessServer();
			server.Logger = GameConsole;
			Task.Factory.StartNew(server.Start);
			var client = new GameClient(this);
			client.OnShutdown += server.Shutdown;
			CurrentContext = client;
			client.Connect(address);
		}

		#region Settings-Related Methods




		// Update graphics engine's known window size
		void Window_ClientSizeChanged(object sender, EventArgs e)
		{
			GraphicsService.UpdateWindowSize(Window.ClientBounds.Size.ToVector2());
			Console.WriteLine(Window.ClientBounds.Size.ToVector2());
		}

		public void SetFullscreen(bool full)
		{

			this.GraphicsDeviceManager.IsFullScreen = full;
			if (full == true)
			{

				GraphicsDeviceManager.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
				GraphicsDeviceManager.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
			}
			else
			{
				GraphicsDeviceManager.PreferredBackBufferWidth = 1920;
				GraphicsDeviceManager.PreferredBackBufferHeight = 1080;
			}
			GraphicsDeviceManager.ApplyChanges();
		}


		#endregion


		public virtual void SetResolution(int width, int height)
		{
			GraphicsDeviceManager.PreferredBackBufferWidth = width;
			GraphicsDeviceManager.PreferredBackBufferHeight = height;
			GraphicsDeviceManager.ApplyChanges();
		}

		protected virtual void CreateServices()
		{
			GraphicsService = new GraphicsService(this)
			{
				ContentManager = Content,
				GraphicsDevice = GraphicsDevice,
				GraphicsDeviceManager = GraphicsDeviceManager
			};
			Components.Add(GraphicsService);
			GraphicsService.Initialize();
			GraphicsService.UpdateWindowSize(new Vector2(
				GraphicsDeviceManager.PreferredBackBufferWidth,
				GraphicsDeviceManager.PreferredBackBufferHeight
			));

			AssetService = new AssetService(this);
			Components.Add(AssetService);
			AssetService.Initialize();

			InputService = new InputService(this);
			Components.Add(InputService);
			InputService.Initialize();

			FPSTracker = new FPSTracker(this);
			Components.Add(FPSTracker);

			GameConsole = new GameConsole(this);

			GameConsole.BindCommand(new Common.Command("serv_start"));
			GameConsole.BindCommand(new Command("serv_stop"));

			Components.Add(GameConsole);


			SettingsMenu = new SettingsWindow(this);
			Components.Add(SettingsMenu);
		}



		protected override void Initialize()
		{

			CreateServices();

			// initalize to main menu
			CurrentContext = new MainMenu(this);

			// bind callback to listen for window resize
			Window.ClientSizeChanged += new EventHandler<EventArgs>(Window_ClientSizeChanged);

			base.Initialize();
		}

		TiledCS.TiledMap TestMap;
		protected override void LoadContent()
		{
			Splash.Load();
			Console.WriteLine(Path.GetFullPath("GameData/devmap.tmx"));
			TestMap = new TiledMap("Wasteland.Assets/devmap.tmx");

		}
		protected override void Update(GameTime gameTime)
		{
			if (InputService.KeyPressed(Keys.F2))
				GraphicsService.TakeScreenshot();

			if (InputService.KeyPressed(Keys.F3))
				SetFullscreen(!GraphicsDeviceManager.IsFullScreen);

			if (CurrentContext != null && CurrentContext.Running)
				CurrentContext?.Update(gameTime);

			if (InputService.KeyPressed(Keys.Escape) || InputService.ButtonPressed(Buttons.X))
				Exit();

			Mouse.SetCursor(MouseCursor.Arrow);

			if (Settings.Keybindings.OpenGameConsole.Pressed())
			{
				GameConsole.Open = !GameConsole.Open;
			}


			Camera.Update(gameTime);

			// splash is specially rendered so it can be drawn over top shit
			Splash?.Update(gameTime);
			if (Splash != null && Splash.SplashTimer < 0)
			{
				Splash.Unload();
				Splash = null;
			}

			base.Update(gameTime);
		}

		int index = 0;
		private void DebugMessage(string data)
		{
			GraphicsService.Text(data, new Vector2(4, 4 + (14 * index)));
			index++;
		}

		private void DrawDebuggingStats(GameTime gt)
		{
			index = 0;
			GraphicsService.Begin();
			DebugMessage(String.Format("fps: {0} inst: {1} var: {2}ms",
				Math.Round(FPSTracker.AverageFramerate, 1),
				Math.Round(FPSTracker.ExactFramerate, 1),
				Math.Round(FPSTracker.FrameVariation * 1000, 2)
			));
			DebugMessage(String.Format("screen: {0} {1} vsync: {2}",
				GraphicsDeviceManager.PreferredBackBufferWidth,
				GraphicsDeviceManager.PreferredBackBufferHeight,
				GraphicsDeviceManager.SynchronizeWithVerticalRetrace
			));
			DebugMessage(String.Format("context {0}",
				CurrentContext?.GetType().Name
			));

			GraphicsService.End();
		}

		protected override void Draw(GameTime gameTime)
		{
			GraphicsDevice.Clear(Color.Black);
			//-------------------------------

			if (CurrentContext != null && CurrentContext.Running)
				CurrentContext.Draw();


			Splash?.Draw();


			// Components aren't drawn automatically
			// so we need to draw the GameConsole ourselves out here
			GameConsole.Draw();
			DrawDebuggingStats(gameTime);
			//--------------------------
			base.Draw(gameTime);
		}

		public void SetVSync(bool vsync)
		{
			this.GraphicsDeviceManager.SynchronizeWithVerticalRetrace = vsync;
		}

		private void DrawLoadingBar()
		{
			var GFX = GraphicsService;

			if (!GFX.FontsLoaded)
				return;

			float frac = (float)GFX.LoadedTextures / (float)GFX.TotalTextures;
			string text = String.Format("Loading: {0} of {1} ({2}%)", GFX.LoadedTextures, GFX.TotalTextures, (int)(frac * 100));
			GFX.GraphicsDevice.Clear(Color.Black);
			GFX.Begin();
			GFX.Text(GFX.Fonts.Arial20, text, GFX.WindowSize / 2.0f, Color.White, TextXAlignment.Center, TextYAlignment.Center);

			float barY = (GFX.WindowSize.Y / 2.0f) + 20.0f;
			float barX = GFX.WindowSize.X / 2.0f;
			float barLength = GFX.WindowSize.X / 3.0f;
			float barHeight = 10.0f;

			Vector2 center = new Vector2(barX, barY);

			GFX.Rect(
				Color.Gray,
				center - new Vector2(barLength / 2.0f, 0),
				new Vector2(barLength, barHeight)
			);
			GFX.Rect(
				Color.White,
				center - new Vector2(barLength / 2.0f, 0),
				new Vector2(barLength * frac, barHeight)
			);


			GFX.End();
		}
	}
}