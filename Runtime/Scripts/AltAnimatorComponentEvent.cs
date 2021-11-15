using Alteracia.Patterns.ScriptableObjects;
using UnityEngine;

namespace Alteracia.Animations
{
    [CreateAssetMenu(fileName = "AltAnimatorComponentEvent", menuName = "AltEvents/AltAnimatorComponentEvent", order = 2)]
    [System.Serializable]
    public class AltAnimatorComponentEvent : ComponentEvent<AltAnimator>
    {
    }
}