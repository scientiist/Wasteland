using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SkeletalAnimation
{
    public class SkeletonAnimationPlayer
    {
        public KeyframeAnimationState[] JointAnimations { get; set; }

        public SkeletonAnimationPlayer(SkeletonAnimation animation)
        {
            JointAnimations = new KeyframeAnimationState[animation.JointAnimations.Length];
            for (int i = 0; i < JointAnimations.Length; i++)
            {
                JointAnimations[i] = new KeyframeAnimationState(animation.JointAnimations[i]);
            }
        }

        public void Update(float elapsedSeconds)
        {
            for (int i = 0; i < JointAnimations.Length; i++)
            {
                var animState = JointAnimations[i];
                animState.Update(elapsedSeconds);
                JointAnimations[i] = animState;
            }
        }

        public void Apply(Skeleton skeleton)
        {
            for (int i = 0; i < JointAnimations.Length; i++)
            {
                skeleton.SetLocalTransform(i,JointAnimations[i].CurrentKeyframe);
            }
        }
    }
}
