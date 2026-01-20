using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CruFramework.UI;
using Random = UnityEngine.Random;

namespace Pjfb.InGame.ClubRoyal
{
    public class ClubRoyalInGameAbsorbBallUI : MonoBehaviour
    {
        [SerializeField] private AnimationCurve moveCurve;
        [SerializeField] private AnimationCurve scaleCurve;
        
        private Vector3 startPosition;
        private Vector3 endPosition;
        private Vector3 bezierPosition;
        private Action onEndCallback;

        private float elapsedSec;
        private const float Duration = 1.0f;
        private const int RandomRange = 10;

        private void Update()
        {
            elapsedSec += Time.deltaTime;
            if (elapsedSec >= Duration)
            {
                transform.position = endPosition;
                onEndCallback?.Invoke();
                gameObject.SetActive(false);
                return;
            }

            var ratio = moveCurve.Evaluate(elapsedSec / Duration);
            var scale = scaleCurve.Evaluate(elapsedSec / Duration);
            var pointA = Vector3.Lerp(startPosition, bezierPosition, ratio);
            var pointB = Vector3.Lerp(bezierPosition, endPosition, ratio);
            var position = Vector3.Lerp(pointA, pointB, ratio);

            transform.position = position;
            transform.localScale = new Vector3(scale, scale, scale);
        }

        public void PlayMoveAnimation(Vector3 _startPosition, Vector3 _endPosition, Action _onEndCallback)
        {
            gameObject.SetActive(true);
            elapsedSec = 0.0f;
            startPosition = _startPosition;
            endPosition = _endPosition;
            onEndCallback = _onEndCallback;

            transform.position = startPosition;

            var distance = Vector3.Distance(startPosition, endPosition);
            var x = Random.Range(-RandomRange, RandomRange) * (distance / 10);
            var y = Random.Range(-RandomRange, RandomRange) * (distance / 10);
            Debug.Log($"Distance: {distance}");
            bezierPosition = ((startPosition + endPosition) / 2) + new Vector3(x, y, 0);
            
        }
    }
}