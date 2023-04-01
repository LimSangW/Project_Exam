using System.Collections;
using System.Collections.Generic;
using Project.UIFramework;
using UnityEngine;

public class LobbyWindowController : AWindowController
{
    protected override void Awake()
    {
        base.Awake();
    }

    protected override void AddListeners()
    {
        InTransitionFinished += DoInTransitionFinished;
    }

    protected override void RemoveListeners()
    {
        InTransitionFinished -= DoInTransitionFinished;
    }

    protected override void OnPropertiesSet()
    {
        //show µ…∂ß ¿Ã∑Ô¡¸.
    }

    protected override void WhileHiding()
    {
    }

    //////////////////////////////////////////
    // Listeners
    //////////////////////////////////////////

    public void OnClickAdvCoupon()
    {
        //
    }

    public void OnClickCloseButton()
    {
    }

    public void CloseThisWindow()
    {
    }

    public void OnClickQuestion(int i)
    {
    }

    public void OnClickPurchaseButton(int idx)
    {
    }

    private void DoInTransitionFinished(IUIScreenController screen)
    {
    }

    public void OnUpdateProductBoxes()
    {
    }
}
