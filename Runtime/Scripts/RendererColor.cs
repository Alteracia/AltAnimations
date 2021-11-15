using System;
using System.Linq;
using UnityEngine;

namespace Alteracia.Animations
{
    [CreateAssetMenu(fileName = "RendererColorAnimation", menuName = "AltAnimations/RendererColor", order = 5)]
    [System.Serializable]
    public class RendererColor : AltAnimation
    {
        [SerializeField]
        private Color start;
        [SerializeField] 
        private Color finish;
        [NonSerialized]
        private Color _start;
        [NonSerialized]
        private Color _finish;
        private Renderer First => Components[0] as Renderer;

        protected override bool PrepareTargets()
        {
            if (!base.PrepareTargets()) return false;
                
            if (First == null) return false;
            
            _start = First.material.color;
            
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
            _finish = First.material.color + finish;
        }

        protected override void MultiplyTarget()
        {
            _finish = First.material.color * finish; // TODO TEST
        }

        protected override void Interpolate()
        {
            foreach (var renderer in Components.Cast<Renderer>())
            {
                renderer.material.color = Color.Lerp(_start, _finish, Progress);
            }
        }

        public override System.Type GetComponentType()
        {
            return typeof(Renderer);
        }
        
        public override bool Equals(AltAnimation other)
        {
            return base.Equals(this, other);
        }
    }
}
