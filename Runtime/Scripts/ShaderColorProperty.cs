using System;
using UnityEngine;

namespace Alteracia.Animations
{
    public abstract class ShaderColorProperty : AltAnimation
    {
        // TODO by Shader or Materials name
        [SerializeField]
        private string property;
        [ColorUsage(true, true)][SerializeField]
        private Color start;
        [ColorUsage(true, true)][SerializeField]
        private Color finish;
        [NonSerialized][ColorUsage(true, true)]
        private Color _start;
        [NonSerialized][ColorUsage(true, true)]
        private Color _finish;

        [NonSerialized] protected Material[] Materials;
        
        private Material First => Materials[0];

        protected override bool PrepareTargets()
        {
            if (!base.PrepareTargets()) return false;

            if (!CheckSharedMaterials() || string.IsNullOrEmpty(property)) return false;
                
            if (First == null) return false;
            
            _start = First.GetVector(property);
            
            return true;
        }

        protected abstract bool CheckSharedMaterials();

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
            _finish = (Color)First.GetVector(property) + finish;
        }

        protected override void MultiplyTarget()
        {
            _finish = First.GetVector(property) * finish; // TODO TEST
        }

        protected override void Interpolate()
        {
            foreach (var material in Materials)
            {
                material.SetVector(property, Color.Lerp(_start, _finish, Progress));
            }
        }

        public override System.Type GetComponentType()
        {
            return GetComponentTypePrivate();
        }

        protected abstract System.Type GetComponentTypePrivate();

        public override bool Equals(AltAnimation other)
        {
            return base.Equals(this, other)
                   && this.property == ((ShaderColorProperty)other).property;
        }
    }
}
