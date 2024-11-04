using System.Collections;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private GameObject _bulletModel;
    private float _speed;
    private float _distance;
    private LayerMask _layerMask;

    private Vector3 _tmpPos;
    private Vector3 _startPos;
    private Vector3 _targetPos;
    private bool _isStart;

    public void SetBullet(float speed, LayerMask mask, float discance, Vector3 targetPos)
    {
        _speed = speed;
        _layerMask = mask;
        _isStart = true;
        _startPos = transform.position;
        _tmpPos = transform.position;
        _distance = discance;
        _targetPos = targetPos;
        _bulletModel.gameObject.SetActive(true);
    }


    private void Update()
    {
        if (!_isStart)
            return;

        if(_distance <= Vector3.Distance(transform.position, _startPos))
        {
            DespawnBullet();
            return;
        }

        Ray ray = new Ray(transform.position, transform.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, 1f, _layerMask))
        {
            transform.position = _targetPos;
            DespawnBullet();
            return;
        }
        Vector3 nextPos = _tmpPos + transform.forward * _speed * Time.fixedDeltaTime;
        Vector3 dir = (nextPos - _tmpPos).normalized;
        float distance = Vector3.Distance(nextPos, _tmpPos);
        Debug.DrawRay(_tmpPos, dir * distance, Color.red, 9999);
        if (Physics.Raycast(_tmpPos, dir, out hit, distance, _layerMask))
        {
            transform.position = _targetPos;
            DespawnBullet();
            return;
        }
        _tmpPos = transform.position;
        transform.position = nextPos;
    }


    private void OnDisable()
    {
        StopAllCoroutines();
    }

    private void DespawnBullet()
    {
        if (!gameObject.activeSelf || !_isStart)
            return;

        _isStart = false;
        StartCoroutine(Despawn());
    }


    private void OnTriggerStay(Collider other)
    {
        if (!_isStart)
            return;

        if(((1 << other.gameObject.layer) & _layerMask) != 0)
        {
            DespawnBullet();
        }
    }

    private IEnumerator Despawn()
    {
        _bulletModel.gameObject.SetActive(false);
        yield return YieldCache.WaitForSeconds(0.1f);
        ObjectPoolManager.Instance.DespawnBullet(this);
    }


}
