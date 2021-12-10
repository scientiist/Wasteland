using System;
using Conarium;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Conarium.UI
{
    public class ScrollRectNode : RectNode
    {

        public float CanvasSize { get; set; }

		float canvasPos;
		float goalCanvasPos;
		public float CanvasPosition
		{
			get => canvasPos;
			set => goalCanvasPos = value;
		}

		public Vector2 RealAbsoluteSize
		{
			get
			{
				var xSize = Size.Pixels.X + (Parent.AbsoluteSize.X * Size.Scale.X);
				var ySize = (Size.Pixels.Y + (Parent.AbsoluteSize.Y * Size.Scale.Y));

				return new Vector2(xSize, ySize);
			}
		}
		public new Vector2 AbsoluteSize
		{
			get
			{
				var xSize = (Size.Pixels.X + (Parent.AbsoluteSize.X * Size.Scale.X)) - ScrollbarWidth;
				var ySize = (Size.Pixels.Y + (Parent.AbsoluteSize.Y * Size.Scale.Y)) * this.CanvasSize;

				return new Vector2(xSize, ySize);
			}
		}

		public Vector2 RealAbsolutePosition
		{
			get
			{
				var xPos = Parent.AbsolutePosition.X + Position.Pixels.X + (Parent.AbsoluteSize.X * Position.Scale.X) - (AnchorPoint.X * AbsoluteSize.X);
				var yPos = Parent.AbsolutePosition.Y + Position.Pixels.Y + (Parent.AbsoluteSize.Y * Position.Scale.Y) - (AnchorPoint.Y * AbsoluteSize.Y);

				return new Vector2(xPos, yPos);
			}
		}
		public override Vector2 AbsolutePosition
		{
			get
			{
				var xPos = Parent.AbsolutePosition.X + Position.Pixels.X + (Parent.AbsoluteSize.X * Position.Scale.X) - (AnchorPoint.X * AbsoluteSize.X);
				var yPos = (Parent.AbsolutePosition.Y + Position.Pixels.Y + (Parent.AbsoluteSize.Y * Position.Scale.Y) - (AnchorPoint.Y * AbsoluteSize.Y)) - ( (CanvasPosition/ CanvasSize) * AbsoluteSize.Y);

				return new Vector2(xPos, yPos);
			}
		}

		public Color ScrollbarColor { get; set; }

		public int ScrollbarWidth { get; set; }

		public RectNode Scrollbar { get; private set; }
		public RectNode Content { get; private set; }
		public ButtonNode Scrubber {get; private set;}
		private int initialScrollValue;

		private void CreateSubcomponents()
        {
			initialScrollValue = Mouse.GetState().ScrollWheelValue;

			Scrollbar = new RectNode("scrollbar")
			{
				Size = new UIVector(ScrollbarWidth, 0, 0, 1),
				//AnchorPoint = new Vector2(1, 0),
				Position = new UIVector(-16, 0, 1, 0),
				RectColor = new Color(0.5f, 0.5f, 0.5f), 
				Parent = this,
			};

			Scrubber = new ButtonNode("scrubber")
			{
				Size = new UIVector(0, 0, 1, 2),
				Position = new UIVector(0,0,0,0),
				Parent = Scrollbar,
				RectColor = new Color(0.25f, 25f, 1.0f),
				BaseColor = new Color(0.75f, 0.75f, 0.75f),
				FocusedColor = new Color(1f, 1f, 1f),
				//Font = GraphicsEngine.Instance.Fonts.Arial10,
			};

			Content = new RectNode("content")
			{
				Size = new UIVector(0, 0, 1, 1),
				Parent = this,
				RectColor = Color.Transparent,
			};
        }
		public override bool IsMouseInside(MouseState mouse)
		{
			return (mouse.X > RealAbsolutePosition.X && mouse.Y > RealAbsolutePosition.Y
				&& mouse.X < (RealAbsolutePosition.X + RealAbsoluteSize.X)
				&& mouse.Y < (RealAbsolutePosition.Y + RealAbsoluteSize.Y));
		}



		float lastFrameScrollValue;

        public ScrollRectNode(string name) : base(name)
        {
			Visible = true;

			CreateSubcomponents();
        }

        public override void Update(GameTime gt)
        {
			//canvasPos = canvasPos.Lerp(goalCanvasPos, gt.GetDelta()*20.0f);

			MouseState mouse = Mouse.GetState();
			initialScrollValue = mouse.ScrollWheelValue;

			float scrubberFrac = 1 / this.CanvasSize;
			float posFrac = this.CanvasPosition/ this.CanvasSize;
			//Scrollbar.Position = new UIVector(-ScrollbarWidth, ( this.RealAbsolutePosition.Y -this.AbsolutePosition.Y ), 1, 0);
			//Scrubber.Size = new UIVector(0, 100, 1, 0);
			//Scrubber.Position = new UIVector(0, 0, 0, posFrac);


			/*if (IsMouseInside(mouse))
            {
				if (mouse.ScrollWheelValue != lastFrameScrollValue)
                {
					float diff = (mouse.ScrollWheelValue - lastFrameScrollValue) / 500.0f;
					//CanvasPosition += diff;
					var dd = CanvasPosition - diff;
					CanvasPosition = Math.Clamp(dd, 0, CanvasSize);

					lastFrameScrollValue = mouse.ScrollWheelValue;
                }
            }*/

			
			base.Update(gt);
		}

        public override void Draw()
		{
            var gfx = GraphicsService.Get();
			Rectangle current = gfx.GraphicsDevice.ScissorRectangle;
			gfx.GraphicsDevice.ScissorRectangle = new Rectangle(RealAbsolutePosition.ToPoint(), RealAbsoluteSize.ToPoint());
			//gfx.Rect(BGColor, RealAbsolutePosition, RealAbsoluteSize);
			gfx.OutlineRect(BorderColor, RealAbsolutePosition, RealAbsoluteSize, BorderSize);
			gfx.Text($"scrollrect canvas pos {CanvasPosition} size {CanvasSize}", RealAbsolutePosition);
            base.Draw();
            gfx.GraphicsDevice.ScissorRectangle = current;

		}

	}
}