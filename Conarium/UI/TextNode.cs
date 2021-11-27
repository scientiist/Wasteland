using System.Text;
using Conarium.Services;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Conarium.UI
{

    public class TextNode : BaseUIFunc
    {
        public TextNode(string name) : base(name)
        {
            Text = "Hello, World!";
            Font = GraphicsService.Get().Fonts.Arial14;
        }


        public override Vector2 AbsolutePosition => Parent.AbsolutePosition;
        public override Vector2 AbsoluteSize => Parent.AbsoluteSize;

        public Color TextColor  {get;set;}

        public virtual string Text {get;set;}

        // TODO: Implement Text Selectability?
        public bool TextSelectable {get;set;}
        public virtual bool TextWrap {get;set;}
        
        public SpriteFont Font {get;set;}

        public TextXAlignment XAlignment {get;set;}
        public TextYAlignment YAlignment {get;set;}

        public bool TextBeingWrapped {get; private set;}
        public int TextWrappingCount {get; private set;}

        public string WrapText(SpriteFont font, string text, float maxLineWidth)
        {
            string[] words = text.Split(' ');
            StringBuilder sb = new StringBuilder();
            float lineWidth = 0f;
            float spaceWidth = font.MeasureString(" ").X;

            var TextWrappingCount = 0;

            foreach(string word in words)
            {
                Vector2 size = font.MeasureString(word);

                if (lineWidth + size.X < maxLineWidth)
                {
                    sb.Append(word + " ");
                    lineWidth += size.X + spaceWidth;
                }
                else
                {
                    sb.Append("\n" + word);
                    lineWidth = size.X + spaceWidth;
                    TextWrappingCount++;
                }
            }
            return sb.ToString();
        }


        protected Vector2 TextDrawPosition {
            get
            {
                Vector2 textDim = Font.MeasureString(Text);
                // text alignment
                Vector2 textFinalPosition = AbsolutePosition;

                if (XAlignment == TextXAlignment.Center)
                    textFinalPosition += new Vector2((AbsoluteSize.X / 2)-(textDim.X / 2), 0);
                if (XAlignment == TextXAlignment.Right)
                    textFinalPosition += new Vector2(AbsoluteSize.X - textDim.X, 0);
                if (YAlignment == TextYAlignment.Center)
                    textFinalPosition += new Vector2(0, (AbsoluteSize.Y / 2) - (textDim.Y / 2));
                if (YAlignment == TextYAlignment.Bottom)
                    textFinalPosition += new Vector2(0, AbsoluteSize.Y - textDim.Y);
                textFinalPosition.Floor();
                return textFinalPosition;
            }
        }
 
        public override void Draw()
        {
            base.Draw();

            string displayedText = "";
            if (Text!=null)
                displayedText = Text;
            if (TextWrap)
                displayedText = WrapText(Font, Text, AbsoluteSize.X);

            var gfx = GraphicsService.Get();
            gfx.Text(Font, displayedText, TextDrawPosition, TextColor);

        }

        public override void Update(GameTime gt)
        {

            base.Update(gt);
        }
    }
}