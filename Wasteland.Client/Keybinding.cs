using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Input;
using Conarium.Services;

namespace Wasteland.Client
{
	public class Keybinding
	{

		public Keybinding()
		{
			Keys = new List<Keys>();
			Buttons = new List<Buttons>();
			MouseButtons = new List<MouseButton>();
		}

		// curried methods for adding binds
		public Keybinding AddBind<TEnum>(TEnum bind) where TEnum : System.Enum
		{
			if (bind is Keys keybind)
				Keys.Add(keybind);
			if (bind is Buttons btnbind)
				Buttons.Add(btnbind);
			if (bind is MouseButton mousebind)
				MouseButtons.Add(mousebind);
			return this;
		}
		// These are redundant & rather ugly
		/*public Keybinding AddBinds<TEnum1, TEnum2>(TEnum1 bind1, TEnum2 bind2) 
		where TEnum1: Enum where TEnum2: Enum
		{
			AddBind(bind1);
			AddBind(bind2);
			return this;
		}
		public Keybinding AddBinds<TEnum1, TEnum2, TEnum3>(TEnum1 bind1, TEnum2 bind2, TEnum3 bind3) 
		where TEnum1: Enum where TEnum2: Enum where TEnum3: Enum
		{
			
			AddBind(bind1);
			AddBind(bind2);
			AddBind(bind3);
			return this;
		}*/


		public bool HasBinded<TEnum>(TEnum bind)
		{
			if (bind is Keys keybind)
				return Keys.Contains(keybind);
			if (bind is Buttons btnbind)
				return Buttons.Contains(btnbind);
			if (bind is MouseButton mousebind)
				return MouseButtons.Contains(mousebind);
			return false;
		}
		public bool RemoveBind<TEnum>(TEnum bind)
		{
			if (bind is Keys keybind)
				return Keys.Remove(keybind);
			if (bind is Buttons btnbind)
				return Buttons.Remove(btnbind);
			if (bind is MouseButton mousebind)
				return MouseButtons.Remove(mousebind);
			return false;
		}


		public void ClearBinds()
		{
			Keys.Clear();
			Buttons.Clear();
			MouseButtons.Clear();
		}
		public List<Keys> Keys {get;set;}
		public List<Buttons> Buttons {get;set;}
		public List<MouseButton> MouseButtons {get;set;}

		public bool IsDown()
		{
			foreach(var key in Keys)
				if (Keyboard.GetState().IsKeyDown(key))
					return true;

			foreach(var button in Buttons)
				if (GamePad.GetState(0).IsButtonDown(button))
					return true;

			var maus = Mouse.GetState();
			foreach(var mbutton in MouseButtons)
			{
				switch(mbutton)
				{
					case MouseButton.RightMouseButton:
						return maus.RightButton == ButtonState.Pressed;
					case MouseButton.LeftMouseButton:
						return maus.LeftButton == ButtonState.Pressed;
					case MouseButton.MiddleMouseButton:
						return maus.MiddleButton == ButtonState.Pressed;
				}
			}
			

			return false;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public bool Released()
		{
			var Input = InputService.Get();

			foreach(var key in Keys)
				if (Input.KeyReleased(key))
					return true;

			foreach(var button in Buttons)
				if (Input.ButtonReleased(button, 0))
					return true;

			var maus = Mouse.GetState();
			foreach(var mbutton in MouseButtons)
			{
				switch(mbutton)
				{
					case MouseButton.RightMouseButton:
						return Input.RMBReleased();
					case MouseButton.LeftMouseButton:
						return Input.LMBReleased();
					case MouseButton.MiddleMouseButton:
						return Input.MMBReleased();
				}
			}
			return false;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public bool Pressed()
		{
			var Input = InputService.Get();

			foreach(var key in Keys)
				if (Input.KeyPressed(key))
					return true;

			foreach(var button in Buttons)
				if (Input.ButtonPressed(button, 0))
					return true;

			var maus = Mouse.GetState();
			foreach(var mbutton in MouseButtons)
			{
				switch(mbutton)
				{
					case MouseButton.RightMouseButton:
						return Input.RMBPressed();
					case MouseButton.LeftMouseButton:
						return Input.LMBReleased();
					case MouseButton.MiddleMouseButton:
						return Input.MMBReleased();
				}
			}
			return false;
		}
	}
}
