using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using UnityEngine;
using UnityEngine.Events;

namespace Alteracia.Animations
{
    public class AltAnimator : MonoBehaviour
    {
        [SerializeField] 
        private string playOnStart = "";
        
        [SerializeField] 
        private bool instantiateAnimations = true;

        [SerializeField]
        private AltAnimationGroup[] animationGroups = null;

        [SerializeField] 
        private UnityEvent defaultCallback = new UnityEvent();
        
        private bool _initialized = false;

        private void Start()
        {
            if (string.IsNullOrEmpty(playOnStart)) return;
            
            this.Play(playOnStart);
        }

        /// <summary>
        /// Play Method for UnityEvent subscription in Inspector
        /// </summary>
        /// <param name="id">Id of animation group</param>
        public void Play(string id)
        {
            this.Play(id, defaultCallback.Invoke);
        }
        
        /// <summary>
        /// Start Playing animation group
        /// </summary>
        /// <param name="id">Id of animation group to play</param>
        /// <param name="finishCallback">Action witch will be called after animation finished or stopped</param>
        public void Play(string id, Action finishCallback)
        {
            if (!this) return;
            if (animationGroups == null)
            {
                finishCallback?.Invoke();
                return;
            }
            
            if (!_initialized) Init();
            
            if (animationGroups.Any(g => g.Id == id))
            {
                var currentAnimG = animationGroups.First(g => g.Id == id);
                // Stop animationGroups with same changing properties as currentAnimG
                foreach (var animG in animationGroups.Where(g => g.Id != id && g.Running))
                    if (currentAnimG.HaveEqualAnimationWith(animG))
                        animG.Stop(true);
                
                currentAnimG.Play(this.gameObject, finishCallback);
            }
            else
                Debug.LogWarning("Can't find animation. Please check animation id", this.gameObject);
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
                    newGroups.Add(Copy(group));
                }

                animationGroups = newGroups.ToArray();
            }
            _initialized = true;
        }

        internal static AltAnimationGroup Copy(AltAnimationGroup group)
        {
            AltAnimationGroup newGroup = ScriptableObject.Instantiate(group);
            newGroup.Nested.Clear();
            
            foreach (var newAnim in from anim in @group.Nested 
                where anim != null select ScriptableObject.Instantiate(anim))
            {
                newAnim.Initialise(newGroup);
                newGroup.Nested.Add(newAnim);
            }

            return newGroup;
        }
        
        /// <summary>
        /// Add array of animations group
        /// </summary>
        /// <param name="groups">Array of animations group</param>
        public void Add(AltAnimationGroup[] groups)
        {
            foreach (AltAnimationGroup group in groups)
            {
                if (group == null) continue;
                this.Add(group);
            }
        }

        /// <summary>
        /// Add one animation group
        /// </summary>
        /// <param name="group">AnimationGroup asset</param>
        public void Add(AltAnimationGroup group)
        {
            // Copy group if animations was instantiated before and animator was setup for instantiate
            if (instantiateAnimations && _initialized) group = Copy(group);

            List<AltAnimationGroup> list = (animationGroups == null) 
                ? new List<AltAnimationGroup>() : animationGroups.ToList();
            list.Add(group);
            animationGroups = list.ToArray();
        }
        
        /// <summary>
        /// Wait for end of all playing animations
        /// <example>
        /// animator.Play(run);
        /// await animator.Wait();
        /// </example>
        /// </summary>
        public async Task Wait()
        {
            if (animationGroups == null) return;
            await Task.Yield();
            foreach (var group in animationGroups.Where(g => g.Running))
                await  group.Wait();
        }
        
        /// <summary>
        /// Stop specific animation group
        /// </summary>
        /// <param name="id">Id of animation group</param>
        /// <param name="invokeFinishCallback">Invoke callback or not</param>
        public void Stop(string id, bool invokeFinishCallback)
        {
            if (animationGroups == null) return;
            
            if (animationGroups.Any(g => g.Id == id))
                animationGroups.First(g => g.Id == id).Stop(invokeFinishCallback);
            else
                Debug.LogWarning("Can't find animation. Please check animation id", this.gameObject);
        }
        
        /// <summary>
        /// Stop all groups
        /// </summary>
        /// <param name="invokeFinishCallback">Invoke callbacks or not</param>
        public void Stop(bool invokeFinishCallback)
        {
            if (animationGroups == null) return;
            
            foreach (var group in animationGroups.Where(g => g.Running))
            {
                group.Stop(invokeFinishCallback);
            }
        }

        private void OnDestroy()
        {
            this.Stop(false);
        }
    }
}
