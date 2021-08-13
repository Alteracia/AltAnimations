using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Alteracia.Animation
{
    [CreateAssetMenu(fileName = "ShaderAnimation", menuName = "AltAnimations/ShaderFloatProperty", order = 5)]
    [System.Serializable]
    public class ShaderFloatProperty : AltAnimation
    {
        private Material[] _sharedMaterials;
        enum MeshType
        {
            Renderer,
            Graphic
        }
        [SerializeField]
        private MeshType mesh;
        [SerializeField]
        private string property;
        [SerializeField]
        private float finish;
        
        private float _start;

        public override void ChangeFinish(object finish)
        {
            this.finish = (float)finish;
        }

        protected override bool PrepareTargets()
        {
            if (!TryGetSharedMaterials() 
                || string.IsNullOrEmpty(property) 
                || !_sharedMaterials[0]) return false;
            
            _start = _sharedMaterials[0].GetFloat(property);
            
            return true;
        }

        protected override void Interpolate()
        {
            base.Interpolate();
            
            if (!TryGetSharedMaterials() || string.IsNullOrEmpty(property)) return;
            
            foreach (var material in _sharedMaterials.Where(m => m))
            {
                material.SetFloat(property, Mathf.Lerp(_start, finish, progress));
            }
        }

        public override System.Type GetComponentType()
        {
            switch (mesh)
            {
                case MeshType.Renderer:
                    return typeof(Renderer);
                case MeshType.Graphic:
                    return typeof(Graphic);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public override bool Equals(AltAnimation other)
        {
            return base.Equals(this, other)
                   && this.mesh == ((ShaderFloatProperty)other).mesh
                   && this.property == ((ShaderFloatProperty)other).property;
        }

        private bool TryGetSharedMaterials()
        {
            // Do not update every run
            if (_sharedMaterials != null && _sharedMaterials.Length > 0) return true;
            // Invalid animation
            if (components == null || components.Length == 0) return false;
            
            switch (mesh)
            {
                case MeshType.Renderer:
                    List<Material> rendMaterials = new List<Material>();
                    
                    foreach (var component in components)
                    {
                        Renderer rend = (Renderer) component;
                        rendMaterials.AddRange(rend.sharedMaterials);
                    }
                    
                    _sharedMaterials = rendMaterials.ToArray();
                    
                    return rendMaterials.Count > 0;
                case MeshType.Graphic:
                    List<Material> graphMaterials = new List<Material>();
                    
                    foreach (var component in components)
                    {
                        Graphic graph = (Graphic) component;
                        // Get material for Rendering in case masked UI TODO test can be material changed
                        graphMaterials.Add(graph.materialForRendering);
                    }
                    
                    _sharedMaterials = graphMaterials.ToArray();
                    
                    return graphMaterials.Count > 0;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
