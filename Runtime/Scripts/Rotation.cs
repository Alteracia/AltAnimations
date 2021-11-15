using System;
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
        
        protected override bool PrepareTargets()
        {
            if (!base.PrepareTargets()) return false;
            
            Transform any = Components[0] as Transform;
            if (any == null) return false;
            
            _start = any.rotation;

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
            Transform first = Components[0] as Transform;
            Vector3 euler = first.rotation.eulerAngles + finish.eulerAngles;
            _finish = Quaternion.Euler(euler);
        }

        protected override void MultiplyTarget()
        {
            Transform first = Components[0] as Transform;
            _finish = first.rotation * finish;
        }

        protected override void Interpolate()
        {
            foreach (var comp in Components)
            {
                Transform trans = (Transform)comp;
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
