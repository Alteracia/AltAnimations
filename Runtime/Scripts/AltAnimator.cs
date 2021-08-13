using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

namespace Alteracia.Animation
{
    public class AltAnimator : MonoBehaviour
    {
        [SerializeField] 
        private bool instantiateAnimations = true;

        [SerializeField]
        private AltAnimationGroup[] animationGroups = null;
        
        
        private bool _initialized = false;

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
            /*
            if (initiateAnimations)
            {
                foreach (var group in animationGroups)
                {
                    InitGroup(group);
                }
            }
            */
            _initialized = true;
        }

        /*
        private void InitGroup(AltAnimationGroup group)
        {
            foreach (AltAnimation anim in group.Animations)
            {
                if (anim.GetComponentType() == null)
                {
                    Debug.LogError("Can't get Component Type of animation \"" + anim.name + "\"", this.gameObject);
                    continue;
                }

                if (string.IsNullOrEmpty(anim.GameObjectName))
                {
                    if (anim.MultiComponents)
                    {
                        if (anim.ExcludeSelf)
                            anim.Components = this.gameObject.GetComponentsInChildren(anim.GetComponentType())
                                .Where(a => a.gameObject != this.gameObject).ToArray();
                        else
                            anim.Components = this.gameObject.GetComponentsInChildren(anim.GetComponentType());
                    }
                    else
                    {
                        Component only = this.gameObject.GetComponentInChildren(anim.GetComponentType());
                        if (!only || (only.gameObject == this.gameObject && anim.ExcludeSelf))
                            continue;
                        anim.Components = new List<Component> {only}.ToArray();
                    }
                    continue;
                }
                
                if (anim.MultiComponents) // Get All with the same name
                    anim.Components = this.gameObject.GetComponentsInChildren(anim.GetComponentType()).
                     Where(c => c.gameObject.name == anim.GameObjectName && (!anim.ExcludeSelf || c.gameObject != this.gameObject)).ToArray();
                else
                {
                    Component[] components = this.gameObject.GetComponentsInChildren(anim.GetComponentType());
                    if (components == null || components.Length == 0 || !components.Any(c => c.gameObject.name == anim.GameObjectName && (!anim.ExcludeSelf || c.gameObject != this.gameObject)))
                        return;
                    Component first = components.First(c => c.gameObject.name == anim.GameObjectName && (!anim.ExcludeSelf || c.gameObject != this.gameObject));
                    var list = new List<Component> { first };
                    anim.Components = list.ToArray();
                }
            }
        }
*/
        
        private static AltAnimationGroup Copy(AltAnimationGroup group)
        {
            AltAnimationGroup newGroup = ScriptableObject.Instantiate(group);
            
            List<AltAnimation> newAnimations = new List<AltAnimation>();
            foreach (var anim in group.Animations)
            {
                if (anim == null) continue;
                AltAnimation newAnim = ScriptableObject.Instantiate(anim);
                newAnimations.Add(newAnim);
            }

            newGroup.Animations = newAnimations.ToArray();
            return newGroup;
        }
        
        public void Add(AltAnimationGroup[] groups)
        {
            foreach (AltAnimationGroup group in groups)
            {
                if (group == null) continue;
                this.Add(group);
            }
        }

        public void Add(AltAnimationGroup group)
        {
            if (instantiateAnimations)
            {
                group = Copy(group);
            }
            //InitGroup(group);
            
            List<AltAnimationGroup> list = (animationGroups == null) 
                ? new List<AltAnimationGroup>() : animationGroups.ToList();
            list.Add(group);
            animationGroups = list.ToArray();
        }
        
        public void Play(int id, System.Object target, Action finishCallback = null)
        {
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
                {
                    if (currentAnimG.HaveEqualAnimationWith(animG))
                    {
                        // Debug.Log(currentAnimG.name + " stop " + animG);
                        animG.Stop(true);
                    }
                }
                currentAnimG.ChangeFinish(target);
                currentAnimG.Play(this.gameObject, finishCallback);
            }
            else
                Debug.LogWarning("Can't find animation. Please check animation id", this.gameObject);
        }

        public void Play(int id, Action finishCallback = null)
        {
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
                {
                    if (currentAnimG.HaveEqualAnimationWith(animG))
                    {
                        // Debug.Log(currentAnimG.name + " stop " + animG);
                        animG.Stop(true);
                    }
                }
                currentAnimG.Play(this.gameObject, finishCallback);
            }
            else
                Debug.LogWarning("Can't find animation. Please check animation id", this.gameObject);
            
        }
       
        public void Stop(int id, bool invokeFinishCallback)
        {
            if (animationGroups == null) return;
            
            if (animationGroups.Any(g => g.Id == id))
                animationGroups.First(g => g.Id == id).Stop(invokeFinishCallback);
            else
                Debug.LogWarning("Can't find animation. Please check animation id", this.gameObject);
        }
        
        public void Stop(bool invokeFinishCallback)
        {
            if (animationGroups == null) return;
            
            foreach (var group in animationGroups.Where(g => g.Running))
            {
                group.Stop(invokeFinishCallback);
            }
        }

        public async Task Wait()
        {
            if (animationGroups == null) return;
            await Task.Yield(); // Await in case if Animation not start yet
            foreach (var group in animationGroups.Where(g => g.Running))
            {
                await  group.Wait();
            }
        }

        private void OnDestroy()
        {
            this.Stop(false);
        }
    }
}
