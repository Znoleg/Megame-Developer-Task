using System;
using System.Collections.Generic;
using UnityEngine;

public class ScoreCounter : Singleton<ScoreCounter>
{
    [SerializeField] private List<AsteroidScore> _asteroidScores = new List<AsteroidScore>();
    [SerializeField] private int _ufoScore;

    public int Score { get; private set; } = 0;

    public event Action<int> OnScoreChange;

    private void Start()
    {
        AsteroidsDestroyer.Instance.OnAsteroidDestroy += AddAsteroidScore;
        UFOSpawner.Instance.OnUFODeath += AddUFOScore;
    }

    private void AddUFOScore() => AddScore(_ufoScore);

    private void AddAsteroidScore(AsteroidSize asteroidSize) 
        => AddScore(_asteroidScores.Find(pair => pair.AsteroidSize == asteroidSize).Score);

    private void AddScore(int addScore)
    {
        Score += addScore;
        OnScoreChange?.Invoke(Score);
    }

    private void OnDestroy()
    {
        AsteroidsDestroyer.Instance.OnAsteroidDestroy -= AddAsteroidScore;
        UFOSpawner.Instance.OnUFODeath -= AddUFOScore;
    }

    [Serializable]
    private class AsteroidScore
    {
        [SerializeField] private AsteroidSize _asteroidSize;
        [SerializeField] private int _score;

        public AsteroidSize AsteroidSize => _asteroidSize;
        public int Score => _score;
    }
}

