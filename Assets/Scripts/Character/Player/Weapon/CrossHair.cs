using System.Collections;
using TMPro.Examples;
using UnityEngine;
using UnityEngine.UI;

public class CrossHair : MonoBehaviour
{
    [SerializeField] private RectTransform _rightCrossHair;
    [SerializeField] private RectTransform _leftCrossHair;
    [SerializeField] private RectTransform _upCrossHair;
    [SerializeField] private RectTransform _downCrossHair;

    [SerializeField] private Transform _aimCenter;
    [Space(10f)]
    [SerializeField] private Image[] _sideCrossHair;
    private bool _isVisibility;
    private Coroutine _SideCrossHairEnableRoutine;


    public void Init(GunController gunController)
    {
        gunController.OnTargetDamaged += SideCrossHairEnable;
    }


    private void FixedUpdate()
    {
        AimCenterMove();
    }

    public void OnEnable()
    {
        _aimCenter.position = transform.position;
    }

    public void OnDisable()
    {
        for (int i = 0; i < _sideCrossHair.Length; i++)
        {
            Color color = new Color(0, 1, 0, 0);
            _sideCrossHair[i].color = color;
        }
    }

    public void Visibility()
    {
        if (_isVisibility == false)
        {
            _rightCrossHair.gameObject.SetActive(true);
            _leftCrossHair.gameObject.SetActive(true);
            _upCrossHair.gameObject.SetActive(true);
            _downCrossHair.gameObject.SetActive(true);
            _isVisibility = true;
        }
    }

    public void Hidden()
    {
        if (_isVisibility == true)
        {
            _rightCrossHair.gameObject.SetActive(false);
            _leftCrossHair.gameObject.SetActive(false);
            _upCrossHair.gameObject.SetActive(false);
            _downCrossHair.gameObject.SetActive(false);

            for (int i = 0; i < _sideCrossHair.Length; i++)
            {
                Color color = new Color(0, 1, 0, 0);
                _sideCrossHair[i].color = color;
            }
            _isVisibility = false;
        }
    }

    public void CrossHairAeraSize(float fireAccuracy)
    {
        float CrossHairDistance = 0.35f + (fireAccuracy * 2.5f);
        _rightCrossHair.anchoredPosition = new Vector2(CrossHairDistance, 0);
        _leftCrossHair.anchoredPosition = new Vector2(-CrossHairDistance, 0);
        _upCrossHair.anchoredPosition = new Vector2(0, CrossHairDistance);
        _downCrossHair.anchoredPosition = new Vector2(0, -CrossHairDistance);
    }


    public void SideCrossHairEnable()
    {
        if (_SideCrossHairEnableRoutine != null)
            StopCoroutine(_SideCrossHairEnableRoutine);

        _SideCrossHairEnableRoutine = StartCoroutine(IESideCrossHairEnable());
    }

    IEnumerator IESideCrossHairEnable()
    {
        for (int i = 0; i < _sideCrossHair.Length; i++)
        {
            Color color = new Color(0, 1, 0, 1);
            _sideCrossHair[i].color = color;
        }
        yield return YieldCache.WaitForSeconds(0.1f);

        float alpha = 1;
        while(alpha > 0)
        {
            alpha -= 0.2f;
            for (int i = 0; i < _sideCrossHair.Length; i++)
            {
                Color color = new Color(0, 1, 0, alpha);
                _sideCrossHair[i].color = color;
            }
            yield return YieldCache.WaitForSeconds(0.02f);
        }
    } 

    private void AimCenterMove()
    {
        Vector3 _aimNextPos = transform.position + -transform.forward;
        if(Vector3.Distance(_aimNextPos, _aimCenter.position) > 0.1f)
        {
            _aimCenter.position = Vector3.Lerp(_aimCenter.position, _aimNextPos, Time.deltaTime * 20f);
        }
    }
}
