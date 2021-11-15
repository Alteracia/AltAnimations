using System;
using UnityEngine;

namespace Alteracia.Animations
{
    [CreateAssetMenu(fileName = "LocalPositionAnimation", menuName = "AltAnimations/LocalPosition", order = 2)]
    [System.Serializable]
    public class LocalPosition : AltAnimation
    {
        [SerializeField] 
        private Vector3 start;
        [SerializeField] 
        private Vector3 finish;
        [NonSerialized]
        private Vector3 _start;
        [NonSerialized]
        private Vector3 _finish;

        protected override bool PrepareTargets()
        {
            if (!base.PrepareTargets()) return false;
            
            Transform any = Components[0] as Transform;
            if (any == null) return false;
            
            _start = any.localPosition;
          
            return true;
        }
        
        protected override void UpdateCurrentProgressFromStart()
        {
            Progress = Vector3.Distance(start, _start) / Vector3.Distance(start, finish);
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
            Transform first = Components[0] as Transform;
            _finish = first.localPosition + finish;
        }

        protected override void MultiplyTarget()
        {
            Transform first = Components[0] as Transform;
            _finish = Vector3.Cross(first.localPosition, finish); // TODO Check
        }

        protected override void Interpolate()
        {
            foreach (var comp in Components)
            {
                Transform trans = (Transform)comp;
                trans.localPosition = Vector3.Lerp(_start, _finish, Progress);
            }
        }
        
        public override System.Type GetComponentType()
        {
            return typeof(Transform);
        }
        
        public override bool Equals(AltAnimation other)
        {
            return base.Equals(this, other);
        }
    }
}
