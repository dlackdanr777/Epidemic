using Muks.PcUI;
using Muks.Tween;
using UnityEngine;

public class UIBuild : PcUIView
{
    [SerializeField] private Transform _gridLayout; //슬롯들을 자식으로 가지고있는 오브젝트
    [SerializeField] private BuildSystem _buildSystem;
    [SerializeField] private UIBuildSlot _slotPrefab;
    private UIBuildSlot[] _buildSlot;

    [Header("Animation Options")]
    [SerializeField] private GameObject _animeUI;
    [SerializeField] private CanvasGroup _canvasGroup;

    [Space]
    [SerializeField] private float _showDuration;
    [SerializeField] private TweenMode _showTweenMode;

    [Space]
    [SerializeField] private float _hideDuration;
    [SerializeField] private TweenMode _hideTweenMode;

    private Vector2 _tmpPos;

    public override void Init()
    {
        SetSlots();
        gameObject.SetActive(false);
    }

    public override void Show()
    {
        VisibleState = VisibleState.Appearing;
        gameObject.SetActive(true);
        _buildSystem.BuildDisable();
        _canvasGroup.interactable = false;
        _canvasGroup.alpha = 0;
        _animeUI.transform.position = _tmpPos != Vector2.zero ? _tmpPos : _animeUI.transform.position;

        TweenData tween = _canvasGroup.TweenAlpha(1, _showDuration, _showTweenMode);
        tween.OnComplete(() =>
        {
            VisibleState = VisibleState.Appeared;
            _canvasGroup.interactable = true;
        });

    }

    public override void Hide()
    {
        VisibleState = VisibleState.Disappearing;
        gameObject.SetActive(true);
        _canvasGroup.interactable = false;
        _canvasGroup.alpha = 1;

        _tmpPos = _animeUI.transform.position;

        TweenData tween = _canvasGroup.TweenAlpha(0, _hideDuration, _hideTweenMode);
        tween.OnComplete(() =>
        {
            VisibleState = VisibleState.Disappeared;
            _canvasGroup.interactable = true;
            gameObject.SetActive(false);
        });
    }



    private void SetSlots()
    {
        //건축물 정보 구조체 배열의 길이를 가져온다.
        int craftItemLength = _buildSystem.GetCraftItemLength();
        _buildSlot = new UIBuildSlot[craftItemLength];

        for (int i = 0, count = craftItemLength; i < count ; i++)
        {
            _buildSlot[i] = Instantiate(_slotPrefab, Vector3.zero, Quaternion.identity);
            _buildSlot[i].transform.parent = _gridLayout;
            _buildSlot[i].UpdateUI(_buildSystem.GetCraftItem(i));

            int index = i;
            _buildSlot[i].Button.onClick.AddListener(() => OnButtonClicked(index));
        }
    }


    private void OnButtonClicked(int index)
    {
        _buildSystem.SelectCraftItem(index);
        _uiNav.Pop("UIBuild");
    }


}
