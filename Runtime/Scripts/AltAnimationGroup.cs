using System;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

namespace Alteracia.Animations
{
    [CreateAssetMenu(fileName = "AnimationGroup", menuName = "AltAnimations/AnimationGroup", order = 1)]
    public class AltAnimationGroup : Patterns.ScriptableObjects.RootScriptableObject, IAnimation
    {
        [SerializeField]
        private string id;
        public string Id => id;

        [NonSerialized]
        private int _playingCount = -1;
        public bool Running => _playingCount > 0;

        private Action _finishCallback;

        public void Play(GameObject animator, Action finishCallback = null)
        {
            ChangeCallback(finishCallback);
            
            if (Nested.Count == 0) return;// || this.Running) return;
            
            _playingCount = Nested.Count;
            
            // Play all without callback
            foreach (var anim in Nested.Cast<AltAnimation>())
            {
                if (!anim) CountFinished();
                else anim.Play(animator, CountFinished);
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
            foreach (var anim in Nested.Cast<AltAnimation>())
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
            await Task.Yield();
            
            if (changeCallback && !this.Running) // TODO Check this
                ChangeCallback(finishCallback);
            
            // Wait all animations
            foreach (var animation in Nested.Cast<AltAnimation>().Where(a => a.Running))
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
            return (from anim in Nested.Cast<AltAnimation>()
                from otherAnim in other.Nested.Cast<AltAnimation>() where anim.Equals(otherAnim)
                select anim).Any();
        }

#if UNITY_EDITOR
        
        [ContextMenu("Add position animation")]
        private void AddPositionAnim() => AddNested<Position>();
        
        [ContextMenu("Add X position animation")]
        private void AddPositionXAnim() => AddNested<PositionX>();
        
        [ContextMenu("Add Y position animation")]
        private void AddPositionYAnim() => AddNested<PositionY>();
        
        [ContextMenu("Add Z position animation")]
        private void AddPositionZAnim() => AddNested<PositionZ>();

        [ContextMenu("Add local position animation")]
        private void AddLocalPositionAnim() => AddNested<LocalPosition>();
        
        [ContextMenu("Add anchored position animation")]
        private void AddAnchoredPositionAnim() => AddNested<AnchoredPosition>();
        
        [ContextMenu("Add rotation animation")]
        private void AddRotationAnim() => AddNested<Rotation>();
        
       // [ContextMenu("Add local rotation animation")]
       // private void AddLocalRotationAnim() => AddNested<LocalRotation>();
       
       // [ContextMenu("Add rect rotation animation")]
       // private void AddRectRotationAnim() => AddNested<RectRotation>();
        
       // [ContextMenu("Add scale animation")]
       // private void AddScaleAnim() => AddNested<Scale>();
       
       // [ContextMenu("Add local scale animation")]
       // private void AddLocalScaleAnim() => AddNested<LocalScale>();
       
        [ContextMenu("Add rect scale animation")]
        private void AddRectScaleAnim() => AddNested<RectScale>();
        
        [ContextMenu("Add method float animation")]
        private void AddMethodFloatAnim() => AddNested<FloatMethod>();

        [ContextMenu("Add renderer shader float animation")]
        private void AddRendererShaderFloatAnim() => AddNested<RendererShaderFloatProperty>();
        
        [ContextMenu("Add graphic shader float animation")]
        private void AddGraphicShaderFloatAnim() => AddNested<GraphicShaderFloatProperty>();
        
        [ContextMenu("Add renderer color animation")]
        private void AddRendererColorAnim() => AddNested<RendererColor>();
        
        [ContextMenu("Add graphic color animation")]
        private void AddGraphicColorAnim() => AddNested<GraphicColor>();
        
        [ContextMenu("Add canvas alpha animation")]
        private void AddCanvasAlphaAnim() => AddNested<CanvasGroupAlpha>();

        
#endif
       
    }
}
