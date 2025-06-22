using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

public class LoginEvents : MonoBehaviour
{
    private UIDocument document;
    private Button button;
    private List<Button> menuButtons = new List<Button>();
    private AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        document = GetComponent<UIDocument>();
        button = document.rootVisualElement.Q("StartButton") as Button;
        menuButtons = document.rootVisualElement.Query<Button>().ToList();
    }

    private void OnEnable()
    {
        foreach (var btn in menuButtons)
        {
            if(btn != null)
            {
                btn.RegisterCallback<ClickEvent>(OnAllButtonsClick);
            }
        }
        button.RegisterCallback<ClickEvent>(OnStartClick);
    }

    private void OnDisable()
    {
        foreach (var btn in menuButtons)
        {
            if (btn != null)
                btn.UnregisterCallback<ClickEvent>(OnAllButtonsClick);
        }
        button.UnregisterCallback<ClickEvent>(OnStartClick);
    }

    private void OnAllButtonsClick(ClickEvent evt)
    {
        audioSource.Play();
    }

    private void OnStartClick(ClickEvent evt)
    {
        SceneManager.LoadScene("House_Brith", LoadSceneMode.Single);
    }

}
