﻿using System;
using System.Linq;
using UnityEngine;

namespace Alteracia.Animations
{
    [CreateAssetMenu(fileName = "RotationAnimation", menuName = "AltAnimations/Rotation", order = 2)]
    [System.Serializable]
    public class Rotation : AltAnimation
    {
        /// <summary>
        /// Rotation at start
        /// </summary>
        [SerializeField]
        private Quaternion start;
        /// <summary>
        /// Rotation for finish
        /// </summary>
        [SerializeField]
        private Quaternion finish;
        
        [NonSerialized]
        private Quaternion _start;
        [NonSerialized]
        private Quaternion _finish;
        
        private Transform First => Components[0] as Transform;
        
        protected override bool PrepareTargets()
        {
            if (!base.PrepareTargets()) return false;
            
            if (First == null) return false;
            
            _start = First.rotation;

            return true;
        }

        protected override void UpdateCurrentProgressFromStart()
        {
            Progress = Quaternion.Angle(start, (Quaternion)_start) / Quaternion.Angle(start, finish);
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
            _finish = Quaternion.Euler(First.rotation.eulerAngles + finish.eulerAngles);
        }

        protected override void MultiplyTarget()
        {
            _finish = First.rotation * finish;
        }

        protected override void Interpolate()
        {
            foreach (var trans in Components.Cast<Transform>())
            {
                trans.rotation = Quaternion.Lerp(_start, _finish, Progress);
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
