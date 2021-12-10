using System.Threading.Tasks;
using System;
using System.IO;
using Conarium;
using Conarium.Services;
using Conarium.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using TiledCS;
using Wasteland.Client.Menus;
using Wasteland.Server;

namespace Wasteland.Client
{
	public interface IGameClient
	{
		IContext CurrentContext {get;set;}
		Game GameReference {get;}

		void ConnectToServer(string address);
	}

    public class WastelandClient : Game, IGameClient
    {
		#region Game-centric metadata
		public readonly static string CurrentVersion = "0.1";

		#endregion

		public Game GameReference => this;
        

        #region Conarium Services
        public Conarium.Services.AssetService    AssetService    {get; private set;}
        public Conarium.Services.GraphicsService GraphicsService {get; private set;}
        public Conarium.Services.InputService    InputService    {get; private set;}

		public Settings Configuration {get;set;}
        
		public GameClient SessionInstance {get; private set;}
		#endregion

        private GraphicsDeviceManager GraphicsDeviceManager;
		#region Game Objects
        public GameConsole GameConsole {get; set;}
        public Camera2D Camera {get;set;}
        //public Splash Splash {get; private set;}
        public FPSTracker FPSTracker {get; private set;}
		#endregion
        Vector2 CanvasResolution {get; set;}

		public IContext CurrentContext 
		{
			get => currContext;
			set {
				currContext?.Unload();
				currContext?.Dispose();
				currContext = value;
				currContext.Running = true;
				currContext.Load();
			}
		}
		private IContext currContext;
		
		Splash Splash;
        //MainMenu MainMenu;
        WindowNode SettingsMenu;

		public WastelandClient()
        {
            
            Content.RootDirectory = "Content";
            IsMouseVisible = true;

            Window.AllowUserResizing = true;
            Window.AllowAltF4 = true;

            GraphicsDeviceManager = new GraphicsDeviceManager(this) {
                PreferredBackBufferWidth = 1920,
                PreferredBackBufferHeight = 1080,
                SynchronizeWithVerticalRetrace = false,
                IsFullScreen = false,
                PreferredDepthStencilFormat = DepthFormat.Depth24Stencil8
            };

            CanvasResolution = new Vector2(1920, 1080);

            
            Camera = new Camera2D(this);
            

			Splash = new Splash(this);
			Configuration = Settings.Load<Settings>("settings.xml", true);
            SetFullscreen(false);
            SetVSync(false);
            GraphicsDeviceManager.ApplyChanges();
		}

		public void ConnectToServer(string address)
		{
			HeadlessServer server = new HeadlessServer();
			server.Logger = GameConsole; 
			Task.Factory.StartNew(server.Start);
			var client = new GameClient(this);
			//TODO: client.OnShutdown += server.Shutdown;
			CurrentContext = client;
			client.Connect(address);
		}

		#region Settings-Related Methods
        public void TakeScreenshot(string filename = "")
		{
			Color[] colors = new Color[GraphicsDevice.Viewport.Width * GraphicsDevice.Viewport.Height];

			GraphicsDevice.GetBackBufferData<Color>(colors);

			using (Texture2D tex2D = new Texture2D(GraphicsDevice, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height))
			{
				Directory.CreateDirectory("Screenshots");
				tex2D.SetData<Color>(colors);
				if (string.IsNullOrEmpty(filename))
				{
					filename = Path.Combine("Screenshots", DateTime.Now.ToFileTime()+".png");
				}
				using (FileStream stream = File.Create(filename))
				{
					tex2D.SaveAsPng(stream, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height);
				}
			}
		}

        // Update graphics engine's known window size
		void Window_ClientSizeChanged(object sender, EventArgs e) {
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
                CanvasResolution = new Vector2(1920, 1080);
			}
			GraphicsDeviceManager.ApplyChanges();
		}


		#endregion
        

        public virtual void SetResolution(int width, int height)
        {
            GraphicsDeviceManager.PreferredBackBufferWidth = width;
			GraphicsDeviceManager.PreferredBackBufferHeight = height;
            //GraphicsService?.UpdateWindowSize(new Vector2(width, height));
			CanvasResolution = new Vector2(width, height);
			GraphicsDeviceManager.ApplyChanges();
        }

        protected virtual void CreateServices()
        {
            GraphicsService = new GraphicsService(this){
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
            Components.Add(GameConsole);
        }

        protected override void Initialize()
        {

            CreateServices();

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




			var state = GamePad.GetState(0);
			Console.WriteLine(state);
			
            
			if (CurrentContext!=null && CurrentContext.Running)
				CurrentContext?.Update(gameTime);

            if (InputService.KeyPressed(Keys.Escape) || InputService.ButtonPressed(Buttons.X))
                Exit();

            Mouse.SetCursor(MouseCursor.Arrow);

            if (Settings.Get().Keybindings.OpenGameConsole.IsDown())
            {
                Console.WriteLine("Fucking hell");
                GameConsole.Open = !GameConsole.Open;
            }
                

            Camera.Update(gameTime);

			// splash is specially rendered so it can be drawn over top shit
			Splash?.Update(gameTime);
			if (Splash != null && Splash.SplashTimer < 0)
			{
				Console.WriteLine("Bye bye Splash");
				Splash.Unload();
				Splash = null;
			}

            base.Update(gameTime);
        }

        int index = 0;
        private void DebugMessage(string data)
        {
            GraphicsService.Text(data, new Vector2(4, 4+(14*index) ));
            index++;
        }

        private void DrawDebuggingStats(GameTime gt)
        {
            index = 0;
            GraphicsService.Begin();
            DebugMessage(String.Format("fps: {0} inst: {1} var: {2}ms", 
                Math.Round(FPSTracker.AverageFramerate, 1),
                Math.Round(FPSTracker.ExactFramerate, 1),
                Math.Round(FPSTracker.FrameVariation*1000, 2)
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
			
			if (CurrentContext!=null && CurrentContext.Running)
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
			string text = String.Format("Loading: {0} of {1} ({2}%)", GFX.LoadedTextures, GFX.TotalTextures, (int)(frac*100));
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
				center - new Vector2(barLength/2.0f, 0), 
				new Vector2(barLength, barHeight)
			);
			GFX.Rect(
				Color.White,
				center - new Vector2(barLength / 2.0f, 0),
				new Vector2(barLength*frac, barHeight)
			);
			
		
			GFX.End();
        }
    }
}