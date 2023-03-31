using UnityEngine;
using System;

namespace Project.UIFramework
{
    public abstract class ATransitionComponent : MonoBehaviour
    {
        public abstract void Animate(Transform target, Action callWhenFinished);
    }
}