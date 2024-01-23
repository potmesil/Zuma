using UnityEngine;

public class Ball : MonoBehaviour
{
    private SpriteRenderer _renderer;
    private float _distance;

    public Sprite Sprite
    {
        get => _renderer.sprite;
        set => _renderer.sprite = value;
    }
    public float Distance
    {
        get => _distance;
        set
        {
            _distance = value > 0 ? value : 0;
            transform.position = GameManager.Instance.BgMath.CalcPositionByDistance(_distance);
            
            if (_distance == 0)
            {
                gameObject.SetActive(false);
            }
            else if (!gameObject.activeSelf)
            {
                gameObject.SetActive(true);
            }
        }
    }

    private void Awake()
    {
        _renderer = GetComponent<SpriteRenderer>();
    }
}