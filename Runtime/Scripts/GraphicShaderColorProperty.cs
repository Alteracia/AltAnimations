using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Alteracia.Animations
{
    [CreateAssetMenu(fileName = "GraphicShaderColorPropertyAnimation", menuName = "AltAnimations/GraphicShaderColorProperty", order = 5)]
    [System.Serializable]
    public class GraphicShaderColorProperty : ShaderColorProperty
    {
        protected override bool CheckSharedMaterials()
        {
            // Do not update every run
            if (Materials != null && Materials.Length > 0) return true;

            List<Material> graphMaterials = new List<Material>();

            foreach (var graph in Components.Cast<Graphic>())
            {
                graphMaterials.Add(graph.materialForRendering);
            }

            Materials = graphMaterials.ToArray();

            return graphMaterials.Count > 0;
        }

        protected override System.Type GetComponentTypePrivate()
        {
            return typeof(Graphic);
        }
    }
}