using UnityEngine;
using UnityEngine.UI;

namespace Alteracia.Animation
{
    [CreateAssetMenu(fileName = "ColorAnimation", menuName = "AltAnimations/GraphicColor", order = 5)]
    [System.Serializable]
    public class GraphicColor : AltAnimation
    {
        private Graphic[] _graphics;
       
        [SerializeField]
        private Color start;
        [SerializeField] 
        private Color finish;
        
        private Color _start;

        protected override bool PrepareTargets()
        {
            if (!base.PrepareTargets()) return false;
                
            if (_graphics == null || _graphics.Length == 0) return false;
            
            _start = _graphics[0].color;
            
            return true;
        }
        
        protected override void UpdateCurrentProgressFromStart()
        {
            Progress = Vector4.Distance(start, _start) / Vector4.Distance(start, finish);
        }

        protected override void Interpolate()
        {
            base.Interpolate();
            
            if (_graphics == null || _graphics.Length == 0) return;
            
            foreach (var graphic in _graphics)
            {
                graphic.color = Color.Lerp(_start, finish, Progress);
            }
        }

        public override System.Type GetComponentType()
        {
            return typeof(Graphic);
        }
    }
}
