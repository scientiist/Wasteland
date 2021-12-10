
using System.Collections.Generic;
using System.Diagnostics;
using Conarium;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Conarium.UI
{
    public class RectNode : BaseUIFunc
    {


        public RectNode(string name) : base(name)
        {
            BorderSize = 1f;
			RectColor = Color.White;
			BorderColor = Color.Black;
			BorderEnabled = true;
        }
		
		public Color RectColor { get; set; }
		public bool BorderEnabled { get; set; }
		public float BorderSize { get; set; }
		public Color BorderColor { get; set; }
		



		public override void Draw()
		{
            var gfx = GraphicsService.Get();

			gfx.Rect(RectColor, AbsolutePosition, AbsoluteSize);
			gfx.OutlineRect(BorderColor, AbsolutePosition, AbsoluteSize, BorderSize);


			Rectangle current = gfx.GraphicsDevice.ScissorRectangle;
			if (ClipsDescendants)
				gfx.GraphicsDevice.ScissorRectangle = new Rectangle(AbsolutePosition.ToPoint(), AbsoluteSize.ToPoint());

			foreach(var child in Children)
                child.Draw();

			if (ClipsDescendants)
				gfx.GraphicsDevice.ScissorRectangle = current;

            
		}

	}
}