using BansheeGz.BGSpline.Components;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField] private Sprite[] _sprites;
    [SerializeField] private int _ballCount;
    [SerializeField] private float _ballSpeed;
    [SerializeField] private float _bulletSpeed;
    [SerializeField] private int _bulletLoadTime;
    [SerializeField, Range(1, 5)] private int _lifeCount;

    [Header("Windows")]
    [SerializeField] private Transform _lifeWindow;
    [SerializeField] private GameObject _gameOverWindow;
    [SerializeField] private GameObject _retryWindow;
    [SerializeField] private GameObject _winWindow;
    [SerializeField] private GameObject _pauseWindow;

    public static GameManager Instance { get; private set; }
    public BGCcMath BgMath { get; private set; }
    public MoveBalls MoveBallsScript { get; private set; }
    public int BallCount => _ballCount;
    public float BallSpeed => _ballSpeed;
    public float BulletSpeed => _bulletSpeed;
    public int BulletLoadTime => _bulletLoadTime;

    private GameData _gameData;

    private void Awake()
    {
        Instance = this;
        BgMath = FindObjectOfType<BGCcMath>();
        MoveBallsScript = FindObjectOfType<MoveBalls>();

        var sceneIndex = SceneManager.GetActiveScene().buildIndex;

        _gameData = GameData.Load() ?? new GameData();

        if (_gameData.SceneIndex != sceneIndex)
        {
            _gameData.SceneIndex = sceneIndex;
            _gameData.LifeCount = _lifeCount;
            GameData.Save(_gameData);
        }

        for (var i = 0; i < _gameData.LifeCount; i++)
        {
            _lifeWindow.GetChild(i).gameObject.SetActive(true);
        }

        Time.timeScale = 1;
    }
    
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && !_gameOverWindow.activeSelf && !_retryWindow.activeSelf && !_winWindow.activeSelf)
        {
            if (_pauseWindow.activeSelf)
            {
                Time.timeScale = 1;
                _pauseWindow.SetActive(false);
            }
            else
            {
                Time.timeScale = 0;
                _pauseWindow.SetActive(true);
            }
        }
    }

    public void Win()
    {
        Time.timeScale = 0;
        _winWindow.SetActive(true);
    }

    public void Lose()
    {
        Time.timeScale = 0;

        _gameData.LifeCount -= 1;
        _lifeWindow.GetChild(_gameData.LifeCount).gameObject.SetActive(false);

        if (_gameData.LifeCount == 0)
        {
            GameData.Delete();
            _gameOverWindow.SetActive(true);
        }
        else
        {
            GameData.Save(_gameData);
            _retryWindow.SetActive(true);
        }
    }

    public bool IsPaused()
    {
        return Time.timeScale == 0;
    }

    public void MainMenuBtn_Click()
    {
        SceneManager.LoadScene(0);
    }

    public void RetryBtn_Click()
    {
        SceneManager.LoadScene(_gameData.SceneIndex);
    }

    public void NextLevelBtn_Click()
    {
        SceneManager.LoadScene(_gameData.SceneIndex + 1);
    }

    public void ContinueBtn_Click()
    {
        Time.timeScale = 1;
        _pauseWindow.SetActive(false);
    }

    public Sprite GetRandomSprite()
    {
        return _sprites[Random.Range(0, _sprites.Length)];
    }
}