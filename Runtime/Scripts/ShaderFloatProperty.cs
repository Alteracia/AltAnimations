using System;
using UnityEngine;

namespace Alteracia.Animations
{
    public abstract class ShaderFloatProperty : AltAnimation
    {
        // TODO by Shader or Materials name
        [SerializeField]
        private string property;
        [SerializeField]
        private float start;
        [SerializeField]
        private float finish;
        [NonSerialized]
        private float _start;
        [NonSerialized]
        private float _finish;

        [NonSerialized] protected Material[] Materials;
        
        private Material First => Materials[0];

        protected override bool PrepareTargets()
        {
            if (!base.PrepareTargets()) return false;

            if (!CheckSharedMaterials() || string.IsNullOrEmpty(property)) return false;
                
            if (First == null) return false;
            
            _start = First.GetFloat(property);
            
            return true;
        }

        protected abstract bool CheckSharedMaterials();

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
            _finish = First.GetFloat(property) + finish;
        }

        protected override void MultiplyTarget()
        {
            _finish = First.GetFloat(property) * finish; // TODO TEST
        }

        protected override void Interpolate()
        {
            foreach (var material in Materials)
            {
                material.SetFloat(property, Mathf.Lerp(_start, _finish, Progress));
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
                   && this.property == ((ShaderFloatProperty)other).property;
        }
    }
}
