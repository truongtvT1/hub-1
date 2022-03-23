using UnityEngine;
using System.Collections;
using ThirdParties.Truongtv.SoundManager;

namespace RavingBots.Water2D
{
	[RequireComponent(typeof(BuoyancyEffector2D))]
	public class Water2DEffects : MonoBehaviour
	{
		public Water2DSplashFX SplashFXPrefab;
		public int SplashFXPrecache = 30;
		public float SplashFXPowerScale = 0.1f;
		public float SplashFXPowerThreshold = 0.1f;
		public float SplashFXOffset = 0.2f;
		public AudioClip[] SplashFXSounds;
		public float SplashFXPowerToVolume = 1;
		public float SplashFXPowerToPitch = 1;

		public float FloatingSpeed = 1f;
		public float FloatingRange = 1f;

		BuoyancyEffector2D _buoyancyEffector2D;
		float _surfaceLevel;

		Water2DSplashFX[] _splashCache;
		int _splash;

		void Awake()
		{
			_buoyancyEffector2D = GetComponent<BuoyancyEffector2D>();
			_surfaceLevel = _buoyancyEffector2D.surfaceLevel;

			_splashCache = new Water2DSplashFX[SplashFXPrecache];
			var container = new GameObject("Splash Container").transform;
			container.transform.position = Vector3.zero;
			for (var i = 0; i < _splashCache.Length; i++)
			{
				var splash = Instantiate(SplashFXPrefab);
				splash.transform.parent = container;
				_splashCache[i] = splash;
            }
        }

		void FixedUpdate()
		{
			_buoyancyEffector2D.surfaceLevel = _surfaceLevel - FloatingRange * 0.5f * (Mathf.Sin(Mathf.PI * 2f * FloatingSpeed * Time.fixedTime) + 1f);
		}

		public void OnTriggerEnter2D(Collider2D other)
		{
			// Debug.Log(other.name);
			// var rigidbody = other.transform.parent.GetComponent<Rigidbody2D>();
			// var power = SplashFXPowerScale * Vector2.Dot(rigidbody.velocity, Vector2.down) * rigidbody.mass;
			// Debug.Log("herer");
			// if (power < SplashFXPowerThreshold)
			// 	return;
			// Debug.Log("2222");
			if (other.gameObject.name.Contains("Box"))
			{
				Debug.Log("collide box");
				other.GetComponent<Rigidbody2D>().mass = 3;
			}
			var splash = _splashCache[_splash];
			splash.transform.position = new Vector3(other.bounds.center.x, other.bounds.min.y - SplashFXOffset,100);
			splash.Play(2.5f, SplashFXSounds[Random.Range(0, SplashFXSounds.Length)], 5 * SplashFXPowerToVolume, SplashFXPowerToPitch / 5);

			_splash = (_splash + 1) % _splashCache.Length;
        }
	}
}