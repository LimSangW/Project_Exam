using System.Collections.Generic;
using UnityEngine;

namespace Project.UIFramework
{
    [CreateAssetMenu(fileName = "UISettings", menuName = "UIFramework/UI Settings")]
    public class UISettings : ScriptableObject
    {
        [SerializeField] private UIFrame templateUIPrefab = null;
        [SerializeField] private List<GameObject> screensToRegister = null;
        [SerializeField] private bool deactivateScreenGOs = true;

        public UIFrame CreateUIInstance(bool instanceAndRegisterScreens = true)
        {
            var newUI = Instantiate(templateUIPrefab);

            if (instanceAndRegisterScreens)
            {
                foreach (var screen in screensToRegister)
                {
                    var screenInstance = Instantiate(screen);
                    var screenController = screenInstance.GetComponent<IUIScreenController>();

                    if (screenController != null)
                    {
                        newUI.RegisterScreen(screen.name, screenController, screenInstance.transform);
                        if (deactivateScreenGOs && screenInstance.activeSelf)
                        {
                            screenInstance.SetActive(false);
                        }
                    }
                    else
                    {
                        DebugEx.LogError("[UIConfig] Screen doesn't contain a ScreenController! Skipping " + screen.name);
                    }
                }
            }

            return newUI;
        }

        private void OnValidate()
        {
            List<GameObject> objectsToRemove = new List<GameObject>();
            for (int i = 0; i < screensToRegister.Count; i++)
            {
                var screenCtl = screensToRegister[i].GetComponent<IUIScreenController>();
                if (screenCtl == null)
                {
                    objectsToRemove.Add(screensToRegister[i]);
                }
            }

            if (objectsToRemove.Count > 0)
            {
                DebugEx.LogError("[UISettings] Some GameObjects that were added to the Screen Prefab List didn't have ScreenControllers attached to them! Removing.");
                foreach (var obj in objectsToRemove)
                {
                    DebugEx.LogError("[UISettings] Removed " + obj.name + " from " + name + " as it has no Screen Controller attached!");
                    screensToRegister.Remove(obj);
                }
            }
        }
    }
}