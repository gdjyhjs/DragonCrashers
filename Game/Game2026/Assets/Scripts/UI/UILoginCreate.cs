using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class UILoginCreate : MonoBehaviour
{
    public UIDocument docLoginMain;
    public UIDocument docLoginCreate;
    List<Button> btns;

    private void Awake()
    {
        btns = docLoginMain.rootVisualElement.Query<Button>().ToList();
    }

    private void OnEnable()
    {
        BtnRegisterCall();
    }

    private void OnDisable()
    {
        BtnUnRegisterCall();
    }

    private void BtnRegisterCall()
    {
        foreach (var btn in btns)
        {
            btn.RegisterCallback<ClickEvent>(UICommonTools.OnBtnClickSoure);
            switch (btn.name)
            {
                case "BtnOk":
                    btn.RegisterCallback<ClickEvent>(BtnClickOk);
                    break;
                case "BtnEsc":
                    btn.RegisterCallback<ClickEvent>(BtnClickEsc);
                    break;
            }
        }
    }

    private void BtnUnRegisterCall()
    {
        foreach (var btn in btns)
        {
            btn.UnregisterCallback<ClickEvent>(UICommonTools.OnBtnClickSoure);
            switch (btn.name)
            {
                case "BtnOk":
                    btn.UnregisterCallback<ClickEvent>(BtnClickOk);
                    break;
                case "BtnEsc":
                    btn.UnregisterCallback<ClickEvent>(BtnClickEsc);
                    break;
            }
        }
    }

    /// <summary>
    /// 点击新游戏
    /// </summary>
    private void BtnClickOk(ClickEvent evt)
    {
        docLoginMain.enabled = false;
        docLoginCreate.enabled = true;
    }

    /// <summary>
    /// 点击退出游戏
    /// </summary>
    private void BtnClickEsc(ClickEvent evt)
    {
        docLoginCreate.enabled = false;
        docLoginMain.enabled = true;
    }


}
