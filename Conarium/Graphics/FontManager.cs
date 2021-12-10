using System.IO;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Conarium.Graphics
{
	public class FontManager
    {
        public SpriteFont Arial8 { get; private set; }
        public SpriteFont Arial10 { get; private set; }
        public SpriteFont Arial12 { get; private set; }
        public SpriteFont Arial14 { get; private set; }
        public SpriteFont Arial16 { get; private set; }
        public SpriteFont Arial20 { get; private set; }
        public SpriteFont Arial30 { get; private set; }
        public SpriteFont Arial10Italic { get; private set; }
        public SpriteFont Consolas10 { get; private set; }
        public SpriteFont Consolas12 { get; private set; }
        public SpriteFont ComicSans10 { get; private set; }
        public void LoadAssets(string sourcePath, ContentManager Content)
        {
            
            Content.RootDirectory = Path.Combine(sourcePath, "Assets", "Fonts");
            Arial8 = Content.Load<SpriteFont>("Arial8");
            Arial10 = Content.Load<SpriteFont>("Arial10");
            Arial12 = Content.Load<SpriteFont>("Arial12");
            Arial14 = Content.Load<SpriteFont>("Arial14");
            Arial16 = Content.Load<SpriteFont>("Arial16");
            Arial20 = Content.Load<SpriteFont>("Arial20");
            Arial30 = Content.Load<SpriteFont>("Arial30");
            Arial10Italic = Content.Load<SpriteFont>("Arial10Italic");
            Consolas10 = Content.Load<SpriteFont>("Consolas10");
            Consolas12 = Content.Load<SpriteFont>("Consolas12");
            ComicSans10 = Content.Load<SpriteFont>("ComicSans10");
        }
    }
}