using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MoveBalls : MonoBehaviour
{
    [SerializeField] private GameObject _ballPrefab;

    private float _pathDistance;
    private float _ballDiameter;
    private List<BallSection> _sections;

    private void Start()
    {
        _pathDistance = GameManager.Instance.BgMath.GetDistance();
        _ballDiameter = _ballPrefab.transform.localScale.x;
        _sections = new List<BallSection> { new() };
        
        for (var i = 0; i < GameManager.Instance.BallCount; i++)
        {
            var sprite = GameManager.Instance.GetRandomSprite();

            while (_sections[0].Balls.Count >= 2 && _sections[0].Balls[^1].Sprite == sprite && _sections[0].Balls[^2].Sprite == sprite)
            {
                sprite = GameManager.Instance.GetRandomSprite();
            }

            _sections[0].Balls.Add(InstantiateBall(sprite, 0));
        }
    }

    private void Update()
    {
        if (GameManager.Instance.IsPaused())
        {
            return;
        }

        if (_sections.Count == 0)
        {
            GameManager.Instance.Win();
            return;
        }
        
        if (_sections[0].Balls[0].Distance >= _pathDistance)
        {
            GameManager.Instance.Lose();
            return;
        }

        MoveLastSectionBalls();
        JoinStoppedSections();
    }

    public void AddNewBallOnHit(Ball hittedBall, Sprite sprite)
    {
        for (var sectionIndex = 0; sectionIndex < _sections.Count; sectionIndex++)
        {
            var sectionBalls = _sections[sectionIndex].Balls;
            var ballIndex = sectionBalls.IndexOf(hittedBall);

            if (ballIndex != -1)
            {
                // pridani kulicky na pozici zasahu
                sectionBalls.Insert(ballIndex, InstantiateBall(sprite, hittedBall.Distance, hittedBall.transform.GetSiblingIndex()));

                // hledani barev
                if (!RemoveMatchedColors(sectionIndex, ballIndex)) // barva se nenasla
                {
                    // je potreba napozicovat pridanou a predchozi kulicky
                    for (var i = ballIndex; i >= 0; i--)
                    {
                        sectionBalls[i].Distance += _ballDiameter;
                    }
                }

                break;
            }
        }
    }

    private void MoveLastSectionBalls()
    {
        var lastSectionBalls = _sections[^1].Balls;
        var distance = lastSectionBalls[0].Distance + GameManager.Instance.BallSpeed * Time.deltaTime;

        for (var i = 0; i < lastSectionBalls.Count; i++)
        {
            lastSectionBalls[i].Distance = distance - i * _ballDiameter;
        }
    }

    private void JoinStoppedSections()
    {
        for (var sectionIndex = 0; sectionIndex < _sections.Count - 1; sectionIndex++)
        {
            var currentSection = _sections[sectionIndex];
            var currentSectionBalls = currentSection.Balls;
            var nextSection = _sections[sectionIndex + 1];
            var nextSectionBalls = nextSection.Balls;
            var sectionsDist = currentSectionBalls[^1].Distance - nextSectionBalls[0].Distance;

            if (sectionsDist <= _ballDiameter)
            {
                var ballIndex = currentSectionBalls.Count - 1;

                currentSectionBalls.AddRange(nextSectionBalls);

                _sections.Remove(nextSection);

                if (!RemoveMatchedColors(sectionIndex, ballIndex))
                {
                    for (var i = ballIndex; i >= 0; i--)
                    {
                        currentSectionBalls[i].Distance = currentSectionBalls[i + 1].Distance + _ballDiameter;
                    }
                }

                break;
            }
        }
    }

    private bool RemoveMatchedColors(int sectionIndex, int ballIndex)
    {
        var sectionBalls = _sections[sectionIndex].Balls;
        var sprite = sectionBalls[ballIndex].Sprite;
        var frontIndex = 0;
        var backIndex = 0;

        for (var i = ballIndex; i >= 0; i--)
        {
            if (sprite == sectionBalls[i].Sprite) frontIndex = i;
            else break;
        }

        for (var i = ballIndex; i < sectionBalls.Count; i++)
        {
            if (sprite == sectionBalls[i].Sprite) backIndex = i;
            else break;
        }

        var count = backIndex - frontIndex + 1;

        if (count >= 3)
        {
            // odstraneni kulicek z okna
            sectionBalls.GetRange(frontIndex, count).ForEach(x => Destroy(x.gameObject));

            // prekopani sekcí
            _sections.RemoveAt(sectionIndex);
            _sections.Insert(sectionIndex, new BallSection(sectionBalls.Take(frontIndex)));
            _sections.Insert(sectionIndex + 1, new BallSection(sectionBalls.Skip(backIndex + 1)));
            _sections.RemoveAll(x => x.Balls.Count == 0);

            return true;
        }

        return false;
    }

    private Ball InstantiateBall(Sprite sprite, float distance, int? siblingIndex = null)
    {
        var ball = Instantiate(_ballPrefab, transform).GetComponent<Ball>();
        ball.Sprite = sprite;
        ball.Distance = distance;

        if (siblingIndex.HasValue)
        {
            ball.transform.SetSiblingIndex(siblingIndex.Value);
        }

        return ball;
    }
}