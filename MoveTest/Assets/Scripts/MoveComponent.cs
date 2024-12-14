using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Test
{
    public enum EasingType
    {
        Linear,
        EaseIn,
        EaseOut,
        EaseInOut
    }
    
    public class MoveObject : MonoBehaviour
    {
        [SerializeField] private Vector3 startPosition = Vector3.zero;
        [SerializeField] private Vector3 targetPosition = Vector3.one;
        [SerializeField] private float duration = 2f;
        [SerializeField] private bool pingpong = true;
        [SerializeField] private EasingType easingType = EasingType.Linear;
        [SerializeField] private Button moveButton;
        [SerializeField] private Button resetButton;
        
        private void Start()
        {
            moveButton.onClick.AddListener(() =>
            {
                gameObject.transform.position = startPosition;
                Move(gameObject, startPosition, targetPosition, duration, pingpong, easingType);
            });
            
            resetButton.onClick.AddListener(() =>
            {
                StopAllCoroutines();
                gameObject.transform.position = startPosition;
            });
        }
        
        public void Move(GameObject obj, Vector3 begin, Vector3 end, float time, bool pingpong, EasingType easingType)
        {
            StartCoroutine(MoveCoroutine(obj, begin, end, time, pingpong, easingType));
        }

        private IEnumerator MoveCoroutine(GameObject obj, Vector3 begin, Vector3 end, float time, bool pingpong, EasingType easingType)
        {
            do
            {
                yield return AnimateMovement(obj, begin, end, time, easingType);

                if (pingpong)
                {
                    yield return AnimateMovement(obj, end, begin, time, easingType);
                }
            } while (pingpong);
        }

        private IEnumerator AnimateMovement(GameObject obj, Vector3 start, Vector3 target, float duration, EasingType easingType)
        {
            float elapsedTime = 0f;

            while (elapsedTime < duration)
            {
                elapsedTime += Time.deltaTime;
                float t = Mathf.Clamp01(elapsedTime / duration);

                t = ApplyEasing(t, easingType);

                obj.transform.position = Vector3.Lerp(start, target, t);

                yield return null;
            }

            obj.transform.position = target;
        }

        private float ApplyEasing(float t, EasingType easingType)
        {
            switch (easingType)
            {
                case EasingType.EaseIn:
                    return t * t;

                case EasingType.EaseOut:
                    return t * (2 - t);

                case EasingType.EaseInOut:
                    return t < 0.5f ? 2 * t * t : -1 + (4 - 2 * t) * t;

                case EasingType.Linear:
                default:
                    return t;
            }
        }
    }
}