using System;
using Conarium;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Conarium.UI
{
    public class WindowNode : RectNode
    {
        public static bool GlobalMouseLock {get;set;}

        public Vector2 MinimumWindowSize {get;set;}
        RectNode TitleBar;
        public ButtonNode CloseButton;
        public RectNode Frame {get; private set;}
        public RectNode Content {get;set;}
        public WindowNode(string title, int width, int height) : base(title) 
        {

            Size = UIVector.FromPixels(width, height);
            RectColor = new Color(0f, 0f, 0.1f, 0.8f);
            BorderSize = 2;
            BorderColor = new Color(0.2f, 0.2f, 0.3f);
            MinimumWindowSize = new Vector2(200, 100);
            

            TitleBar = new RectNode("titlebar")
            {
                
                RectColor = new Color(0.2f, 0.2f, 0.3f),
                Parent = this,
                Position = new UIVector(0,0,0,0),
                Size = new UIVector(0, 20, 1, 0),
                BorderSize = 1,
                BorderColor = new Color(0.2f, 0.2f, 0.3f),
            };
            var titleBarText = new TextNode("titlebarText")
            {
                Text = title,
                Font = GraphicsService.Get().Fonts.Arial12,
                TextColor = Color.White,
                Parent = TitleBar,
                YAlignment = TextYAlignment.Center,
                XAlignment = TextXAlignment.Center,
            };

            CloseButton = new ButtonNode("closebutton")
            {
                Parent = TitleBar,
                Size = UIVector.FromPixels(30, 20),
                Position = new UIVector(-30, 0, 1, 0),
                BorderSize = 0,
            };
            //CloseButton.OnPressed += (e,s) => this.Visible = false;
            var closeButtonXText = new TextNode("x")
            {
                Font = GraphicsService.Get().Fonts.Arial16,
                TextColor = new Color(0.6f, 0.6f, 1f),
                Text = "X",
                Parent = CloseButton,
                YAlignment = TextYAlignment.Center,
                XAlignment = TextXAlignment.Center,
            };

            Content = new RectNode("content")
            {
                Parent = this,
                Size = new UIVector(-8, -24, 1, 1),
                Position = new UIVector(4, 20, 0, 0),
                RectColor = Color.Transparent,
                BorderSize = 0,
                //BorderColor = new Color(0.2f, 0.2f, 0.3f),
            };
        }

        // 
        Vector2 ParentSize => Parent!=null ? Parent.AbsoluteSize : GraphicsService.Get().WindowSize;
        Vector2 ParentPosition => Parent!=null ? Parent.AbsolutePosition : Vector2.Zero;
        // TODO: 
        public override Vector2 AbsoluteSize => new Vector2(
			Size.Pixels.X + (ParentSize.X * Size.Scale.X),
			Size.Pixels.Y + (ParentSize.X * Size.Scale.Y)
		);


        public override Vector2 AbsolutePosition => new Vector2(
			ParentPosition.X + Position.Pixels.X + (ParentPosition.X * Position.Scale.X) - (AnchorPoint.X * AbsoluteSize.X),
			ParentPosition.Y + Position.Pixels.Y + (ParentPosition.Y * Position.Scale.Y) - (AnchorPoint.Y * AbsoluteSize.Y)
		);

        Vector2 WindowMoveLock;
        bool WindowMoving;
        bool WindowResizing;


        public override void Update(GameTime gt)
        {
            if (!Active)
                return;


            if (!GlobalMouseLock && TitleBar.IsMouseInside() && !CloseButton.IsMouseInside() && InputService.Get().LMBPressed() && !WindowMoving)
            {
                WindowMoveLock = Mouse.GetState().Position.ToVector2();
                WindowMoving = true;
                GlobalMouseLock = true;
            }

            if (!GlobalMouseLock && IsMouseInside() && !Content.IsMouseInside())
            {
                Mouse.SetCursor(MouseCursor.SizeNWSE);
                if (InputService.Get().RMBPressed() && !WindowMoving)
                {
                    WindowMoveLock = Mouse.GetState().Position.ToVector2();
                    WindowResizing = true;
                    GlobalMouseLock = true;
                }
            }
            if (GlobalMouseLock && !InputService.Get().GetLMB() && WindowMoving)
            {
                WindowMoving = false;
                GlobalMouseLock = false;
            }
            if (GlobalMouseLock && !InputService.Get().GetRMB() && WindowResizing)
            {
                WindowResizing = false;
                GlobalMouseLock = false;
            }
            if (WindowMoving)
            {
                var mouse = Mouse.GetState().Position.ToVector2();
                var deltaMouse = mouse - WindowMoveLock;
                Position += UIVector.FromPixels((int)deltaMouse.X, (int)deltaMouse.Y);
                WindowMoveLock = Mouse.GetState().Position.ToVector2();
            }

            if (WindowResizing)
            {
                var mouse = Mouse.GetState().Position.ToVector2();
                var deltaMouse = mouse - WindowMoveLock;
                Size += UIVector.FromPixels((int)deltaMouse.X, (int)deltaMouse.Y);
                WindowMoveLock = Mouse.GetState().Position.ToVector2();
            }


            Size = UIVector.FromPixels(
                (int)Math.Clamp(Size.PixelX, MinimumWindowSize.X, 9999),
                (int)Math.Clamp(Size.PixelY, MinimumWindowSize.Y, 9999)
            );

            base.Update(gt);
        }

        public override void Draw()
        {
            if (!Visible)
                return;
            base.Draw();
        }

    }
}