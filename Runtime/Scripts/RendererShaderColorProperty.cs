using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Alteracia.Animations
{
    [CreateAssetMenu(fileName = "RendererShaderColorPropertyAnimation", menuName = "AltAnimations/RendererShaderColorProperty", order = 5)]
    [System.Serializable]
    public class RendererShaderColorProperty : ShaderColorProperty
    {
        protected override bool CheckSharedMaterials()
        {
            // Do not update every run
            if (Materials != null && Materials.Length > 0) return true;
            
            List<Material> rendMaterials = new List<Material>();
                    
            foreach (var rend in Components.Cast<Renderer>())
            {
                rendMaterials.AddRange(rend.sharedMaterials);
            }
                    
            Materials = rendMaterials.ToArray();
                    
            return rendMaterials.Count > 0;
                
        }

        protected override System.Type GetComponentTypePrivate()
        {
            return typeof(Renderer);
        }
    }
}