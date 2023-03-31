using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Project.UIFramework
{
    public class PanelUILayer : AUILayer<IPanelController>
    {
        [SerializeField]
        private PanelPriorityLayerList priorityLayers = null;

        public override void ReparentScreen(IUIScreenController controller, Transform screenTransform)
        {
            var ctl = controller as IPanelController;
            if (ctl != null)
            {
                ReparentToParaLayer(ctl.Priority, screenTransform);
            }
            else
            {
                base.ReparentScreen(controller, screenTransform);
            }
        }

        public override void ShowScreen(IPanelController screen)
        {
            screen.Show();
        }

        public override void ShowScreen<TProps>(IPanelController screen, TProps properties)
        {
            screen.Show(properties);
        }

        public override void HideScreen(IPanelController screen)
        {
            screen.Hide();
        }

        public bool IsPanelVisible(string panelId)
        {
            IPanelController panel;
            if (registeredScreens.TryGetValue(panelId, out panel))
            {
                return panel.IsVisible;
            }

            return false;
        }

        private void ReparentToParaLayer(PanelPriority priority, Transform screenTransform)
        {
            Transform trans;
            if (!priorityLayers.ParaLayerLookup.TryGetValue(priority, out trans))
            {
                trans = transform;
            }

            screenTransform.SetParent(trans, false);
        }
    }
}