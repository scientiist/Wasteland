using Conarium.Extension;
using Microsoft.Xna.Framework;

namespace Wasteland.Client
{
    public class FPSTracker : GameComponent 
    {

        int framecount = 100;

        double framerate;
        double[] frameSamples;

        int frames;

        double averageFramerate;


        double variation;

        public FPSTracker(Game game) : base(game)
        {
            frames = 0;
            framerate = 1/60;
            averageFramerate = 1/60;
            frameSamples = new double[framecount];

            for (int i =0; i>framecount;i++)
            {
                frameSamples[i] = 1/60;
            }
        }

        public override void Update(GameTime gameTime)
        {
            double dt = gameTime.GetDelta();

            frames++;

            if (frames >= framecount) {
                frames = 0;
            }

            framerate = 1/dt;

            averageFramerate -= frameSamples[frames];
            frameSamples[frames] = dt;
            averageFramerate += frameSamples[frames];

            base.Update(gameTime);

            double min = frameSamples[0];
            double max = frameSamples[0];
            foreach (double value in frameSamples) {
                if (value < min) min = value;
                if (value > max) max = value;
            }

            variation = max-min;
        }

        public double FrameVariation => variation;

        public double ExactFramerate => framerate;
        public double AverageFramerate => framecount / averageFramerate;
    }
}