using System;
using UnityEngine;
using System.Collections;
using ThirdParties.Truongtv.SoundManager;

public class ParticleGold : MonoBehaviour {
    public AudioClip soundCoinExplosion;
    public AudioClip soundCoin;
    public Transform transformTarget;
    public float minSpeed;
    public float maxSpeed;
    [SerializeField] private ParticleSystem system;
    private float waitTime = 0.35f; //time chờ để tween các đồng xu vào target

    private static ParticleSystem.Particle[] particles = new ParticleSystem.Particle[1000];
    private int count;
    private Vector2 target;

    private bool playingSound = false;
    
    public void Play(float delay)
    {
        StartCoroutine(IEPlay(delay));
    }

    public void Play(int value)
    {
        var main = system.main;
        main.maxParticles = value;
        Play();
        
    }
    private IEnumerator IEPlay(float delay)
    {
        yield return new WaitForSeconds(delay);
        Play();
    }

	void Update(){
        waitTime -= Time.deltaTime;

        if (waitTime <= 0)
        {
            target = transformTarget.position;

            system.Stop();
            system.gravityModifier = 0f;
            system.startSpeed = 0f;
            count = system.GetParticles(particles);

            for (int i = 0; i < count; i++)
            {
                ParticleSystem.Particle particle = particles[i];

                Vector3 v1 = particle.position;
                Vector3 v2 = new Vector3(target.x, target.y, v1.z);

                particle.position = Vector3.MoveTowards(v1, v2, 13f * Time.deltaTime);
                particles[i] = particle;
            }

            system.SetParticles(particles, count);

            if (waitTime <= -0.1f && !playingSound)
            {
                StartCoroutine(IEPlaySound());
                playingSound = true;
            }
        }
	}

    public void Play()
    {
        system.Stop();
        waitTime = 0.35f;
        playingSound = false;
        
        system.startSpeed = UnityEngine.Random.Range(minSpeed, maxSpeed);
        system.Play();
        if (SoundManager.Instance && soundCoinExplosion != null) SoundManager.Instance.PlaySfx(soundCoinExplosion);
    }

    private IEnumerator IEPlaySound()
    {
        for (int i = 0; i < 4; i++)
        {
            yield return new WaitForSeconds(UnityEngine.Random.Range(0f, 0.3f));
            if (SoundManager.Instance && soundCoin != null) SoundManager.Instance.PlaySfx(soundCoin);
        }
    }
}