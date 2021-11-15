using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Alteracia.Animations
{
    [CreateAssetMenu(fileName = "ColorAnimation", menuName = "AltAnimations/GraphicColor", order = 5)]
    [System.Serializable]
    public class GraphicColor : AltAnimation
    {
        [SerializeField]
        private Color start;
        [SerializeField] 
        private Color finish;
        [NonSerialized]
        private Color _start;
        [NonSerialized]
        private Color _finish;
        private Graphic First => Components[0] as Graphic;

        protected override bool PrepareTargets()
        {
            if (!base.PrepareTargets()) return false;
                
            if (First == null) return false;
            
            _start = First.color;
            
            return true;
        }
        
        protected override void UpdateCurrentProgressFromStart()
        {
            Progress = Vector4.Distance(start, _start) / Vector4.Distance(start, finish);
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
            _finish = First.color + finish;
        }

        protected override void MultiplyTarget()
        {
            _finish = First.color * finish; // TODO TEST
        }

        protected override void Interpolate()
        {
            foreach (var graphic in Components.Cast<Graphic>())
            {
                graphic.color = Color.Lerp(_start, _finish, Progress);
            }
        }

        public override System.Type GetComponentType()
        {
            return typeof(Graphic);
        }
        
        public override bool Equals(AltAnimation other)
        {
            return base.Equals(this, other);
        }
    }
}
