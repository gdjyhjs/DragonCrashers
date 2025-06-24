using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class UILoginMain : MonoBehaviour
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
                case "BtnNew":
                    btn.RegisterCallback<ClickEvent>(BtnClickNew);
                    break;
                case "BtnStart":
                    btn.RegisterCallback<ClickEvent>(BtnClickStart);
                    break;
                case "BtnSetting":
                    btn.RegisterCallback<ClickEvent>(BtnClickSetting);
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
                case "BtnNew":
                    btn.UnregisterCallback<ClickEvent>(BtnClickNew);
                    break;
                case "BtnStart":
                    btn.UnregisterCallback<ClickEvent>(BtnClickStart);
                    break;
                case "BtnSetting":
                    btn.UnregisterCallback<ClickEvent>(BtnClickSetting);
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
    private void BtnClickNew(ClickEvent evt)
    {
        docLoginMain.enabled = false;
        docLoginCreate.enabled = true;
    }

    /// <summary>
    /// 点击继续游戏
    /// </summary>
    private void BtnClickStart(ClickEvent evt)
    {
    }

    /// <summary>
    /// 点击设置
    /// </summary>
    private void BtnClickSetting(ClickEvent evt)
    {
    }

    /// <summary>
    /// 点击退出游戏
    /// </summary>
    private void BtnClickEsc(ClickEvent evt)
    {
        // 开发模式下使用 Debug.Log 代替退出
#if UNITY_EDITOR
        Debug.Log("游戏已请求退出");
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit(); // 发布版本直接退出应用
#endif
    }


}
