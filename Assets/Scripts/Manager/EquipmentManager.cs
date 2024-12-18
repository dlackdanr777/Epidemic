using Muks.DataBind;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipmentManager : MonoBehaviour
{
    public static EquipmentManager Instance
    {
        get
        {
            if (_instance == null)
            {
                GameObject obj = new GameObject("EquipmentManager");
                _instance = obj.AddComponent<EquipmentManager>();
                DontDestroyOnLoad(obj);
            }

            return _instance;
        }
    }

    private static EquipmentManager _instance;

    public float Speed => Mathf.Clamp(_playerDefalutSpeed + _equipAddSpeed, 0.1f, 10f);
    public int Armor => Mathf.Clamp( _equipAddArmor, -100, 100);


    private float _equipAddSpeed;
    private int _equipAddArmor;
    private float _playerDefalutSpeed;

    public void SetPlayerSpeed(float speed)
    {
        _playerDefalutSpeed = speed;
        DataBind.SetTextValue("PlayerSpeed", Speed.ToString());
    }


    private void Awake()
    {
        if (_instance != null)
            return;

        _instance = this;
        UserInfo.OnChangeEquipItemHandler += OnChangeEquipmentEvent;
        OnChangeEquipmentEvent();
    }



    private void OnChangeEquipmentEvent()
    {
        _equipAddSpeed = 0;
        _equipAddArmor = 0;
        for (int i = 0, cnt = (int)EquipItemType.Length; i < cnt; ++i)
        {
            EquipItemType type = (EquipItemType)i;
            EquipmentItem item = UserInfo.GetEquipItem(type);

            if (item == null)
                continue;

            if(item.Data is IPlayerChangeArmor changeArmor)
                _equipAddArmor += changeArmor.Armor;

            if(item.Data is IPlayerChangeSpeed changeSpeed)
                _equipAddSpeed += changeSpeed.Speed;
        }

        DataBind.SetTextValue("PlayerArmor", Armor.ToString());
        DataBind.SetTextValue("PlayerSpeed", Speed.ToString());
    }

}
