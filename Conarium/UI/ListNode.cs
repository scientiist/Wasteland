using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Conarium.UI
{
    public class ListNode : BaseUIFunc
	{
		public int Padding { get; set; }

		public override Vector2 AbsolutePosition => Parent.AbsolutePosition;
        public override Vector2 AbsoluteSize => Parent.AbsoluteSize;
		
		
		public bool ExpandSelected { get; set; }
		public float ExpandedHeight { get; set; }
		public float CompressedHeight { get; set; }

        public ListNode(string name) : base(name)
		{
			Padding = 1;
			ExpandSelected = true;
		}

		public override void Draw()
		{
			foreach (var child in Children)
				child.Draw();
		}

		public override void Update(GameTime gt)
		{
            double delta = gt.ElapsedGameTime.TotalSeconds;

			double alphaCapped = Math.Clamp(delta * 100.0, 1 / 1000.0, 0.5);

			int ypos = 0;
			foreach (var child in Children)
			{

				child.Update(gt);

				if (child is TextNode label)
				{
					child.Position = new UIVector(0, ypos, 0, 0);
					// TODO: FIXME
					//ypos += (int)child.AbsoluteSize.Y*(label.TextWrap+1);
					ypos += Padding;
				} else
				{
					child.Position = new UIVector(0, ypos, 0, 0);

					ypos += (int)child.AbsoluteSize.Y;
					ypos += Padding;
				}
			}
		}
	}
}