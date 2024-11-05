using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance
    {
        get
        {
            if (_instance == null)
            {
                GameObject obj = new GameObject("UIManager");
                _instance = obj.AddComponent<UIManager>();
                DontDestroyOnLoad(obj);
            }

            return _instance;
        }
    }

    private static UIManager _instance;

    private static UIManagerCanvas _uiManagerCanvas;

    public void Awake()
    {
        if (_instance != null)
            return;

        _instance = this;
        DontDestroyOnLoad(gameObject);
        if (_uiManagerCanvas != null)
            return;

        UIManagerCanvas uIManagerCanvas = Resources.Load<UIManagerCanvas>("UI/UIManagerCanvas");
        _uiManagerCanvas = Instantiate(uIManagerCanvas, transform);

        LoadingSceneManager.OnChangeSceneHandler += OnChangeSceneEvent;
    }

    public void ShowRightText(string textContent)
    {
        _uiManagerCanvas.ShowRightText(textContent);
    }

    public void HiddenRightText()
    {
        _uiManagerCanvas.HideRightText();
    }


    public void ShowCenterText(string textContent)
    {
        _uiManagerCanvas.ShowCenterText(textContent);
    }

    private void OnChangeSceneEvent()
    {
        _uiManagerCanvas.HideCenterText();
        _uiManagerCanvas.HideRightText();
    }
}
