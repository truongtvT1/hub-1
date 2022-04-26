using System;
using Projects.Scripts.Hub;
using UnityEngine;

namespace MiniGame.StickRun
{
    public class StickmanPlayerController : MonoBehaviour
    {
        public float normalSpeed, sprintSpeed;
        public CharacterAnimation anim;
        public CapsuleCollider2D collider;
        public MoveDirection moveDirection;
        public Vector2 sprintCollider;

        private void OnCollisionEnter2D(Collision2D other)
        {
            Debug.Log(other.gameObject.name);
        }

        private void Update()
        {
            transform.position = Vector3.MoveTowards(transform.position,transform.position + new Vector3((int) moveDirection * 10,0),normalSpeed * Time.deltaTime);
        }
    }
}