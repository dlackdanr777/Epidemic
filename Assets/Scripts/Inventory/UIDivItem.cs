using System.Collections;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIDivItem : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _AmountText;
    [SerializeField] private TextMeshProUGUI _itemNameTMP;
    [SerializeField] private Button _okButton;
    [SerializeField] private Button _cancelButton;

    [SerializeField] private Button _valueAddButton;
    [SerializeField] private Button _valueSubButton;


    private UIInventory _uiInventory;
    private bool _isAddButtonClick;
    private bool _isSubButtonClick;
    private float _addButtonTimer;
    private float _subButtonTimer;
    private int AddMaxValue;

    private Coroutine _coroutine;

    public void Init(UIInventory uiInventory)
    {
        _uiInventory = uiInventory;
        EventTrigger.Entry addButtonDownEvent = new EventTrigger.Entry();
        EventTrigger.Entry addButtonUpEvent = new EventTrigger.Entry();
        addButtonDownEvent.eventID = EventTriggerType.PointerDown;
        addButtonUpEvent.eventID = EventTriggerType.PointerUp;
        addButtonDownEvent.callback.AddListener( (eventData) =>  onClickedAddButtonDown() );
        addButtonUpEvent.callback.AddListener((eventData) => onClickedAddButtonUp() );
        _valueAddButton.GetComponent<EventTrigger>().triggers.Add(addButtonDownEvent);
        _valueAddButton.GetComponent<EventTrigger>().triggers.Add(addButtonUpEvent);

        EventTrigger.Entry subButtonDownEvent = new EventTrigger.Entry();
        EventTrigger.Entry subButtonUpEvent = new EventTrigger.Entry();
        subButtonDownEvent.eventID = EventTriggerType.PointerDown;
        subButtonUpEvent.eventID = EventTriggerType.PointerUp;
        subButtonDownEvent.callback.AddListener((eventData) => onClickedSubButtonDown());
        subButtonUpEvent.callback.AddListener((eventData) => onClickedSubButtonUp());
        _valueSubButton.GetComponent<EventTrigger>().triggers.Add(subButtonDownEvent);
        _valueSubButton.GetComponent<EventTrigger>().triggers.Add(subButtonUpEvent);

        _cancelButton.onClick.AddListener(() => gameObject.SetActive(false));
        gameObject.SetActive(false);
    }

    private void FixedUpdate()
    {
        onClickedAddButton();
        onClickedSubButton();
    }


    public void SetActive(Item item)
    {
        if(item != null)
        {
            if (item.Amount > 1)
            {
                gameObject.SetActive(true);
                if (_coroutine != null)
                {
                    StopCoroutine(_coroutine);
                }
                _AmountText.text = "1";
                _itemNameTMP.text = item.Data.Name + " x" + item.Amount;
                AddMaxValue = item.Amount - 1;

                _coroutine = StartCoroutine(EnableInput());
                _okButton.onClick.RemoveAllListeners();
                _okButton.onClick.AddListener(() => DivItem(item));
                _okButton.onClick.AddListener(() => gameObject.SetActive(false));
                _okButton.onClick.AddListener(() => { if (_coroutine != null) StopCoroutine(_coroutine); });
            }
            else
            {
                Debug.Log("1개 이상을 가지고 있어야 나눌 수 있습니다.");
            }
        }
        else
        {
            Debug.Log("아이템이 없습니다.");
        }
    }


    private IEnumerator EnableInput()
    {
        while (true)
        {
            if(Input.GetKey(KeyCode.Return))
            {
                _okButton.onClick?.Invoke();
                yield break;
            }
            yield return YieldCache.WaitForSeconds(0.01f);
        }
    }


    public void DivItem(Item item)
    {
        Regex regexNumber = new Regex(@"[0-9]");
        bool isMatch = regexNumber.IsMatch(_AmountText.text); //숫자만 들어있나 확인
        if (isMatch && _AmountText.text != "") //만약 숫자만 들어있다면
        {
            int divAmount = int.Parse(_AmountText.text);
            if (divAmount != 0)
            {
                Inventory.Instance.DivItem(item, divAmount);
            }
            else
            {
                DebugLog.Log("0은  나눌 수 없습니다.");
            }
        }
        else
        {
            DebugLog.Log("숫자 이외는 입력 할 수 없습니다.");
        }
    }


    private void onClickedAddButtonUp()
    {
        _isAddButtonClick = false;
        _addButtonTimer = 0;
    }


    private void onClickedAddButtonDown()
    {
        _isAddButtonClick = true;
        AddValue();
    }


    private void onClickedAddButton()
    {
        if (_isAddButtonClick) { _addButtonTimer += Time.deltaTime; }
        if(_addButtonTimer > 0.5f)
        {
            AddValue();
        }
    }


    private void onClickedSubButtonUp()
    {
        _isSubButtonClick = false;
        _subButtonTimer = 0;
    }


    private void onClickedSubButtonDown()
    {
        _isSubButtonClick = true;
        SubValue();
    }

    private void onClickedSubButton()
    {
        if (_isSubButtonClick) { _subButtonTimer += Time.deltaTime; }
        if (_subButtonTimer > 0.5f)
        {
            SubValue();
        }
    }


    private void AddValue()
    {
        int tempValue = int.Parse(_AmountText.text);
        if (tempValue < AddMaxValue)
        {
            tempValue += 1;
            _AmountText.text = tempValue.ToString();
        }
    }

    private void SubValue()
    {
        int tempValue = int.Parse(_AmountText.text);
        if (tempValue > 1)
        {
            tempValue -= 1;
            _AmountText.text = tempValue.ToString();
        }
    }
}
