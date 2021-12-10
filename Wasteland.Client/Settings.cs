using System.Linq;
using System.Security.AccessControl;
using System;
using System.Collections.Generic;
using Conarium;
using Microsoft.Xna.Framework.Input;
using Wasteland.Common;
using System.Xml.Serialization;
using System.Reflection;

namespace Wasteland.Client
{
	public enum KeybindType
	{
		Unset,
		Keyboard,
		MouseButton,
		Gamepad
	}
	public enum MouseButton
	{
		RightMouseButton,
		LeftMouseButton,
		MiddleMouseButton,
		XtraButton1,
		XtraButton2,

	}

	// TODO: write serialization scheme for this crap.
	public class Keybinds
	{

		#region Keybind Definitions & Defaults
		[Keybind("Navigation", "Move Up")] 		public Keybinding PlayerMoveUp 		= new Keybinding()
			.AddBind(Keys.W)
			.AddBind(Buttons.DPadUp);
		[Keybind("Navigation", "Move Down")] 	public Keybinding PlayerMoveDown 	= new Keybinding()
			.AddBind(Keys.S) 
			.AddBind(Buttons.DPadDown);
		[Keybind("Navigation", "Move Left")]  	public Keybinding PlayerMoveLeft 	= new Keybinding()
			.AddBind(Keys.A)
			.AddBind(Buttons.DPadLeft);
		[Keybind("Navigation", "Move Right")] 	public Keybinding PlayerMoveRight 	= new Keybinding()
			.AddBind(Keys.D)
			.AddBind(Buttons.DPadRight);
		[Keybind("Actions", "Attack")] 			public Keybinding PlayerAttack 		= new Keybinding()
			.AddBind(MouseButton.LeftMouseButton)
			.AddBind(Keys.Enter)
			.AddBind(Buttons.A);
		[Keybind("Actions", "Interact")] 		public Keybinding PlayerInteract 	= new Keybinding()
			.AddBind(MouseButton.RightMouseButton)
			.AddBind(Keys.I)
			.AddBind(Buttons.Y);
		[Keybind("Actions", "Reload Weapon")] 	public Keybinding PlayerReload 		= new Keybinding()
			.AddBind(Keys.R)
			.AddBind(Buttons.X);
		[Keybind("Actions", "View Inventory")] 	public Keybinding ViewInventory 	= new Keybinding()
			.AddBind(Keys.E)
			.AddBind(Buttons.Back);
		[Keybind("Menu", "Open Game Console")] 	public Keybinding OpenGameConsole 	= new Keybinding()
			.AddBind(Keys.OemTilde)
			.AddBind(Buttons.BigButton);
		[Keybind("Menu", "Open Pause Menu")] 	public Keybinding TogglePauseMenu 	= new Keybinding()
			.AddBind(Keys.Escape)
			.AddBind(Buttons.Start);
		#endregion

		public List<Keybinding> BindCollection {get; private set;}

		public Keybinds()
		{
			BindCollection = new List<Keybinding>();
			// filter properties for those with keybind attribute
			var propertyList = this.GetType().GetProperties().Where(
				(t) => t.GetCustomAttribute(typeof(KeybindAttribute))!=null
			);
			propertyList = propertyList.ToArray();
			foreach(var prop in propertyList)
			{
				Keybinding thisPropVal = (Keybinding)prop.GetValue(this);
				BindCollection.Add(thisPropVal);
			}
			Console.WriteLine(BindCollection);
		}

		public void Clear<TBind>(TBind bind)
		where TBind : System.Enum
		{
			foreach(var keybinding in BindCollection)
				if (keybinding.HasBinded<TBind>(bind))
					keybinding.RemoveBind<TBind>(bind);
		}
	}
	

	// Settings Menu Metadata Attributes
	// Defines how objects will appear on the Settings menu
	// & how they interact with settings properties
	public class SettingsMenuEntryAttribute : Attribute
	{
		public SettingsMenuEntryAttribute(string prefab, string label)
		{

		}
	}

	public delegate string BoolDisplayConversion(bool input);
	public class CheckboxAttribute : Attribute
	{
		public CheckboxAttribute(string label, bool defaultValue, string trueText, string falseText)  : base()
		{

		}
	}
	public class KeybindAttribute : Attribute
	{
		public KeybindAttribute(string grouping, string label) : base()
		{

		}
	}
	public class EnumListAttribute : Attribute
	{
		public EnumListAttribute(string label) : base()
		{

		}
	}

	// put it all together to get our Settings class

    public class Settings : XMLConfiguration
    {
		[XmlIgnore] static Settings Instance;
		public static Settings Get() => Instance;
		public Settings() : base()
		{
			Instance = this;
			Keybindings = new Keybinds();
		}


		[Checkbox("V-Sync", false, "On", "Off")] public bool VerticalSync {get;set;} = false;

		[EnumList("Language")] public Language Language {get;set;}

		public Keybinds Keybindings {get;set;}

    }
}
