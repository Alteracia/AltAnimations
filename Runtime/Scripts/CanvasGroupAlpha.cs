using UnityEngine;
using UnityEngine.Serialization;

namespace Alteracia.Animation
{
    [CreateAssetMenu(fileName = "CgAlphaAnimation", menuName = "AltAnimations/CanvasGroupAlpha", order = 4)]
    [System.Serializable]
    public class CanvasGroupAlpha : AltAnimation
    {
        [Header("Target")]
        private CanvasGroup[] _canvasGroups;
        [SerializeField]
        private float start;
        [SerializeField] 
        private float finish;
        
        private float _start;

        protected override bool PrepareTargets()
        {
            base.PrepareTargets();
            
            if (_canvasGroups == null || _canvasGroups.Length == 0) 
                return false;
            
            _start = _canvasGroups[0].alpha;
            
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
        
        protected override void Interpolate()
        {
            base.Interpolate();
            
            if (_canvasGroups == null || _canvasGroups.Length == 0) return;
            foreach (var canvasGroup in _canvasGroups)
            {
                canvasGroup.alpha = Mathf.Lerp(_start, finish, Progress);
            }
        }
        
        public override System.Type GetComponentType()
        {
            return typeof(CanvasGroup);
        }
        
        public override bool Equals(AltAnimation other)
        {
            return base.Equals(this, other);
        }
    }
}
