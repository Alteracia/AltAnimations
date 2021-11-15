using Alteracia.Patterns.ScriptableObjects;
using UnityEngine;

namespace Alteracia.Animations
{
    [CreateAssetMenu(fileName = "AltAnimatorTwoStateEvent", menuName = "AltEvents/AltAnimatorTwoStateEvent", order = 2)]
    [System.Serializable]
    public class AltAnimatorTwoStateEvent : TwoStateEvents<AltAnimator>
    {
    }
}