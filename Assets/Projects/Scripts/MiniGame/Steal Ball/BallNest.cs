﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

namespace MiniGame.Steal_Ball
{
    public class BallNest : MonoBehaviour
    {
        public float subOffsetX = .5f;
        public float subOffsetY = .5f;
        public float ballCheckInterval = .5f;
        private PlayerStealBallController controller;
        private List<Ball> listBall = new List<Ball>();
        
        public void Init(PlayerStealBallController controller)
        {
            this.controller = controller;
            listBall = new List<Ball>();
            StartCoroutine(CheckBallStolen());
        }

        IEnumerator CheckBallStolen()
        {
            while (GamePlayController.Instance.state != GameState.End)
            {
                var temp = new List<Ball>();
                for (int i = 0; i < listBall.Count; i++)
                {
                    if (listBall[i].state == Ball.State.Unavailable)
                    {
                        OnBallStolen();
                        temp.Add(listBall[i]);
                    }
                }
                for (int i = 0; i < temp.Count; i++)
                {
                    listBall.Remove(temp[i]);
                }
                yield return new WaitForSeconds(ballCheckInterval);
            }
        }

        public bool IsBallInNest(Ball ball)
        {
            return listBall.Contains(ball);
        }
        
        public void OnBallStolen()
        {
            controller.OnBallStolen();
        }
        
        public void OnBallRelease(Ball ball)
        {
            //TODO: fx glow
            ball.transform.SetParent(transform.parent);
            var offset = new Vector2(subOffsetX, subOffsetY);
            ball.transform.position = (Vector2) transform.position - offset + Random.insideUnitCircle;
            listBall.Add(ball);
        }
    }
}