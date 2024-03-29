using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace AAA.Utility.GameObjectUtil
{
    public class GameObjectRotator : MonoBehaviour
    {
        [Header("Values")]
        [SerializeField] private Vector3 axis;
        [Tooltip("Multiplies the speed")]
        [SerializeField] private float speedMultiplier = 1f;

        [SerializeField] private bool changeContinuous = false;

        private void Start()
        {
            if (changeContinuous)
                StartCoroutine(ChangeContinuous(axis));
        }

        public void RotateAmount(float amount)
        {
            StopAllCoroutines();
            var targetRotation = transform.eulerAngles + (axis * amount);
            StartCoroutine(SetRotation(Quaternion.Euler(targetRotation), speedMultiplier));
        }

        public void StartChangeContinuous()
        {
            StopAllCoroutines();
            StartCoroutine(ChangeContinuous(axis));
        }

        public void StartChangeContinuous(Vector3 axis)
        {
            StopAllCoroutines();
            StartCoroutine(ChangeContinuous(axis));
        }

        public void SetRotationSmooth()
        {
            StopAllCoroutines();
            StartCoroutine(SetRotation(Quaternion.Euler(axis), speedMultiplier));
        }
        public void SetRotationSmooth(float axisMultiplier)
        {
            StopAllCoroutines();
            StartCoroutine(SetRotation(Quaternion.Euler(axis * axisMultiplier), speedMultiplier));
        }
        public void SetRotationSmooth(Vector3 targetRotation)
        {
            StopAllCoroutines();
            StartCoroutine(SetRotation(Quaternion.Euler(targetRotation), speedMultiplier));
        }

        public void StopRotation()
        {
            StopAllCoroutines();
        }

        private IEnumerator ChangeContinuous(Vector3 axis)
        {
            while (changeContinuous)
            {
                transform.Rotate(axis * Time.deltaTime * speedMultiplier);
                yield return null;
            }
        }

        private IEnumerator SetRotation(Quaternion targetRotation, float speedMultiplier)
        {
            var startTime = Time.time;

            while (true)
            {
                var progress = (Time.time - startTime) / speedMultiplier;

                if (progress > 1)
                {
                    transform.rotation = targetRotation;
                    progress = 1;
                    break;
                }
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, progress);
                yield return null;
            }
        }
    }
}