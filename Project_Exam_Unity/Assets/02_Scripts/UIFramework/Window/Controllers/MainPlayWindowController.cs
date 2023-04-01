using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Project.UIFramework;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.CullingGroup;

public class MainPlayWindowController : AWindowController
{
    [SerializeField] private RectTransform gridLayout;
    private List<Button> buttons;
    protected override void Awake()
    {
        base.Awake();

        buttons = gridLayout.GetComponentsInChildren<Button>().ToList();

        for (int i = 0; i < buttons.Count(); i++)
        {
            string strText = buttons[i].gameObject.name;
            strText = Regex.Replace(strText, @"\D", "");
            int nTmp = int.Parse(strText);
            buttons[i].onClick.AddListener(() => { OnClickButton(nTmp); });
        }
        
        void OnClickButton(int tValue) 
        {
            InGameManager.Instance.MainPlayer.CurrentDirView = tValue;
        }
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
        
    }

    protected override void WhileHiding()
    {
    }

    //////////////////////////////////////////
    // Listeners
    //////////////////////////////////////////

    public void OnClickAdvCoupon()
    {
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
