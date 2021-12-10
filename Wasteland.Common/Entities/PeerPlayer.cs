using System;

namespace Wasteland.Common
{
	public class PeerPlayer : Player
	{
		public PeerPlayer(Guid networkUUID) : base(networkUUID)
		{
		}
	}
}