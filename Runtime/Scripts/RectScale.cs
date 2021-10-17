using System;
using System.Linq;
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

        private RectTransform First => Components[0] as RectTransform;
        
        protected override bool PrepareTargets()
        {
            if (!base.PrepareTargets()) return false;
            
            if (First == null) return false;
            
            _start = First.sizeDelta;
            
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

        protected override void OverwriteTarget()
        {
            _finish = finish;
        }

        protected override void AddTarget()
        {
            _finish = First.sizeDelta + finish;
        }

        protected override void MultiplyTarget()
        {
            _finish = First.sizeDelta * finish; // TODO TEST
        }

        protected override void Interpolate()
        {
            foreach (var trans in Components.Cast<RectTransform>())
            {
                trans.sizeDelta = Vector2.Lerp(_start, _finish, Progress);
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