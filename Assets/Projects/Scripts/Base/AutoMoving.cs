using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Sirenix.OdinInspector;
using ThirdParties.Truongtv.SoundManager;
using UnityEngine;
using UnityEngine.Events;

namespace Base
{
    public class AutoMoving : BaseMoving
    {
        [SerializeField] private bool autoMove = true, onBallEnterMove = false;
        [SerializeField] private bool openEnded, ignoreTimeScale = true;
        [SerializeField] private UnityEvent onMoveNextComplete;
        [SerializeField] private UnityEvent onStartMoving;
        [SerializeField] private UnityEvent onMovingEvent;
        [SerializeField,ShowIf(nameof(openEnded))] private UnityEvent onMoveBackComplete;
        [SerializeField] private SimpleAudio simpleAudio;
        [SerializeField] private AudioClip movingSound, stopSound;
        [SerializeField,HideIf("autoMove")] private SpriteRenderer detectCamera;

        public List<Vector3> PointList
        {
            get
            {
                return pointList;
            }
        }
        
        private bool _increase,_started;
        protected virtual void Start()
        {
            pointIndex = 0;
            _increase = true;

            _started = false;
            if (autoMove && enabled)
            {
                Auto();
            }
            else if (!onBallEnterMove)
            {
                StartCoroutine(CheckVisible());
            }
        }

        IEnumerator CheckVisible()
        {
            yield return new WaitUntil(() => detectCamera != null && detectCamera.isVisible);
            Auto();
        }

        [Button]
        public void Active()
        {
            if(!_started)
                Auto();
        }
        [Button]
        public void DeActive()
        {
            currentTween.Kill();
            _started = false;
           
            if (openEnded) // move to end then move back
            {
                if (_increase)
                {
                    
                    MoveBack(pointIndex, onMove: () => onMovingEvent?.Invoke(), onStart: () =>
                        {
                            onStartMoving?.Invoke();
                            // if(movingSound!=null)
                            //     simpleAudio.Play(movingSound,true);
                        },complete:
                        () =>
                        {
                            if (pointIndex > 0)
                            {
                                pointIndex -= 1;
                                DeActive();
                            }
                            else
                            {
                                _increase = false;
                                if (stopSound!= null)
                                {
                                    simpleAudio.Play(stopSound);
                                }
                            }
                        },ignoreTimeScale:ignoreTimeScale);
                }
                else
                {
                    MoveBack(pointIndex-1, onMove: () => onMovingEvent?.Invoke(), onStart: () =>
                        {
                            onStartMoving?.Invoke();
                            // if(movingSound!=null)
                            //     simpleAudio.Play(movingSound,true);
                        },complete:
                        () =>
                        {
                            if (pointIndex > 0)
                            {
                                DeActive();
                            }
                            else
                            {
                                if (stopSound!= null)
                                {
                                    // simpleAudio.Play(stopSound);
                                }
                            }
                        },ignoreTimeScale:ignoreTimeScale);
                }
            }
            else // move to end then move to first
            {
                MoveBack(pointIndex, onMove: () => onMovingEvent?.Invoke(), onStart: () =>
                    {
                        onStartMoving?.Invoke();
                        // if(movingSound!=null)
                        //     simpleAudio.Play(movingSound,true);
                    },complete:
                    () =>
                    {
                        if (pointIndex > 0)
                        {
                            pointIndex -= 1;
                            DeActive();
                        }
                    },ignoreTimeScale:ignoreTimeScale);
            }
        }
        
        private void Auto()
        {
            _started = true;
           
            if (openEnded) // move to end then move back
            {
                if (_increase)
                {
                    if (pointIndex + 1 <= pointList.Count - 1 )
                    {
                        MoveNext(pointIndex+1, ()=>
                        {
                            if (stopSound!= null)
                            {
                                // simpleAudio.Play(stopSound);
                            }
                            Auto();
                        }, onMove: () => onMovingEvent?.Invoke(), onStart: () =>
                        {
                            onStartMoving?.Invoke();
                            // if(movingSound!=null)
                            //     simpleAudio.Play(movingSound,true);
                        },ignoreTimeScale:ignoreTimeScale);
                    }
                    else
                    {
                        _increase = false;
                        Auto();
                        onMoveNextComplete.Invoke();
                    }
                }
                else
                {
                    if (pointIndex - 1 >=0 )
                    {
                        MoveBack(pointIndex-1, ()=>
                        {
                           
                            Auto();
                        }, onMove: () => onMovingEvent?.Invoke(), onStart: () =>
                        {
                            onStartMoving?.Invoke();
                            // if(movingSound!=null)
                            //     simpleAudio.Play(movingSound,true);
                        },ignoreTimeScale:ignoreTimeScale);
                    }
                    else
                    {
                        _increase = true;
                        Auto();
                        onMoveBackComplete.Invoke();
                    }
                }
            }
            else // move to end then move to first
            {
                MoveNext((pointIndex+1)%pointList.Count, ()=>
                {
                    if (stopSound!= null)
                    {
                        // simpleAudio.Play(stopSound);
                    }
                    onMoveNextComplete.Invoke();
                    Auto();
                }, onMove: () => onMovingEvent?.Invoke(), onStart: () =>
                {
                    onStartMoving?.Invoke();
                    // if(movingSound!=null)
                    //     simpleAudio.Play(movingSound,true);
                },ignoreTimeScale:ignoreTimeScale);
            }
        }

        public void Stop()
        {
            _started = false;
            currentTween.Kill();
        }

        protected override Vector3[] GetListPoint()
        {
            if (!openEnded && pointList.Count>2)
            {
                var result = new List<Vector3>(pointList);
                result.Add(pointList[0]);
                return result.ToArray();
            }
            return base.GetListPoint();
        }

        private void OnDrawGizmosSelected()
        {
            if (!Application.isPlaying)
            {
                if (onBallEnterMove)
                {
                    autoMove = false;
                }
            }
        }
    }
}