using UnityEngine;

namespace Alteracia.Animation
{
    [CreateAssetMenu(fileName = "RotationAnimation", menuName = "AltAnimations/Rotation", order = 2)]
    [System.Serializable]
    public class Rotation : AltAnimation
    {
        private Transform[] _transforms;
        [SerializeField]
        private Quaternion start;
        [SerializeField] 
        private Quaternion finish;
        
        private Quaternion _start;
        private Quaternion _finish;
        
        public override void ChangeFinish(object finish) // TODO Rid of
        {
            this.finish = (Quaternion)finish;
        }

        protected override bool PrepareTargets()
        {
            if (_transforms == null || _transforms.Length == 0)
                _transforms = System.Array.ConvertAll(components, item => (Transform)item);
            if (_transforms == null || _transforms.Length == 0) return false;
            
            _start = _transforms[0].rotation;

            return true;
        }

        protected override void UpdateCurrentProgressFromStart()
        {
            progress = Quaternion.Angle(start, _start) / Quaternion.Angle(start, finish);
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
            Vector3 euler = _transforms[0].rotation.eulerAngles + finish.eulerAngles;
            _finish = Quaternion.Euler(euler);
        }

        protected override void MultiplyTarget()
        {
            _finish = _transforms[0].rotation * finish;
        }

        protected override void Interpolate()
        {
            base.Interpolate();
            
            if (_transforms == null || _transforms.Length == 0) return;
            foreach (var trans in _transforms)
            {
                trans.rotation = Quaternion.Lerp(_start, _finish, progress);
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
