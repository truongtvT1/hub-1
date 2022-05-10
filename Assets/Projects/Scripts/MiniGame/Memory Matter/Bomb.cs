using System;
using System.Collections;
using UnityEngine;

namespace MiniGame.MemoryMatter
{
    public class Bomb : MonoBehaviour
    {
        public ParticleSystem blowFx, waitFx;
        public float duration, blowRadius, force;
        

        private void OnEnable()
        {
            StartCoroutine(Boom());
        }

        IEnumerator Boom()
        {
            transform.GetChild(0).GetComponent<Renderer>().enabled = true;
            var renderer = transform.GetChild(0).GetChild(0).GetComponent<Renderer>();
            if (renderer)
            {
                renderer.enabled = true;
            }
            blowFx.gameObject.SetActive(false);
            waitFx.gameObject.SetActive(true);
            yield return new WaitForSeconds(duration);
            waitFx.gameObject.SetActive(false);
            blowFx.gameObject.SetActive(true);
            transform.GetChild(0).GetComponent<Renderer>().enabled = false;
            if (renderer)
            {
                renderer.enabled = false;
            }
            CircleCast();
            yield return new WaitUntil(() => !blowFx.isPlaying);
            gameObject.SetActive(false);
        }

        void CircleCast()
        {
            var hit = Physics2D.CircleCastAll(transform.GetChild(0).position, blowRadius,transform.right,0);
            if (hit.Length != 0)
            {
                for (int i = 0; i < hit.Length; i++)
                {
                    if (hit[i].collider.CompareTag("Player"))
                    {
                        var player = hit[i].transform.GetComponent<PlayerMovement>();
                        player.SetForce((player.transform.position - transform.GetChild(0).position.normalized) * force, true);
                    }
                    else
                    {
                        var rigidBody = hit[i].collider.attachedRigidbody;
                        if (rigidBody != null)
                        {
                            Debug.Log("bloom " + hit[i].collider.name);
                            rigidBody.AddForce((hit[i].transform.position - transform.GetChild(0).position).normalized * force, ForceMode2D.Impulse);
                        }
                    }
                }
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.GetChild(0).position ,blowRadius);
        }
    }
}