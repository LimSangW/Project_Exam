using UnityEngine;

namespace Project.UIFramework
{
    [System.Serializable]
    public class PanelProperties : IPanelProperties
    {
        [SerializeField]
        private PanelPriority priority;

        public PanelPriority Priority
        {
            get { return priority; }
            set { priority = value; }
        }
    }
}