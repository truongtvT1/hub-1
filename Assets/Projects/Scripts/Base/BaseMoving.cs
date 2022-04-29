using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEditor.Experimental;
using UnityEngine;

namespace Base
{
    public class BaseMoving : MonoBehaviour
    {
        [SerializeField] protected Transform target,pointParent;
        [SerializeField] protected Ease curve = Ease.Linear,reserveCurve = Ease.Linear;
        [SerializeField] protected float speed,reserveSpeed;
        [SerializeField] protected float stepStartDelay;
        [SerializeField] protected LineRenderer line;
        [SerializeField] protected List<Vector3> pointList;
        [SerializeField] protected bool startAtFirstPoint;

        protected Tween currentTween;
        protected int pointIndex = 0;

        private void Awake()
        {
            pointList = new List<Vector3>();
            if(pointParent==null) return;
            foreach (Transform point in pointParent)
            {
                if(point.gameObject.activeSelf)
                    pointList.Add(point.position);
            }
            if(pointList.Count>0&& target!=null && startAtFirstPoint)
                target.position = pointList[0];
            if (pointList.Count >= 2 && line)
            {
                var points = GetListPoint();
                line.positionCount = points.Length;
                line.SetPositions(points);
            }
        }

        protected void MoveNext(int endIndex, Action complete = null,
            Action onMove = null, Action onStart = null,bool ignoreTimeScale = false)
        {
            target.DOKill();
            currentTween = target.transform
                .DOMove(pointList[endIndex], speed)
                .SetDelay(stepStartDelay)
                .SetEase(curve)
                .SetSpeedBased(true)
                .OnStart(() =>
                {
                    onStart?.Invoke();
                })
                .OnUpdate(() =>
                {
                    OnTweenUpdate();
                    onMove?.Invoke();
                })
                .OnComplete(() => {
                    pointIndex = endIndex;
                    complete?.Invoke(); 
                });
            if (ignoreTimeScale)
                currentTween.SetUpdate(UpdateType.Normal, ignoreTimeScale);
            currentTween.Play();
        }
        protected void MoveBack(int endIndex,Action complete = null,Action onMove = null,Action onStart = null,bool ignoreTimeScale = false)
        {
            target.DOKill();
            currentTween = target.transform
                .DOMove(pointList[endIndex], reserveSpeed)
                .SetDelay(stepStartDelay)
                .SetEase(reserveCurve)
                .SetSpeedBased(true)
                .OnStart(() =>
                {
                    onStart?.Invoke();
                })
                .OnUpdate(() => 
                {
                    OnTweenUpdate(false);
                    onMove?.Invoke();
                })
                .OnComplete(() => {
                    pointIndex = endIndex;
                    complete?.Invoke(); 
                });
            if (ignoreTimeScale)
                currentTween.SetUpdate(UpdateType.Normal, ignoreTimeScale);
            currentTween.Play();
        }

        protected virtual Vector3[] GetListPoint()
        {
            return pointList.ToArray();
        }

        protected virtual void OnTweenUpdate(bool onMoveNext = true)
        {
            
        }
#if UNITY_EDITOR
        protected virtual void OnDrawGizmos()
        {
            if (!Application.isPlaying)
            {
                pointList = new List<Vector3>();
                if(pointParent==null) return;
                foreach (Transform point in pointParent)
                {
                    if(point.gameObject.activeSelf)
                        pointList.Add(point.position);
                }
                if(pointList.Count>0&& target!=null && startAtFirstPoint)
                    target.position = pointList[0];
                // Gizmos.color = Color.green;
                // foreach (var pos in pointList)
                // {
                //     Gizmos.DrawSphere(pos,0.3f);
                //
                // }
                if (pointList.Count >= 2 && line)
                {
                    var points = GetListPoint();
                    line.positionCount = points.Length;
                    line.SetPositions(points);
                }
            }
        }
#endif
    }
}