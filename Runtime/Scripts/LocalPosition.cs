﻿using System;
using System.Linq;
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
        
        private Transform First => Components[0] as Transform;

        protected override bool PrepareTargets()
        {
            if (!base.PrepareTargets()) return false;
            
            if (First == null) return false;
            
            _start = First.localPosition;
          
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
            _finish = First.localPosition + finish;
        }

        protected override void MultiplyTarget()
        {
            _finish = Vector3.Cross(First.localPosition, finish);
        }

        protected override void Interpolate()
        {
            foreach (var trans in Components.Cast<Transform>())
            {
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
