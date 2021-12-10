using System.Linq;
using Conarium.Extension;
using Conarium;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Conarium
{
    public class Camera2D : GameComponent
    {
        #region Matrix Construction
        float zoom;
        float rotation;
        Vector2 position;
        Matrix Transform = Matrix.Identity;
        bool IsViewTransformationDirty = true;
        Matrix CamTranslationMatrix = Matrix.Identity;
        Matrix CamRotationMatrix = Matrix.Identity;
        Matrix CamScaleMatrix = Matrix.Identity;


        public Vector2 ScreenCenterToWorldSpace => Position;

        public Vector2 WindowSize => GraphicsService.Get().WindowSize;

        public Matrix View => Matrix.CreateTranslation(new Vector3(0,0,0)) *
            Matrix.CreateRotationZ(Rotation) *
            Matrix.CreateScale(Zoom) *
            Matrix.CreateTranslation(new Vector3(0,0,0));


        private float Max(params float[] args) => args.Max();
        private float Min(params float[] args) => args.Min();

        public Rectangle GetVisibleArea()
        {
            var inverseViewMatrix = Matrix.Invert(View);
            var tl = Vector2.Transform(Vector2.Zero, inverseViewMatrix);
            var tr = Vector2.Transform(new Vector2(WindowSize.X, 0), inverseViewMatrix);
            var bl = Vector2.Transform(new Vector2(0, WindowSize.Y), inverseViewMatrix);
            var br = Vector2.Transform(WindowSize, inverseViewMatrix);


            var min = new Vector2(
                Min(tl.X, tr.X, bl.X, br.X),
                Min(tl.Y, tr.Y, bl.Y, br.Y));

            var max = new Vector2(
                Max(tl.X, tr.X, bl.X, br.X),
                Max(tl.Y, tr.Y, bl.Y, br.Y));

            return new Rectangle(
                (int)min.X, (int)min.Y, (int)(max.X-min.X), (int)(max.Y - min.Y));
        }

        #endregion

        public Vector2 Position;
        public Vector2 GoalPosition;

        public float goamZoom;


        public float MaxZoom {get;set;}
        public float MinZoom {get;set;}

        public float Rotation;

        

        public float Zoom;

        public bool FreeLook;

        public Camera2D(Game game) : base(game)
        {
            
        }
        
        void Center()
        {
            GoalPosition = Vector2.Zero;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            var Input = InputService.Get();



            if (Input.KeyPressed(Keys.F))
                Center();


            Position = Vector2.Lerp(Position, GoalPosition, gameTime.GetDelta()*8);



        }

        public void Draw()
        {

        }
    }
}