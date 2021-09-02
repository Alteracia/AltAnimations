using System;
using UnityEngine;

namespace Alteracia.Animation
{
    [CreateAssetMenu(fileName = "RectScaleAnimation", menuName = "AltAnimations/RectScale", order = 3)]
    [System.Serializable]
    public class RectScale : AltAnimation
    {
        [SerializeField]
        private Vector2 start;
        [SerializeField]
        private Vector2 finish;
        
        [NonSerialized]
        private Vector2 _start;
        [NonSerialized]
        private Vector2 _finish;

        protected override bool PrepareTargets()
        {
            if (!base.PrepareTargets()) return false;
            
            RectTransform any = Components[0] as RectTransform;
            if (any == null) return false;
            _start = any.sizeDelta;
            
            return true;
        }
        
        protected override void UpdateCurrentProgressFromStart()
        {
            Progress = Vector2.Distance(start, _start) / Vector2.Distance(start, finish);
        }

        protected override void SetConstantStart()
        {
            _start = start;
        }

        protected override void Interpolate()
        {
            foreach (var comp in Components)
            {
                RectTransform trans = (RectTransform)comp;
                trans.sizeDelta = Vector2.Lerp(_start, finish, Progress);
            }
        }
        
        public override System.Type GetComponentType()
        {
            return typeof(RectTransform);
        }
        
        public override bool Equals(AltAnimation other)
        {
            return base.Equals(this, other);
        }
    }
    
}