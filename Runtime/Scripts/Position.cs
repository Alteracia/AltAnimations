using UnityEngine;
using UnityEngine.Serialization;

namespace Alteracia.Animation
{
    [CreateAssetMenu(fileName = "PositionAnimation", menuName = "AltAnimations/Position", order = 2)]
    [System.Serializable]
    public class Position : AltAnimation
    {
        private Transform[] _transforms;
        [SerializeField] 
        private Vector3 start;
        [FormerlySerializedAs("position")] [SerializeField] 
        private Vector3 finish;
        
        private Vector3 _start;

        protected override bool PrepareTargets()
        {
            if (_transforms == null || _transforms.Length == 0)
                _transforms = System.Array.ConvertAll(components, item => (Transform)item);
            if (_transforms == null || _transforms.Length == 0) return false;
            
            _start = _transforms[0].position;
          
            return base.PrepareTargets();
        }
        
        protected override void UpdateCurrentProgressFromStart()
        {
            progress = Vector3.Distance(start, _start) / Vector3.Distance(start, finish);
        }

        protected override void SetConstantStart()
        {
            _start = start;
        }

        protected override void Interpolate()
        {
            base.Interpolate();
            
            if (_transforms == null || _transforms.Length == 0) return;
            foreach (var trans in _transforms)
            {
                trans.position = Vector3.Lerp(_start, finish, progress);
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
