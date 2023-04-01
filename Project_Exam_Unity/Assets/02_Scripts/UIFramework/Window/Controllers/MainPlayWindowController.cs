using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Project.UIFramework;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainPlayWindowController : AWindowController
{
    [SerializeField] private RectTransform gridLayout;
    [SerializeField] private Slider timeProgress;
    [SerializeField] private TextMeshProUGUI scoreText;

    private Timer gameTimer;
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
            InGameManager.Instance.MainPlayer.PlayerAction(tValue);
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
        gameTimer = TimerManager.Instance.GetTimer("GameTimer");
        scoreText.SetText($"점수 : 0");
        timeProgress.value = timeProgress.maxValue;
        
        if(gameTimer == null) return;
        
        gameTimer.updateCallback += (x) =>
        {
            float temp = (float)x / (float)gameTimer.MaxTime;
            timeProgress.value = timeProgress.maxValue * temp;
        };

        InGameManager.Instance.OnScored += (score) =>
        {
            scoreText.SetText($"점수 : {score}");
        };
    }

    protected override void WhileHiding()
    {
    }

    private void DoInTransitionFinished(IUIScreenController screen)
    {
    }
}
