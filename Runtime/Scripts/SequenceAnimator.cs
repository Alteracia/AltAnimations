using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;


namespace Alteracia.Animations
{
    public class SequenceAnimator : MonoBehaviour
    {
        [SerializeField] 
        private bool playOnStart;
        [SerializeField] 
        private bool instantiateAnimations = true;
        [SerializeField] 
        private bool loop;
        [SerializeField] 
        private AltAnimationGroup[] animationGroups = null;
        
        private bool _initialized = false;
        private AltAnimationGroup _current;
        private bool _playing;
        
        private void Start()
        {
            if (!playOnStart) return;
            
            this.Play();
        }
        
        public async void Play()
        {
            if (!_initialized) Init();

            _playing = true;
            do
            {
                foreach (var group in animationGroups)
                {
                    _current = group;
                    
                    _current.Play(this.gameObject);
                    await _current.Wait();

                    if (!_playing) return;
                }
            } while (loop);
            _playing = false;
        }

        private void OnDestroy()
        {
            Stop();
        }

        public void Stop()
        {
            loop = false;
            _current.Stop(false);
            _playing = false;
            _current = null;
        }
        
        private void Init()
        {
            if (instantiateAnimations)
            {
                // Copy all animations in groups
                List<AltAnimationGroup> newGroups = new List<AltAnimationGroup>();
                foreach (var group in animationGroups)
                {
                    if (group == null) continue;
                    newGroups.Add(AltAnimator.Copy(group));
                }

                animationGroups = newGroups.ToArray();
            }
            _initialized = true;
        }
        
    }
}