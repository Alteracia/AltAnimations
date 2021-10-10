using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

namespace Alteracia.Animation
{
    [CreateAssetMenu(fileName = "CgAlphaAnimation", menuName = "AltAnimations/CanvasGroupAlpha", order = 4)]
    [System.Serializable]
    public class CanvasGroupAlpha : AltAnimation
    {
        [SerializeField]
        private float start;
        [SerializeField] 
        private float finish;
        
        [NonSerialized]
        private float _start;
        [NonSerialized]
        private float _finish;

        protected override bool PrepareTargets()
        {
            if (!base.PrepareTargets()) return false;

            CanvasGroup any = Components[0] as CanvasGroup;
            
            if (any == null) return false;
            _start = any.alpha;
            
            return true;
        }

        protected override void UpdateCurrentProgressFromStart()
        {
            Progress = (_start - start) / (finish - start);
        }

        protected override void SetConstantStart()
        {
            _start = start;
        }
        
        protected override void Interpolate()
        {
            foreach (var canvasGroup in Components.Cast<CanvasGroup>())
            {
                canvasGroup.alpha = Mathf.Lerp(_start, finish, Progress);
            }
        }
        
        public override System.Type GetComponentType()
        {
            return typeof(CanvasGroup);
        }
        
        public override bool Equals(AltAnimation other)
        {
            return base.Equals(this, other);
        }
    }
}
