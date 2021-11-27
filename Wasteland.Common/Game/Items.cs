using System;

namespace Wasteland.Common.Game
{

	


	public interface IWeapon 
	{
		float BaseDamage {get;set;}
		float HeadshotChance {get;set;}
	}
	public interface IStackable
	{
		int Amount {get;set;}


	}
	public interface IWearable
	{
		float Protection {get;set;}
	}
	public interface IMelee
	{
		float BaseDamage {get;set;}
		float SwingRange {get;set;}
		float HeadshotChance {get;set;}
	}

	//----------------------------------------
	// Option 1 => Flags based
	[Flags]
	public enum ItemUsage
	{
		Wearable = 0x0,
		Consumable = 0x1,
		Throwable = 0x2,
		MeleeWeapon = 0x4,
		RangedWeapon = 0x5,
	}

	public class ExampleItem
	{
		public ItemUsage Usage = ItemUsage.Throwable | ItemUsage.MeleeWeapon;


		public void Niggers()
		{
			if (Usage.HasFlag(ItemUsage.MeleeWeapon))
			{

			}
		}
	}
	//----------------------------------------
	// Option 2 => Inheritance based

	public abstract class Item 
	{ 
		public virtual ItemUsage Usage {get;set;}
		public void Niggers()
		{
			if (this is Weapon ourWeapon)
			{
				ourWeapon.Niggers();
			}
		}
	}
	public abstract class Weapon : Item { }
	public abstract class MeleeWeapon : Weapon {}
	public abstract class RangedWeapon : Weapon { }
	public abstract class ThrowableWeapon : Weapon { }
	public abstract class Wearable : Item { }
	public abstract class Consumable : Item { }

	

}
