using UnityEngine;
using UnityEngine.Serialization;

namespace Alteracia.Animation
{
    [CreateAssetMenu(fileName = "AnchoredPositionAnimation", menuName = "AltAnimations/AnchoredPosition", order = 3)]
    [System.Serializable]
    public class AnchoredPosition : AltAnimation
    {
        private RectTransform[] _transforms;
        
        [SerializeField]
        private Vector2 start;
        [FormerlySerializedAs("position")] [SerializeField]
        private Vector2 finish;
        
        private Vector2 _start;

        protected override bool PrepareTargets()
        {
            if (_transforms == null || _transforms.Length == 0)
                _transforms = System.Array.ConvertAll(components, item => (RectTransform)item);
            
            if (_transforms == null || _transforms.Length == 0) return false;

            _start = _transforms[0].anchoredPosition;
            
            return true;
        }
        
        protected override void UpdateCurrentProgressFromStart()
        {
            progress = Vector2.Distance(start, _start) / Vector2.Distance(start, finish);
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
                trans.anchoredPosition = Vector2.Lerp(_start, finish, progress);
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
