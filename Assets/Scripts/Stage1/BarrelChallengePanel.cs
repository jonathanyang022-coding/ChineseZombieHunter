using System;
using UnityEngine;
using UnityEngine.UI;

namespace ChineseZombieHunter
{
    public class BarrelChallengePanel : MonoBehaviour
    {
        [SerializeField] private Text promptText;
        [SerializeField] private Text barrelLabelText;
        [SerializeField] private Text feedbackText;
        [SerializeField] private Button[] answerButtons = new Button[3];
        [SerializeField] private Text[] answerButtonTexts = new Text[3];

        public void ConfigureChoices(Stage1CharacterEntry[] entries, Action<string> onChoice)
        {
            if (entries == null || entries.Length != 3)
            {
                return;
            }

            for (int i = 0; i < 3; i++)
            {
                Stage1CharacterEntry entry = entries[i];

                if (answerButtonTexts != null && i < answerButtonTexts.Length && answerButtonTexts[i] != null)
                {
                    answerButtonTexts[i].text = entry.Character;
                }

                if (answerButtons != null && i < answerButtons.Length && answerButtons[i] != null)
                {
                    string chosenCharacter = entry.Character;
                    answerButtons[i].onClick.RemoveAllListeners();
                    answerButtons[i].onClick.AddListener(() => onChoice?.Invoke(chosenCharacter));
                }
            }
        }

        public void SetBarrelLabel(string label)
        {
            if (barrelLabelText != null)
            {
                barrelLabelText.text = label;
            }
        }

        public void SetPrompt(string text)
        {
            if (promptText != null)
            {
                promptText.text = text;
            }
        }

        public void ShowFeedback(string message)
        {
            if (feedbackText != null)
            {
                feedbackText.text = message;
            }
        }

        public void SetChoiceButtonsInteractable(bool interactable)
        {
            if (answerButtons == null)
            {
                return;
            }

            for (int i = 0; i < answerButtons.Length; i++)
            {
                if (answerButtons[i] != null)
                {
                    answerButtons[i].interactable = interactable;
                }
            }
        }
    }
}
