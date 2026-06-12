using System;
using UnityEngine;
using UnityEngine.UI;

namespace ChineseZombieHunter
{
    public class HandwritingPracticePanel : MonoBehaviour
    {
        [SerializeField] private Text titleText;
        [SerializeField] private Text instructionText;
        [SerializeField] private Text characterText;
        [SerializeField] private Button clearButton;
        [SerializeField] private Text clearButtonText;
        [SerializeField] private HandwritingPad handwritingPad;

        public void Show(Stage1CharacterEntry entry)
        {
            if (titleText != null)
            {
                titleText.text = "Trace It";
            }

            if (instructionText != null)
            {
                instructionText.text = "Use your finger or mouse to write the character in the box.";
            }

            if (characterText != null)
            {
                characterText.text = entry.Character;
            }

            if (clearButtonText != null)
            {
                clearButtonText.text = "Clear";
            }

            if (clearButton != null)
            {
                clearButton.onClick.RemoveAllListeners();
                clearButton.onClick.AddListener(() => handwritingPad?.ClearCanvas());
            }
        }

        public void ClearPad()
        {
            handwritingPad?.ClearCanvas();
        }
    }
}
