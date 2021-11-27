using System;
using Conarium.Services;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Input;

namespace Conarium.UI
{
    public interface IButtonWidget
	{
        bool MouseDown { get; }
		bool Selected { get; set; }
		Color UnselectedBGColor { get; set; }
		Color SelectedBGColor { get; set; }
    }

	public enum ButtonPressFunctionality
	{
		OnPress,
		OnRelease
	}

    public class ButtonNode: RectNode
	{
        public ButtonNode(string name) : base(name)
        {
			FocusedColor = Color.Gray;
			PressedColor = Color.DarkGray;
        }

		
		/// <summary>
		/// Mouse Button/Gamepad/Touch Press
		/// </summary>
		public event EventHandler OnPressed; 
		/// <summary>
		/// Mouse Button/Gamepad/Touch Press Released
		/// </summary>
		public event EventHandler OnReleased;
		public event EventHandler OnFocused;
		public event EventHandler OnUnfocused;

		public SoundEffect ButtonClickSound {get;set;}

		public Color BaseColor {get;set;}
        public Color FocusedColor { get; set; }
		public Color PressedColor { get; set; }

		public bool Focused { get; set; }
        public bool MouseDown { get; private set; }

		MouseState prevMouse;

		public static float ClickTimer = 0;

		Color backgroundColor;

		public override void Update(GameTime gt)
		{
			if (!Active)
                return;

			MouseState mouse = Mouse.GetState();
			Color finalRectColor = RectColor;
			Focused = IsMouseInside(mouse);

			RectColor = Focused ? FocusedColor : BaseColor;


			if (IsMouseInside())
			{
				Mouse.SetCursor(MouseCursor.Hand);
			}
			


            if (Focused && !IsMouseInside(prevMouse)) 
			{
				OnFocused?.Invoke(this, null);
				ButtonClickSound?.Play();
				
				
			}

            if (!Focused && IsMouseInside(prevMouse))
			{
				OnUnfocused?.Invoke(this, null);
				ButtonClickSound?.Play();
			}

			if (MouseDown && InputService.Get().LMBReleased()) 
			{
				MouseDown = false;
				if (Focused)
					OnPressed?.Invoke(this, null);			
            }

            if (Focused)
			{ 
				if (InputService.Get().LMBPressed()) 
				{
					MouseDown = true;
					finalRectColor = PressedColor;
					ButtonClickSound?.Play();
					
					ClickTimer = 0; 
                }

				/*if (InputService.Get().RMBPressed()) 
				{
                    ButtonClickSound?.Play();
					OnReleased?.Invoke(this, null);
					ClickTimer = 0; 
                }*/
						
            }



			RectColor = MouseDown ? PressedColor : RectColor; 

			prevMouse = mouse;

			
            base.Update(gt);
		}
	}
}