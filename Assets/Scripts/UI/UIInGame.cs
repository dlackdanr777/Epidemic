using Muks.DataBind;
using Muks.PcUI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PcUINavigation))]
public class UIInGame : MonoBehaviour
{
    private PcUINavigation _uiNav;

    private void Awake()
    {
        _uiNav = GetComponent<PcUINavigation>();
        _uiNav.OnChangeViewHandler += OnViewChangeEvent;

        DataBind.SetUnityActionValue("ShowInventory", ShowInventory);
        DataBind.SetUnityActionValue("HideInventory", HideInventory);

        DataBind.SetUnityActionValue("ShowBuild", ShowBuild);
        DataBind.SetUnityActionValue("HideBuild", HideBuild);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            if (_uiNav.CheckActiveView("UIInventory"))
            {
                HideInventory();
                return;
            }

            ShowInventory();
        }

        if(Input.GetKeyDown(KeyCode.B))
        {
            if (_uiNav.CheckActiveView("UIBuild"))
            {
                HideBuild();
                return;
            }

            ShowBuild();
        }
    }

    private void ShowInventory()
    {
        _uiNav.Push("UIInventory");
    }

    private void HideInventory()
    {
        _uiNav.Pop("UIInventory");
    }

    private void ShowBuild()
    {
        _uiNav.Push("UIBuild");
    }

    private void HideBuild()
    {
        _uiNav.Pop("UIBuild");
    }

    private void OnViewChangeEvent()
    {
        if(_uiNav.Count == 0)
        {
            GameManager.Instance.CursorHidden();
            return;
        }

        GameManager.Instance.CursorVisible();
    }
}
