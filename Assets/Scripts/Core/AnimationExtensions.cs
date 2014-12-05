using UnityEngine;
using System.Collections;

namespace UnityEngineExt
{
    public static class AnimationExtensions {
        public static void PlayImmediately(this Animation animation, string clipName)
        {
            animation.Play(clipName);
            animation.gameObject.SampleAnimation(animation.GetClip(clipName), 0.0f);
        }

        public static bool IsFinishedPlayingAnimationWithTag(this Animator thiz, string tag, int layerIndex = 0)
        {
            AnimatorStateInfo state = thiz.GetCurrentAnimatorStateInfo(layerIndex);

            return state.IsTag(tag) && state.normalizedTime >= 1.0f;
        }
    }
}