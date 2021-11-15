using Alteracia.Patterns.ScriptableObjects;
using UnityEngine;

namespace Alteracia.Animations
{
    [CreateAssetMenu(fileName = "AltAnimatorObjectEvent", menuName = "AltEvents/AltAnimatorObjectEvent", order = 2)]
    [System.Serializable]
    public class AltAnimatorObjectEvent : ObjectEvent<AltAnimator>
    {
    }
}