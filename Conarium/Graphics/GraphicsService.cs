using System;
using System.Collections.Generic;
using System.IO;
using Conarium.Datatypes;
using Conarium.Extension;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Conarium.Graphics
{
	public enum TextXAlignment { Left, Center, Right }
	public enum TextYAlignment { Top, Center, Bottom }



    public class GraphicsService : Service
    {

        public static GraphicsService Get() => Instance;
		public static GraphicsService Instance {get; private set;}

        public bool LoadFonts { get; set; }

		public float GlobalAnimationTimer { get; private set; }

        private Texture2D pixel;


        public int TotalTextures { get; private set; }
        public int LoadedTextures { get; private set; }

        public bool FontsLoaded { get; set; }


        public GraphicsService(Game game) : base(game)
        {

            Instance = this;
            LoadingDelay = 0.03f;
            FontsLoaded = false;
            LoadFonts = true;
        }

        public override void Initialize()
        {
            SpriteBatch = new SpriteBatch(GraphicsDevice);

            Fonts = new FontManager();   
            

            // create pixel
            pixel = new Texture2D(GraphicsDevice, 1, 1);
            pixel.SetData<Color>(new Color[] { Color.White });

            FontsLoaded = true; // if
            if (LoadFonts)
                Fonts.LoadAssets("GameData", ContentManager);
        }
        FontManager fonts;
        public FontManager Fonts 
        {
            get 
            {
                if (!FontsLoaded)
                    throw new Exception("Tried to qualify font before fonts have been loaded! Check your initialization methods.");
                return fonts;
            }
            private set { fonts = value;}
        }

        public void UpdateWindowSize(Vector2 newSize) {
            WindowSize = new Vector2(
            GraphicsDeviceManager.PreferredBackBufferWidth, 
            GraphicsDeviceManager.PreferredBackBufferHeight);
        }

        public Vector2 WindowSize {get;set;} //=> 

        public SpriteSortMode SpriteSortMode { get; set; }
        public BlendState BlendState { get; set; }
        public SamplerState SamplerState { get; set; }
        public DepthStencilState DepthStencilState { get; set; }
        public RasterizerState RasterizerState { get; set; }
        public Effect Shader { get; set; }
        public Matrix Matrix { get; set; }

        public ContentManager ContentManager { get; set; }
        public SpriteBatch SpriteBatch { get; set; }
        public GraphicsDevice GraphicsDevice { get; set; }
        public GraphicsDeviceManager GraphicsDeviceManager { get; set; }

        public bool ContentLoaded { get; private set; }

        public float GraphicsTimer { get; set; }

        public float LoadingDelay { get; set; }


        public override void Update(GameTime gt)
        {
			// increase global anim time.
			// used by items, entities, and tiles when an internal state is otherwise not ideal.
			GlobalAnimationTimer += gt.GetDelta();
            base.Update(gt);
        }


        public void Clear(Color color) => GraphicsDevice.Clear(color);
        public void End() => SpriteBatch.End();
        public void Begin(SpriteSortMode sorting = SpriteSortMode.Deferred, BlendState blending = null, SamplerState sampling = null, DepthStencilState depthStencil = null, 
            RasterizerState rasterizing = null, Effect effect = null, Matrix? transform = null)
        {
            SpriteBatch.Begin(sorting, blending, sampling, depthStencil, rasterizing, effect, transform);
        }
        public void Arc(Color color, Vector2 center, float radius, int sides, Rotation startingAngle, Rotation radians, float thickness = 1)
        {
            List<Vector2> arc = PrimitiveShapeFactory.GetArc(radius, sides, startingAngle.Radians, radians.Radians);
            Polygon(color, center, arc, thickness);
        }
        public void Circle(Color color, Vector2 position, double radius, int sides = 12, float thickness = 1)
        {
            List<Vector2> c = PrimitiveShapeFactory.GetCircle(radius, sides);
            Polygon(color, position, c, thickness);
        }
		
        public void Line(Color color, Vector2 point, float length, Rotation angle, float thickness = 1)
        {
            Vector2 origin = new Vector2(0f, 0.5f);
            Vector2 scale = new Vector2(length, thickness);
            SpriteBatch.Draw(pixel, point, null, color, angle.Radians, origin, scale, SpriteEffects.None, 0);
        }
        public void Line(Color color, Vector2 point1, Vector2 point2, float thickness = 1)
        {
            float distance = Vector2.Distance(point1, point2);
            float angle = (float)Math.Atan2(point2.Y - point1.Y, point2.X - point1.X);
            float expanded = (float)Math.Floor(angle * Math.PI);
            float backDown = expanded / (float)Math.PI;

            Line(color, point1, distance, new Rotation(angle), thickness);
        }
        public void OutlineRect(Color color, Vector2 position, Vector2 size, float thickness = 2.0f)
        {
            Line(color, position, position + new Vector2(0, size.Y), thickness);
            Line(color, position, position + new Vector2(size.X, 0), thickness);
            Line(color, position + new Vector2(size.X, 0), position + size, thickness);
            Line(color, position + new Vector2(0, size.Y), position + new Vector2(size.X, size.Y), thickness);
        }
        public void Polygon(Color color, Vector2 position, List<Vector2> points, float thickness = 1)
        {
            if (points.Count < 2)
                return;

            for (int i = 1; i < points.Count; i++)
                Line(color, points[i - 1] + position, points[i] + position, thickness);
        }
        public void Polygon(Color color, List<Vector2> points, float thickness = 1)
        {
            if (points.Count < 2)
                return;

            for (int i = 1; i < points.Count; i++)
                Line(color, points[i - 1], points[i], thickness);
        }
        public void Rect(Color color, Vector2 position, Vector2 size) => Rect(color, position, size, Rotation.Zero);
        public void Rect(Color color, Vector2 position, Vector2 size, Rotation rotation)
        {
            SpriteBatch.Draw(
                pixel,
                new Rectangle(position.ToPoint(), size.ToPoint()),
                null,
                color, rotation.Degrees, new Vector2(0, 0), SpriteEffects.None, 0
            );
        }
		public void Rect(Color color, Vector2 position, Vector2 size, Rotation rotation, Vector2 origin)
        {
            SpriteBatch.Draw(
                pixel,
                new Rectangle(position.ToPoint(), size.ToPoint()),
                null,
                color, rotation.Degrees, origin, SpriteEffects.None, 0
            );
        }
        public void Sprite(Texture2D texture, Vector2 position) => Sprite(texture, position, Color.White);
        public void Sprite(Texture2D texture, Vector2 position, Color color) => SpriteBatch.Draw(texture, position, color);
        public void Sprite(Texture2D texture, Vector2 position, Rectangle? quad, Color color) => SpriteBatch.Draw(texture, position, quad, color);
        public void Sprite(Texture2D texture, Vector2 position, Rectangle? quad, Color color, Rotation rotation, Vector2 origin, Vector2 scale, SpriteEffects efx, float layer) => 
            SpriteBatch.Draw(texture,position,quad,color,rotation.Radians, origin,scale,efx,layer);
        public void Sprite(Texture2D texture, Vector2 position, Rectangle? quad, Color color, Rotation rotation, Vector2 origin, float scale, SpriteEffects efx, float layer) => 
            SpriteBatch.Draw(texture, position, quad, color, rotation.Radians, origin, scale, efx, layer);
        public void Text(string text, Vector2 position) => Text(Fonts.Arial10, text, position);
        public void Text(string text, Vector2 position, Color color) => SpriteBatch.DrawString(Fonts.Arial10, text, position, color);
        public void Text(SpriteFont font, string text, Vector2 position) => Text(font, text, position, Color.White, TextXAlignment.Left, TextYAlignment.Top);
        public void Text(SpriteFont font, string text, Vector2 position, Color color, TextXAlignment textX = TextXAlignment.Left, TextYAlignment textY = TextYAlignment.Top)
        {
            float xoffset = 0;
            float yoffset = 0;

            /*
            Exception has occurred: CLR/System.NullReferenceException
            An unhandled exception of type 'System.NullReferenceException' occurred in MyProject.dll: 'Object reference not set to an instance of an object.'
            at Conarium.GraphicsEngine.Text(SpriteFont font, String text, Vector2 position, Color color, TextXAlignment textX, TextYAlignment textY) in 
            /home/joosh/Projects/Project1/MyProject/Conarium/GraphicsEngine.cs:line 351
            at Conarium.GraphicsEngine.Text(SpriteFont font, String text, Vector2 position) in /home/joosh/Projects/Project1/MyProject/Conarium/GraphicsEngine.cs:line 343
            at Conarium.GraphicsEngine.Text(String text, Vector2 position) in /home/joosh/Projects/Project1/MyProject/Conarium/GraphicsEngine.cs:line 341
            at MyProject.Game1.Draw(GameTime gameTime) in /home/joosh/Projects/Project1/MyProject/Game1.cs:line 146
            at Microsoft.Xna.Framework.Game.DoDraw(GameTime gameTime)
            at Microsoft.Xna.Framework.Game.Tick()
            at Microsoft.Xna.Framework.SdlGamePlatform.RunLoop()
            at Microsoft.Xna.Framework.Game.Run(GameRunBehavior runBehavior)
            at MyProject.Program.Main() in /home/joosh/Projects/Project1/MyProject/Program.cs:line 11
            */

            Vector2 bounds = font.MeasureString(text);

            if (textX == TextXAlignment.Center)
                xoffset = bounds.X / 2;
            if (textX == TextXAlignment.Right)
                xoffset = bounds.X;

            if (textY == TextYAlignment.Center)
                yoffset = bounds.Y / 2;
            if (textY == TextYAlignment.Bottom)
                yoffset = bounds.Y;

            SpriteBatch.DrawString(font, text, position-new Vector2(xoffset, yoffset), color);
        }
        /*public void Line3D(Camera camera, Vector3 pointA, Vector3 pointB, Color color)
		{
			Line3D(camera, pointA, pointB, color, color);
		}
		public void Line3D(Camera camera, Vector3 pointA, Vector3 pointB, Color colorA, Color colorB)
		{
			Line3D(camera, pointA, pointB, colorA, colorB);
		}
		public void Line3D(Camera camera, Vector3 pointA, Vector3 pointB, Color color)
		{
			Line3D(camera, pointA, pointB, color, color);
		}
		public void Line3D(Camera camera, Vector3 pointA, Vector3 pointB, Color colorA, Color colorB)
		{
			_effect3D.View = camera.View;
			_effect3D.Projection = camera.Projection;
			_effect3D.CurrentTechnique.Passes[0].Apply();
			var vertices = new[] { new VertexPositionColor(pointA, colorA), new VertexPositionColor(pointB, colorB)};
			GraphicsDevice.DrawUserPrimitives(PrimitiveType.LineList, vertices, 0, 1);
		}*/


		/// <summary>
		/// Saves a screencap of the game window as a .png file
		/// </summary>
		/// <param name="filename">Path to save file as, defaults to a standard DateTime format.</param>
        public void TakeScreenshot(string filename = "")
		{	
			// create folder if nonexistant
			Directory.CreateDirectory("Screenshots");

			// set default filename
			if (string.IsNullOrEmpty(filename))
				filename = Path.Combine("Screenshots", DateTime.Now.ToFileTime()+".png");

			// create pixel buffer that matches window size Width*Height
			int Width = GraphicsDevice.Viewport.Width;
			int Height = GraphicsDevice.Viewport.Height;
			Color[] captureBuffer = new Color[Width * Height];

			// copy screen pixel-for-pixel into color(pixel) buffer;
			GraphicsDevice.GetBackBufferData<Color>(captureBuffer);

			// create temporary Texture2D
			using (Texture2D tex2D = new Texture2D(GraphicsDevice, Width, Height))
			{
				// push buffer data into texture
				tex2D.SetData<Color>(captureBuffer);

				// monogame api has method to save Texture2D as PNG :D
				using (FileStream stream = File.Create(filename))
					tex2D.SaveAsPng(stream, Width, Height);
			}
		}

    }
}