using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace Alteracia.Animations
{
    public enum TargetOperation { Overwrite, Add, Multiply }
    
    public interface IAnimation
    {
        void Play(GameObject animator, Action finishCallback = null);
        bool Running { get; }
        void ChangeCallback(Action finishCallback = null);
        Task Wait(bool changeCallback = false, Action finishCallback = null);
        void Stop(bool invokeFinishCallback);
    }
    
    [Serializable]
    public abstract class AltAnimation : Patterns.ScriptableObjects.NestedScriptableObject, IAnimation
    {
        /// <summary>
        /// Do nothing (false) or
        /// play animation again (true) when Play is called while animation is running
        /// </summary>
        [Header("Behavior")]
        [Tooltip("Do nothing (false) or play animation again (true) when Play is called while animation is running.")][SerializeField]
        private bool playOver = false;
        /// <summary>
        /// Play animation again after duration passed
        /// </summary>
        [Tooltip("Play animation again after duration passed")][SerializeField] 
        private bool loop = false;
        /// <summary>
        /// Play animation backward after duration passed
        /// </summary>
        [Tooltip("Play animation backward after duration passed")][SerializeField]
        private bool swing = false;
        
        private enum AnimationProperty { Duration, Speed, Start }
        /// <summary>
        /// Duration - duration always the same. Start property has no affect on animation.
        /// Speed - new duration will calculate from current path. Start should be specified correctly.
        /// Start - duration always the same. Animation always begin from start. Target will be set to start without lerp.
        /// </summary>
        [Header("Time")]
        [Tooltip("The way animation starts and end")][SerializeField] 
        private AnimationProperty constant;
        /// <summary>
        /// Delay in seconds once after Play called.
        /// Use curves instead easings to set delay in other place of timeline.
        /// </summary>
        [Tooltip("Delay in seconds once after Play called")][SerializeField][Range(0f, 5f)]
        private float delay = 0f;
        /// <summary>
        /// Duration in seconds
        /// </summary>
        [Tooltip("Duration in seconds")][SerializeField][Range(0.0001f, 5f)]
        private float duration = 1f;
        /// <summary>
        /// Duration combine with delay.
        /// How long will be animation playing.
        /// </summary>
        public float Duration => duration + delay;

        [NonSerialized]
        private float _calculatedDuration = 0f;
        /// <summary>
        /// Duration which was calculate before animation was last Played
        /// </summary>
        public float CalculatedDuration => _calculatedDuration;

        /// <summary>
        /// Type of easing.
        /// Linear - no easing.
        /// Curve - use AnimationCurve.
        /// </summary>
        [Header("Interpolation")]
        [Tooltip("Type of easing")][SerializeField]
        private Logic.AltMath.EasingType easing = Logic.AltMath.EasingType.Linear;
        /// <summary>
        /// Less efficient but more flexible way to set easings
        /// </summary>
        [Tooltip("Less efficient but more flexible way to set easings")][SerializeField]
        private AnimationCurve curve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

        /// <summary>
        /// If true: get all Components
        /// </summary>
        [Header("Target")]
        [Tooltip("If true: get all Components")][SerializeField]
        private bool multiComponents = false;
        /// <summary>
        /// If true: exclude parent gameObject
        /// </summary>
        [Tooltip("If true: exclude parent gameObject")][SerializeField]
        private bool excludeSelf = false;
        /// <summary>
        /// Name of object to get target Component from.
        /// If name is empty: first Component of animator or its child will be set to target.
        /// If no object found: animation will be exclude.
        /// </summary>
        [Tooltip("If empty: target will be the first Component, or all Components")][SerializeField]
        private string gameObjectName;
        /// <summary>
        /// Math operation witch will be applied to set target start.
        /// Overwrite, Add, Multiply
        /// </summary>
        [Tooltip("Math operation witch will be applied to set target start")][SerializeField]
        protected TargetOperation operation = TargetOperation.Overwrite;

        [NonSerialized]
        protected List<Component> Components = new List<Component>();
        [NonSerialized]
        protected float Progress;
        
        [NonSerialized] 
        private List<int> _animatorsIds = new List<int>();
        [NonSerialized]
        private float _timer;
        [NonSerialized]
        private Task<bool> _animationRun = null;
        public bool Running => _animationRun != null;
        [NonSerialized]
        private CancellationTokenSource _cancelTokenSource = new CancellationTokenSource();
        
        private delegate void Finished();
        private event Finished OnFinished;

        public abstract Type GetComponentType();
        
        public virtual bool Equals(AltAnimation other)
        {
            return Equals(this, other);
        }

        protected virtual bool Equals(AltAnimation self, AltAnimation other)
        {
            // compare components
            return self.GetType() == other.GetType()
                   && self.excludeSelf == other.excludeSelf
                   && self.multiComponents == other.multiComponents
                   && self.gameObjectName == other.gameObjectName;
        }

        private void Init(GameObject animator)
        {
            if (this.GetComponentType() == null)
            {
                Debug.LogError("Can't get Component Type of animation \"" + this.name + "\"", animator);
                return;
            }

            Component[] components = animator.GetComponentsInChildren(this.GetComponentType());
            
            if (this.excludeSelf) 
                components = components.Where(c => c.gameObject != animator).ToArray();
            if (!string.IsNullOrEmpty(this.gameObjectName))
                components = components.Where(c =>
                    c.gameObject.name == this.gameObjectName).ToArray();
            
            if (components.Length == 0)
            {
                Debug.LogWarning("Can't get "+ GetComponentType() + " of animation \"" + this.name + "\"", animator);
                return;
            }
            
            if (multiComponents) Components.AddRange(components);
            else Components.Add(components[0]);
            
            _animatorsIds.Add(animator.GetInstanceID());
        }

        public async void Play(GameObject animator = null, Action finishCallback = null)
        {
            if (animator && !_animatorsIds.Contains(animator.GetInstanceID()))
            {
                await Task.Yield(); // be sure that gameObject started
                Init(animator);
            }
            
            // Always change callback
            ChangeCallback(finishCallback);
           
            // IF Task is running and multi play not allowed - break
            if (_animationRun != null)
            {
                // Do nothing
                if (!this.playOver) return ;
                // Cancel running task and Play again
                this.Stop(false);
            }
            
            if (!this.PrepareTargets())
            {
                InvokeFinishCallback();
                return;
            }
            
            this.PrepareParameters();
           
            // Set Task
            _animationRun = this.RunAnimation();
            // Wait
            bool stopped = await _animationRun;
            // Set Task to null
            _animationRun = null;
            
            if (stopped) return;
            // Callback only if animation was finished by self, not canceled
            InvokeFinishCallback();
        }

        private void PrepareParameters()
        {
            _timer = -delay;
            
            switch (constant)
            {
                case AnimationProperty.Duration:
                    _calculatedDuration = duration;
                    break;
                case AnimationProperty.Speed:
                    UpdateCurrentProgressFromStart();
                    _calculatedDuration = duration * Logic.AltMath.Ease(this.easing, 1 - Progress, curve);
                    break;
                case AnimationProperty.Start:
                    _calculatedDuration = duration;
                    SetConstantStart();
                    break;
                default:
                    throw new System.ArgumentOutOfRangeException();
            }

            switch (operation)
            {
                case TargetOperation.Overwrite:
                    OverwriteTarget();
                    break;
                case TargetOperation.Add:
                    AddTarget();
                    break;
                case TargetOperation.Multiply:
                    MultiplyTarget();
                    break;
                default:
                    throw new System.ArgumentOutOfRangeException();
            }
            _cancelTokenSource = new CancellationTokenSource();
        }

        protected virtual bool PrepareTargets()
        {
            return Components != null && Components.Count > 0;
        }

        protected abstract void UpdateCurrentProgressFromStart();
        protected abstract void SetConstantStart();

        protected abstract void OverwriteTarget();
        protected abstract void AddTarget();
        protected abstract void MultiplyTarget();
        

        private async Task<bool> RunAnimation()
        {
            while (_timer < _calculatedDuration || loop || (swing && _timer < _calculatedDuration * 2f))
            {
                if (_cancelTokenSource.Token.IsCancellationRequested)
                    return true;
               
                _timer += Time.deltaTime;

                float alpha = (_timer < 0f ? 0f : _timer) / _calculatedDuration; // Delay
                if (loop || swing)
                    alpha = swing ? Mathf.PingPong(alpha, 1f) : Mathf.Repeat(alpha, 1f);
                else
                    alpha = Mathf.Clamp01(alpha);
                
                this.Progress = Logic.AltMath.Ease(easing, alpha);
                                
                this.Interpolate();
                
                await Task.Yield();
            }
            
            return false;
        }

        protected abstract void Interpolate();

        public void Stop(bool invokeFinishCallback)
        {
            _cancelTokenSource.Cancel();
            
            // AnimationFinished Callback
            if (invokeFinishCallback) this.OnFinished?.Invoke();
            
            this.OnFinished = null;
        }

        public async Task Wait(bool changeCallback = false, Action finishCallback = null)
        {
            if (changeCallback) ChangeCallback(finishCallback);
            if (_animationRun != null)
                await _animationRun;
        }

        public void InvokeFinishCallback() // TODO Private
        {
            this.OnFinished?.Invoke();
            this.OnFinished = null;
        }

        public void ChangeCallback(Action finishCallback = null)
        {
            this.OnFinished = null;
            if (finishCallback != null)
                this.OnFinished = finishCallback.Invoke;
        }
    }
}
