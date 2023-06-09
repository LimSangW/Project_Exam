using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Project.UIFramework;
using Project.Utils;

public class NavigateToWindowSignal : ASignal<string, WindowProperties> { }
public class TogglePanelSignal : ASignal<string, bool, IPanelProperties> { }

public class UIManager : ManagerWithMono<UIManager>
{
    public bool IsInitialized { get; private set; }

    [Header("Common Infos")]
    private UIFrame uiFrame;
    private CanvasGroup canvasGroup;
    private List<IUIScreenController> prevScreens = new List<IUIScreenController>();

    [SerializeField] private UISettings uISettings;
    
    public static Canvas MainUICanvas = null;
    
    public override void Init()
    {
        base.Init();
        
        uiFrame = uISettings.CreateUIInstance();
        canvasGroup = uiFrame.GetComponent<CanvasGroup>();
        if (!canvasGroup)
            canvasGroup = uiFrame.gameObject.AddComponent<CanvasGroup>();
        MainUICanvas = uiFrame.GetComponent<Canvas>();
        MainUICanvas.planeDistance = 1.0F;

        Signals.Get<NavigateToWindowSignal>().AddListener(OnNavigateToWindow);
        Signals.Get<TogglePanelSignal>().AddListener(OnTogglePanel);
        
        IsInitialized = true;
    }

    public override void ClearData()
    {
        uiFrame = null;
        canvasGroup = null;
        
        Signals.Get<NavigateToWindowSignal>().RemoveListener(OnNavigateToWindow);
        Signals.Get<TogglePanelSignal>().RemoveListener(OnTogglePanel);
    }
    
    private void OnNavigateToWindow(string windowId, IWindowProperties props = null)
    {
        // You usually don't have to do this as the system takes care of everything
        // automatically, but since we're dealing with navigation and the Window layer
        // has a history stack, this way we can make sure we're not just adding
        // entries to the stack indefinitely
        string prevWindowId = uiFrame.WindowLayer.CurrentWindow == null
                                ? string.Empty
                                : uiFrame.WindowLayer.CurrentWindow.ScreenId;

        bool wasPrevWindowOpened = !string.IsNullOrEmpty(prevWindowId);
        if (wasPrevWindowOpened)
        {
            if (prevWindowId == windowId)
            {
                Debug.Log("PrevWindow is the same as requested window!");
                return;
            }

            uiFrame.CloseCurrentWindow();
        }

        if (!string.IsNullOrEmpty(windowId))
            uiFrame.OpenWindow(windowId);
    }
    
    private void OnTogglePanel(string panelId, bool isOn, IPanelProperties props = null)
    {
        if (!string.IsNullOrEmpty(panelId) && isOn == uiFrame.IsPanelOpen(panelId))
        {
            Debug.Log($"This panel({panelId}) is already on the toggle state({isOn})!");
            return;
        }

        if (isOn)
        {
            if (props != null)
            {
                uiFrame.ShowPanel(panelId, props);
            }
            else
            {
                uiFrame.ShowPanel(panelId);
            }
        }
        else
        {
            uiFrame.HidePanel(panelId);
        }
    }
}
