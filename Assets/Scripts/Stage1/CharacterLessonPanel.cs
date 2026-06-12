using System;
using UnityEngine;
using UnityEngine.UI;

namespace ChineseZombieHunter
{
    public class CharacterLessonPanel : MonoBehaviour
    {
        [SerializeField] private Text characterText;
        [SerializeField] private Text meaningText;
        [SerializeField] private Text promptText;
        [SerializeField] private Button continueButton;
        [SerializeField] private Text continueButtonText;

        public void Show(Stage1CharacterEntry entry, bool isFinalLesson, Action onContinue)
        {
            if (characterText != null)
            {
                characterText.text = entry.Character;
            }

            if (meaningText != null)
            {
                meaningText.text = $"{entry.Character} = {entry.Meaning}";
            }

            if (promptText != null)
            {
                promptText.text = isFinalLesson ? "Tap to start the challenge." : "Tap to see the next one.";
            }

            if (continueButtonText != null)
            {
                continueButtonText.text = isFinalLesson ? "Start" : "Next";
            }

            if (continueButton != null)
            {
                continueButton.onClick.RemoveAllListeners();
                continueButton.onClick.AddListener(() => onContinue?.Invoke());
            }
        }
    }
}
