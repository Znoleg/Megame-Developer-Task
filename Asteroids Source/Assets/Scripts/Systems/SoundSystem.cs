using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundSystem : MonoBehaviour
{
    [SerializeField] private int _maxSoundsAtTime;
    [SerializeField] private AudioClip _playerShootSound;
    [SerializeField] private AudioClip _accelerateSound;
    [SerializeField] private AudioClip _flyingSaucerSound;
    [SerializeField] private AudioClip _smallExplosionSound;
    [SerializeField] private AudioClip _mediumExplosionSound;
    [SerializeField] private AudioClip _largeExplosionSound;
    private readonly List<AudioSource> _audioSources = new List<AudioSource>();

    private void Awake()
    {
        for (int i = 0; i < _maxSoundsAtTime; i++)
        {
            _audioSources.Add(gameObject.AddComponent<AudioSource>());
        }
    }

    private void Start()
    {
        PlayerMovement.Instance.OnMoveStart += PlayAccelerate;
        PlayerMovement.Instance.OnMoveStop += StopPlayAccelerate;
        PlayerBody.Instance.OnHit += PlayLargeExpolsion;
        BulletSpawner.Instance.OnBulletSpawn += PlayShoot;
        AsteroidsDestroyer.Instance.OnAsteroidDestroy += PlayBlowSound;
        UFOSpawner.Instance.OnUFOSpawn += PlayUFOSound;
        UFOSpawner.Instance.OnUFODeath += TryStopPlayUFOSound;
    }

    private void PlayLargeExpolsion(int lives)
    {
        PlayClip(_largeExplosionSound, false);
    }

    private void TryStopPlayUFOSound()
    {
        PlayClip(_largeExplosionSound, false);
        if (UFOSpawner.Instance.UFOCount == 0) StopClip(_flyingSaucerSound);
    }

    private void PlayUFOSound()
    {
        PlayClip(_flyingSaucerSound, true);
    }

    private void PlayBlowSound(AsteroidSize asteroidSize)
    {
        if (asteroidSize == AsteroidSize.Big) PlayClip(_largeExplosionSound, false);
        else if (asteroidSize == AsteroidSize.Medium) PlayClip(_mediumExplosionSound, false);
        else if (asteroidSize == AsteroidSize.Small) PlayClip(_smallExplosionSound, false);
    }

    private void PlayShoot() => PlayClip(_playerShootSound, false);
    private void StopPlayAccelerate() => StopClip(_accelerateSound);
    private void PlayAccelerate() => PlayClip(_accelerateSound, true);

    private void PlayClip(AudioClip clipToPlay, bool isLooped)
    {
        for (int i = 0; i < _audioSources.Count; i++)
        {
            AudioSource source = _audioSources[i];
            if (!source.isPlaying)
            {
                source.clip = clipToPlay;
                source.loop = isLooped;
                source.Play();
                
                return;
            }
        }
        Debug.LogWarning($"All audiosources are being used in {GetType().Name}. Increase max audiosources.");
    }

    private void StopClip(AudioClip clipToStop)
    {
        foreach (AudioSource audioSource in _audioSources)
        {
            if (audioSource.clip == clipToStop) audioSource.Stop();
        }
    }

    private void OnDestroy()
    {
        PlayerMovement.Instance.OnMoveStart -= PlayAccelerate;
        PlayerMovement.Instance.OnMoveStop -= StopPlayAccelerate;
        PlayerBody.Instance.OnHit -= PlayLargeExpolsion;
        BulletSpawner.Instance.OnBulletSpawn -= PlayShoot;
        AsteroidsDestroyer.Instance.OnAsteroidDestroy -= PlayBlowSound;
        UFOSpawner.Instance.OnUFOSpawn -= PlayUFOSound;
        UFOSpawner.Instance.OnUFODeath -= TryStopPlayUFOSound;
    }
}
