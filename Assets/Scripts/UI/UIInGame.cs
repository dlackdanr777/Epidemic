using Muks.DataBind;
using Muks.PcUI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PcUINavigation))]
public class UIInGame : MonoBehaviour
{
    [SerializeField] private BuildSystem _buildSystem;

    private PcUINavigation _uiNav;

    private void Awake()
    {
        _uiNav = GetComponent<PcUINavigation>();
        _uiNav.OnChangeViewHandler += OnViewChangeEvent;

        DataBind.SetUnityActionValue("ShowInventory", ShowInventory);
        DataBind.SetUnityActionValue("HideInventory", HideInventory);

        DataBind.SetUnityActionValue("ShowBuild", ShowBuild);
        DataBind.SetUnityActionValue("HideBuild", HideBuild);

        DataBind.SetUnityActionValue("ShowStop", ShowStop);
        DataBind.SetUnityActionValue("HideStop", HideStop);
                                          
        DataBind.SetUnityActionValue("ShowWin", ShowWin);
        DataBind.SetUnityActionValue("HideWin", HideWin);
                                          
        DataBind.SetUnityActionValue("ShowLose", ShowLose);
        DataBind.SetUnityActionValue("HideLose", HideLose);
    }

    private void Update()
    {

        if(Input.GetKeyDown(KeyCode.Escape))
        {
            if(_buildSystem.BuildingEnable)
            {
                _buildSystem.BuildDisable();
                return;
            }

            if(_uiNav.Count == 0)
            {
                ShowStop();
                return;
            }

            _uiNav.Pop();
        }

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

    private void ShowStop()
    {
        _uiNav.Push("UIStop");
    }

    private void HideStop()
    {
        _uiNav.Pop("UIStop");
    }

    private void ShowWin()
    {
        _uiNav.Push("UIWin");
    }

    private void HideWin()
    {
        _uiNav.Pop("UIWin");
    }

    private void ShowLose()
    {
        _uiNav.Push("UILose");
    }

    private void HideLose()
    {
        _uiNav.Pop("UILose");
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
