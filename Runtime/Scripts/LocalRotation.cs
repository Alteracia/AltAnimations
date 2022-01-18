﻿using System.Linq;
using UnityEngine;

namespace Alteracia.Animations
{
    [CreateAssetMenu(fileName = "LocalRotationAnimation", menuName = "AltAnimations/LocalRotation", order = 2)]
    [System.Serializable]
    public class LocalRotation : AltAnimation
    {
        /// <summary>
        /// Rotation at start
        /// </summary>
        [SerializeField]
        private Vector3 start;
        /// <summary>
        /// Rotation for finish
        /// </summary>
        [SerializeField]
        private Vector3 finish;
        
        [System.NonSerialized]
        private Quaternion _start;
        [System.NonSerialized]
        private Quaternion _finish;
        
        private Transform First => Components[0] as Transform;
        
        protected override bool PrepareTargets()
        {
            if (!base.PrepareTargets()) return false;
            
            if (First == null) return false;
            
            _start = First.localRotation;

            return true;
        }

        protected override void UpdateCurrentProgressFromStart()
        {
            var st = Quaternion.Euler(start);
            Progress = Quaternion.Angle(st, _start) / Quaternion.Angle(st, Quaternion.Euler(finish));
        }

        protected override void SetConstantStart()
        {
            _start = Quaternion.Euler(start);
        }

        protected override void OverwriteTarget()
        {
            _finish = Quaternion.Euler(finish);
        }

        protected override void AddTarget()
        {
            _finish = Quaternion.Euler(First.localRotation.eulerAngles + finish);
        }

        protected override void MultiplyTarget()
        {
            _finish = First.localRotation * Quaternion.Euler(finish);
        }

        protected override void Interpolate()
        {
            foreach (var trans in Components.Cast<Transform>())
            {
                trans.localRotation = Quaternion.Lerp(_start, _finish, Progress);
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
