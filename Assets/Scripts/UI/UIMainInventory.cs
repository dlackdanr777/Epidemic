using Muks.Tween;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Muks.PcUI;

public class UIMainInventory : PcUIView
{
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
        gameObject.SetActive(false);
    }

    public override void Show()
    {
        VisibleState = VisibleState.Appearing;
        gameObject.SetActive(true);
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
}
