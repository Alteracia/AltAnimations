using System;
using System.Linq;
using UnityEngine;

namespace Alteracia.Animations
{
    [CreateAssetMenu(fileName = "PositionZAnimation", menuName = "AltAnimations/PositionZ", order = 2)]
    [System.Serializable]
    public class PositionZ : AltAnimation
    {
        [SerializeField] 
        private float start;
        [SerializeField] 
        private float finish;
        [NonSerialized]
        private float _start;
        [NonSerialized]
        private float _finish;

        private Transform First => Components[0] as Transform;
        
        protected override bool PrepareTargets()
        {
            if (!base.PrepareTargets()) return false;
            
            if (First == null) return false;
            
            _start = First.position.z;
          
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
        
        protected override void OverwriteTarget()
        {
            _finish = finish;
        }
        
        protected override void AddTarget()
        {
            _finish = First.position.z + finish;
        }

        protected override void MultiplyTarget()
        {
            _finish = First.position.z * finish;
        }

        protected override void Interpolate()
        {
            foreach (var trans in Components.Cast<Transform>())
            {
                Vector3 pos = trans.position;
                trans.position = new Vector3(pos.x, pos.y, Mathf.Lerp(_start, _finish, Progress));
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