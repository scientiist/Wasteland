using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SkeletalAnimation
{
    public struct KeyframeAnimationState
    {
        public Transformation CurrentKeyframe;
        public KeyframeAnimation Animation { get; set; }

        private int currentKeyframeIndex;
        private int nextKeyframeIndex;
        private float currentSectionLength;
        private float timeSinceFrameChange;

        public KeyframeAnimationState(KeyframeAnimation animation)
            :this()
        {
            this.Animation = animation;
            Reset();
        }
        public void Reset()
        {
            timeSinceFrameChange = 0;
            currentKeyframeIndex = 0;
            nextKeyframeIndex = 0;
            currentSectionLength = 0;

            if (Animation.Keyframes.Count == 0)
                CurrentKeyframe = Transformation.Identity;

            if (Animation.Keyframes.Count == 1)
                CurrentKeyframe = Animation.Keyframes[0].Transform;

            if (Animation.Keyframes.Count > 1)
            {
                currentSectionLength = (Animation.Keyframes[1].FrameNumer - Animation.Keyframes[0].FrameNumer) / Animation.Framerate;
                nextKeyframeIndex = 1;
                Update(0);
            }
        }

        public void Update(float elapsedSeconds)
        {
            if (Animation.Keyframes.Count <= 1)
                return;
            timeSinceFrameChange += elapsedSeconds;

            while (timeSinceFrameChange > currentSectionLength)
            {
                timeSinceFrameChange -= currentSectionLength;
                currentKeyframeIndex = nextKeyframeIndex;
                nextKeyframeIndex = (nextKeyframeIndex + 1) % Animation.Keyframes.Count;

                int fn1 = Animation.Keyframes[currentKeyframeIndex].FrameNumer;
                int fn2 = Animation.Keyframes[nextKeyframeIndex].FrameNumer;
                int frameDifference = fn2 > fn1 ? fn2 - fn1 : (Animation.Length - fn1) + fn2;
                currentSectionLength = frameDifference / Animation.Framerate;
            }

            Transformation key1 = Animation.Keyframes[currentKeyframeIndex].Transform;
            Transformation key2 = Animation.Keyframes[nextKeyframeIndex].Transform;
            Transformation.Lerp(ref key1, ref key2, timeSinceFrameChange / currentSectionLength, ref CurrentKeyframe);
        }

    }
}
