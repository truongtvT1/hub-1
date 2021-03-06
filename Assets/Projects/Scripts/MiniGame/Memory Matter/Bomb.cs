using System;
using System.Collections;
using ThirdParties.Truongtv.SoundManager;
using UnityEngine;

namespace MiniGame.MemoryMatter
{
    public class Bomb : MonoBehaviour
    {
        public ParticleSystem blowFx, waitFx;
        public float duration, blowRadius, force;
        public Rigidbody2D rigidBody;
        public AudioClip blowSound;
        private void OnEnable()
        {
            StartCoroutine(Boom());
        }
        IEnumerator Boom()
        {
            rigidBody.gameObject.SetActive(true);
            blowFx.gameObject.SetActive(false);
            waitFx.gameObject.SetActive(true);
            yield return new WaitForSeconds(duration);
            SoundManager.Instance.PlaySfx(blowSound);
            waitFx.gameObject.SetActive(false);
            blowFx.gameObject.SetActive(true);
            rigidBody.gameObject.SetActive(false);
            CircleCast();
            yield return new WaitUntil(() => !blowFx.isPlaying);
            gameObject.SetActive(false);
        }

        void CircleCast()
        {
            var hit = Physics2D.CircleCastAll(rigidBody.transform.position, blowRadius,rigidBody.transform.right,0);
            if (hit.Length != 0)
            {
                for (int i = 0; i < hit.Length; i++)
                {
                    if (hit[i].collider.CompareTag("Player"))
                    {
                        var player = hit[i].transform.GetComponent<PlayerMovement>();
                        Vector2 vForce = player.transform.position - rigidBody.transform.position;
                        player.SetForce(vForce * force / Mathf.Sqrt(vForce.sqrMagnitude), true);
                    }
                    else
                    {
                        var rigidBody = hit[i].collider.attachedRigidbody;
                        if (rigidBody != null)
                        {
                            rigidBody.AddForce((hit[i].transform.position - this.rigidBody.transform.position) * force, ForceMode2D.Impulse);
                        }
                    }
                }
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(rigidBody.transform.position ,blowRadius);
        }
    }
}