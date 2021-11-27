using Microsoft.Xna.Framework;

namespace Conarium.Services
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