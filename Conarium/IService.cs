using Microsoft.Xna.Framework;

namespace Conarium
{
    public interface IService 
    {

    }

    public class Service : GameComponent, IService
    {

        public Service(Game game) : base(game)
        {
            
        }
    }
}