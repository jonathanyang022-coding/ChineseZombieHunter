using UnityEngine;

namespace ChineseZombieHunter
{
    /// <summary>
    /// Small input bridge for a lane-runner shooter.
    /// In the editor it falls back to keyboard controls for quick testing.
    /// </summary>
    public class PlayerInputSource : MonoBehaviour
    {
        [SerializeField] private bool useLegacyEditorInput = true;

        public Vector2 Move { get; private set; }
        public bool FireHeld { get; private set; }

        public void SetMove(Vector2 value)
        {
            Move = Vector2.ClampMagnitude(value, 1f);
        }

        public void SetFireHeld(bool held)
        {
            FireHeld = held;
        }

        private void Update()
        {
            if (!useLegacyEditorInput || Application.isMobilePlatform)
            {
                return;
            }

            Move = new Vector2(Input.GetAxisRaw("Horizontal"), 0f);
            FireHeld = Input.GetButton("Fire1");
        }
    }
}
