using System.Linq;
using UnityEngine;

namespace Alteracia.Animations
{
    [CreateAssetMenu(fileName = "CgAlphaAnimation", menuName = "AltAnimations/CanvasGroupAlpha", order = 4)]
    [System.Serializable]
    public class CanvasGroupAlpha : AltAnimation
    {
        [SerializeField]
        private float start;
        [SerializeField] 
        private float finish;
        
        [System.NonSerialized]
        private float _start;
        [System.NonSerialized]
        private float _finish;
        
        private CanvasGroup First => Components[0] as CanvasGroup;

        protected override bool PrepareTargets()
        {
            if (!base.PrepareTargets()) return false;

            if (First == null) return false;
            
            _start = First.alpha;
            
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
            _finish = First.alpha + finish;
        }

        protected override void MultiplyTarget()
        {
            _finish = First.alpha * finish;
        }

        protected override void Interpolate()
        {
            foreach (var canvasGroup in Components.Cast<CanvasGroup>())
            {
                canvasGroup.alpha = Mathf.Lerp(_start, _finish, Progress);
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
