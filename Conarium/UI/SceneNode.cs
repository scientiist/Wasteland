using System.Collections.Generic;
using System.Linq;
using Conarium.Extension;
using Conarium;
using Microsoft.Xna.Framework;


namespace Conarium.UI
{
    public class SceneNode : BaseUIFunc
    {
        public SceneNode(string name) : base(name)
        {

        }


        public override void Draw()
        {
            GraphicsService.Get().Begin();
            base.Draw();
            GraphicsService.Get().End();
        }


        public override Vector2 AbsolutePosition => Vector2.Zero;
        public override Vector2 AbsoluteSize => GraphicsService.Get().WindowSize;

        
    
    }
}