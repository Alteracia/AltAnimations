using System;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

namespace Alteracia.Animation
{
    [CreateAssetMenu(fileName = "AnimationGroup", menuName = "AltAnimations/AnimationGroup", order = 1)]
    public class AltAnimationGroup : ScriptableObject, IAnimation
    {
        [SerializeField] 
        private string id;
        public int Id => UnityEngine.Animator.StringToHash(id);

        [SerializeField]
        private AltAnimation[] animations;
        public AltAnimation[] Animations
        {
            get => animations;
            set => animations = value;
        }

        private int _playingCount = -1;
        public bool Running => _playingCount > 0;

        private Action _finishCallback;

        public void ChangeFinish(object finish)
        {
            foreach (var anim in animations)
            {
                if (anim)
                    anim.ChangeFinish(finish);
            }
        }

        public void Play(GameObject master, Action finishCallback = null)
        {
            ChangeCallback(finishCallback);
            
            if (animations.Length == 0 || this.Running) return;
            
            _playingCount = animations.Length;
            
            // Play all without callback
            foreach (var anim in animations)
            {
                if (!anim) CountFinished();
                else anim.Play(master, CountFinished);
            }
        }

        /// <summary>
        /// Stop animation group
        /// </summary>
        /// <param name="invokeFinishCallback">If true: onFinish callback of longest member of group will be called</param>
        public void Stop(bool invokeFinishCallback)
        {
            // Invoke callback if needed
            if (invokeFinishCallback) _finishCallback?.Invoke();
            
            // Set callback to null
            ChangeCallback();
            
            // Stop all
            foreach (var anim in animations)
            {
                // Stop with invoke for right count
                anim.Stop(true);
            }
        }

        /// <summary>
        /// Wait while the longest member of group playing
        /// </summary>
        /// <param name="changeCallback">If true finishCallback property will be changed on new</param>
        /// <param name="finishCallback"></param>
        /// <returns></returns>
        public async Task Wait(bool changeCallback = false, Action finishCallback = null)
        {
            if (changeCallback && !this.Running) // TODO Check this
                ChangeCallback(finishCallback);
            
            // Wait all animations
            foreach (var animation in animations.Where(a => a.Running))
                await animation.Wait(); // TODO CHeck
        }

        private void CountFinished()
        {
            _playingCount--;
            
            if (_playingCount != 0) return;
            
            // All animations finished
            _finishCallback?.Invoke();
            this.ChangeCallback();
        }

        public void ChangeCallback(Action finishCallback = null)
        {
            _finishCallback = finishCallback;
        }

        public bool HaveEqualAnimationWith(AltAnimationGroup other)
        {
            // find one equals animation pair
            return (from anim in animations
                from otherAnim in other.animations where anim.Equals(otherAnim)
                select anim).Any();
        }
    }
}
