using System;
using System.Collections.Generic;
using UnityEngine;

namespace Alteracia.Animation
{
    [CreateAssetMenu(fileName = "FloatMethodAnimation", menuName = "AltAnimations/FloatMethodAnimation", order = 6)]
    [Serializable]
    public class FloatMethod : AltAnimation
    {
        [SerializeField]
        private string componentType;
        [SerializeField] 
        private string method;
        [SerializeField] 
        private string property;

        [SerializeField] 
        private float start;
        [SerializeField] 
        private float alpha;
        
        private Action<float>[] _methods;

        private float _start;
        
        protected override bool PrepareTargets()
        {
            if (string.IsNullOrEmpty(method) ||  string.IsNullOrEmpty(property)) return false;
            
            if (_methods == null || _methods.Length == 0)
            {
                List<Action<float>> actions = new List<Action<float>>();
                foreach (var component in components)
                {
                    if (component == null)
                    {
                        Debug.LogWarning("Components of " + this.name + " is invalid");
                        continue;
                    }
                    var mi = component.GetType().GetMethod(method);
                    if (mi == null)
                    {
                        Debug.LogWarning("Can't find \"" + method + "\" Method in component " + component);
                        continue;
                    }

                    actions.Add((Action<float>) Delegate.CreateDelegate(typeof(Action<float>), component, mi));
                }

                if (actions.Count == 0) return false;

                _methods = actions.ToArray();
            }
            
            var prop = components[0].GetType().GetProperty(property);
            if (prop == null)
            {
                Debug.LogWarning("No \"" + property + "\" Property in component " + components[0]);
                return false;
            }
            
            _start = (float)prop.GetValue(components[0]);
            
            return true;
        }
        
        protected override void UpdateCurrentProgressFromStart()
        {
            progress = (_start - start) / (alpha - start);
        }

        protected override void SetConstantStart()
        {
            _start = start;
        }

        protected override void Interpolate()
        {
            base.Interpolate();

            foreach (var action in _methods)
            {
                action.Invoke(Mathf.Lerp(_start, alpha, progress));
            }
        }
        
        public override System.Type GetComponentType()
        {
            Type type = null;
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (var assembly in assemblies)
            {
                type = assembly.GetType(componentType);
                if (type != null)
                    break;
            }
            return type;
        }
        
        public override bool Equals(AltAnimation other)
        {
            return base.Equals(this, other)
                   && this.componentType == ((FloatMethod)other).componentType
                   && this.property == ((FloatMethod)other).property;
        }
    }
}