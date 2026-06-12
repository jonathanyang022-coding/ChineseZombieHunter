using UnityEngine;

namespace ChineseZombieHunter
{
    public class ThirdPersonCameraFollow : MonoBehaviour
    {
        [SerializeField] private Transform target;
        [SerializeField] private Vector3 offset = new Vector3(0f, 4f, -7f);
        [SerializeField] private float followSpeed = 8f;
        [SerializeField] private float lookAhead = 6f;
        [SerializeField] private float lookHeight = 1.5f;

        private void LateUpdate()
        {
            if (target == null)
            {
                return;
            }

            Vector3 targetPosition = target.position + target.TransformDirection(offset);
            transform.position = Vector3.Lerp(transform.position, targetPosition, followSpeed * Time.deltaTime);

            Vector3 lookTarget = target.position + Vector3.up * lookHeight + target.forward * lookAhead;
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(lookTarget - transform.position), followSpeed * Time.deltaTime);
        }
    }
}
