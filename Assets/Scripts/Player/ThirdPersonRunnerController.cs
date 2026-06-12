using UnityEngine;

namespace ChineseZombieHunter
{
    [RequireComponent(typeof(CharacterController))]
    public class ThirdPersonRunnerController : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Transform visualRoot;
        [SerializeField] private PlayerInputSource inputSource;
        [SerializeField] private WeaponController weaponController;

        [Header("Movement")]
        [SerializeField] private float forwardSpeed = 7f;
        [SerializeField] private float laneOffset = 2.5f;
        [SerializeField] private float laneChangeSpeed = 12f;
        [SerializeField] private int laneCount = 3;
        [SerializeField] private bool autoAdvance = true;

        [Header("Combat")]
        [SerializeField] private bool autoFire = true;

        private CharacterController characterController;
        private int currentLaneIndex = 1;
        private int targetLaneIndex = 1;
        private float laneInputLatch;
        private Vector3 velocity;

        public int CurrentLaneIndex => currentLaneIndex;

        private void Awake()
        {
            characterController = GetComponent<CharacterController>();
            if (inputSource == null)
            {
                inputSource = GetComponent<PlayerInputSource>();
            }
        }

        private void Update()
        {
            HandleLaneInput();
            MoveRunner();
            HandleCombat();
        }

        public void ShiftLane(int direction)
        {
            if (direction == 0)
            {
                return;
            }

            targetLaneIndex = Mathf.Clamp(targetLaneIndex + direction, 0, Mathf.Max(0, laneCount - 1));
        }

        private void HandleLaneInput()
        {
            float laneInput = ReadLaneInput();

            if (laneInput > 0.2f && laneInputLatch <= 0.2f)
            {
                ShiftLane(1);
            }
            else if (laneInput < -0.2f && laneInputLatch >= -0.2f)
            {
                ShiftLane(-1);
            }

            laneInputLatch = laneInput;
        }

        private float ReadLaneInput()
        {
            if (inputSource != null)
            {
                return inputSource.Move.x;
            }

            return Input.GetAxisRaw("Horizontal");
        }

        private void MoveRunner()
        {
            float targetX = GetLaneX(targetLaneIndex);
            Vector3 position = transform.position;
            float newX = Mathf.Lerp(position.x, targetX, laneChangeSpeed * Time.deltaTime);
            float xMovement = newX - position.x;

            if (characterController.isGrounded && velocity.y < 0f)
            {
                velocity.y = -2f;
            }

            velocity.y += Physics.gravity.y * Time.deltaTime;

            float zMovement = autoAdvance ? forwardSpeed * Time.deltaTime : 0f;
            float yMovement = velocity.y * Time.deltaTime;

            characterController.Move(new Vector3(xMovement, yMovement, zMovement));
            currentLaneIndex = targetLaneIndex;

            if (visualRoot != null)
            {
                visualRoot.localRotation = Quaternion.Euler(0f, 0f, 0f);
            }
        }

        private void HandleCombat()
        {
            bool fireHeld = autoFire;

            if (inputSource != null && inputSource.FireHeld)
            {
                fireHeld = true;
            }

            if (fireHeld && weaponController != null)
            {
                weaponController.TryFire();
            }
        }

        private float GetLaneX(int laneIndex)
        {
            int clampedIndex = Mathf.Clamp(laneIndex, 0, Mathf.Max(0, laneCount - 1));
            float centeredIndex = clampedIndex - ((laneCount - 1) * 0.5f);
            return centeredIndex * laneOffset;
        }
    }
}
