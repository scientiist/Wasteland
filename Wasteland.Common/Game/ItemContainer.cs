using System;
using System.Collections.Generic;

namespace Wasteland.Common.Game
{


    public class ItemContainer
    {

		bool itemWhitelistEnabled;
		public List<ItemUsage> ItemTypeWhitelist;
		
		public int Width {get;set;}
		public int Height {get;set;}
		public ItemContainer()
		{

		}

		public void Add(Item item)
		{
			if (itemWhitelistEnabled)
			{	
				if (ItemTypeWhitelist.Contains(item.Usage))
				{

				}
			} 
		}
    }
}
