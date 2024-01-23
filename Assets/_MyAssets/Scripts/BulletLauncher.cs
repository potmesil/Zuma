using System.Collections;
using UnityEngine;

public class BulletLauncher : MonoBehaviour
{
    [SerializeField] private GameObject _bulletPrefab;

    private Bullet _bullet;

    private void Start()
    {
        InstantiateBullet();
    }

    private void Update()
    {
        if (GameManager.Instance.IsPaused())
        {
            return;
        }

        if (Input.GetMouseButtonDown(0) && _bullet)
        {
            _bullet.Fire();
            _bullet = null;
            StartCoroutine(Reload());
        }
    }

    private IEnumerator Reload()
    {
        yield return new WaitForSeconds(GameManager.Instance.BulletLoadTime);
        InstantiateBullet();
    }

    private void InstantiateBullet()
    {
        _bullet = Instantiate(_bulletPrefab, transform).GetComponent<Bullet>();
        _bullet.SetSprite(GameManager.Instance.GetRandomSprite());
    }
}