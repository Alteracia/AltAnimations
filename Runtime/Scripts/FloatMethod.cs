using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace Alteracia.Animations
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
        private float finish;
        [NonSerialized] 
        private float _start;
        [NonSerialized] 
        private float _finish;
        [NonSerialized]
        private Action<float>[] _methods;
        [NonSerialized]
        private PropertyInfo _property;
        
        private float First => (float)_property.GetValue(Components[0]);

        protected override bool PrepareTargets()
        {
            if (string.IsNullOrEmpty(method) ||  string.IsNullOrEmpty(property)) return false;
            
            if (!base.PrepareTargets()) return false;
            
            if (!CheckMethods()) return false;
            
            _start = First;
            
            return true;
        }

        private bool CheckMethods()
        {
            if (_methods != null && _methods.Length != 0) return true;
            
            List<Action<float>> actions = new List<Action<float>>();
            foreach (var component in Components)
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

                actions.Add((Action<float>)Delegate.CreateDelegate(typeof(Action<float>), component, mi));
            }

            _methods = actions.ToArray();
            
            _property = Components[0].GetType().GetProperty(property);

            if (_property != null) return _methods.Length != 0;
            
            Debug.LogWarning("No \"" + property + "\" Property in component " + Components[0]);
            return false;
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
            _finish = First + finish;
        }

        protected override void MultiplyTarget()
        {
            _finish = First * finish;
        }

        protected override void Interpolate()
        {
            foreach (var action in _methods)
            {
                action.Invoke(Mathf.Lerp(_start, _finish, Progress));
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