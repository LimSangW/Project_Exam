using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Project.UIFramework;
using Project.Utils;

public class NavigateToWindowSignal : ASignal<string, WindowProperties> { }

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

        IsInitialized = true;
    }

    public override void ClearData()
    {
        uiFrame = null;
        canvasGroup = null;
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
    }
}
