using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ChineseZombieHunter
{
    public class Stage1Manager : MonoBehaviour
    {
        [Header("Lesson Data")]
        [SerializeField] private List<Stage1CharacterEntry> lessonEntries = new List<Stage1CharacterEntry>
        {
            new Stage1CharacterEntry("一", "1"),
            new Stage1CharacterEntry("二", "2"),
            new Stage1CharacterEntry("三", "3"),
        };

        [Header("UI Panels")]
        [SerializeField] private CanvasGroup lessonPanelGroup;
        [SerializeField] private CanvasGroup challengePanelGroup;
        [SerializeField] private CanvasGroup resultPanelGroup;

        [Header("UI Views")]
        [SerializeField] private CharacterLessonPanel lessonPanel;
        [SerializeField] private BarrelChallengePanel challengePanel;
        [SerializeField] private HandwritingPracticePanel handwritingPanel;
        [SerializeField] private LifeDisplay lifeDisplay;
        [SerializeField] private StageResultPanel resultPanel;
        [SerializeField] private Stage1Barrel barrelView;

        [Header("Timing")]
        [SerializeField] private float nextBarrelDelay = 0.45f;

        private Stage1StageState stageState;
        private readonly List<Stage1CharacterEntry> challengeOrder = new List<Stage1CharacterEntry>();
        private int challengeIndex;
        private Coroutine advanceRoutine;

        public string CurrentLessonCharacter => stageState != null ? stageState.CurrentLesson.Character : string.Empty;
        public string CurrentLessonMeaning => stageState != null ? stageState.CurrentLesson.Meaning : string.Empty;
        public int CurrentLives => stageState != null ? stageState.Lives : 0;

        public static Stage1Manager CreateForTests()
        {
            var manager = new GameObject("Stage1Manager_TestHarness").AddComponent<Stage1Manager>();
            manager.lessonEntries = new List<Stage1CharacterEntry>
            {
                new Stage1CharacterEntry("一", "1"),
                new Stage1CharacterEntry("二", "2"),
                new Stage1CharacterEntry("三", "3"),
            };
            manager.stageState = Stage1StageState.CreateDefault(manager.lessonEntries);
            return manager;
        }

        private void Awake()
        {
            EnsureState();
        }

        public void StartStage()
        {
            StartStage(false);
        }

        public void StartStage(bool skipLessonFlow)
        {
            EnsureState();

            StopAdvanceRoutine();

            challengeIndex = 0;
            stageState.Retry();
            handwritingPanel?.ClearPad();
            BuildChallengeOrder();

            if (skipLessonFlow)
            {
                BeginChallenge();
                return;
            }

            ShowLesson();
        }

        public void RetryStage()
        {
            StartStage();
        }

        public void ShowResourcesFromLauncher()
        {
            StopAdvanceRoutine();
            challengePanel?.SetChoiceButtonsInteractable(false);
            SetPanelVisible(lessonPanelGroup, false);
            SetPanelVisible(challengePanelGroup, false);
            SetPanelVisible(resultPanelGroup, false);
        }

        private void ShowLesson()
        {
            if (lessonPanel == null || stageState == null)
            {
                return;
            }

            SetPanelVisible(lessonPanelGroup, true);
            SetPanelVisible(challengePanelGroup, false);
            SetPanelVisible(resultPanelGroup, false);

            bool isFinalLesson = stageState.LessonIndex >= stageState.LessonCount - 1;
            lessonPanel.Show(stageState.CurrentLesson, isFinalLesson, OnLessonContinuePressed);
            handwritingPanel?.Show(stageState.CurrentLesson);

            if (lifeDisplay != null)
            {
                lifeDisplay.SetLives(stageState.Lives);
            }
        }

        private void OnLessonContinuePressed()
        {
            if (stageState == null)
            {
                return;
            }

            if (stageState.AdvanceLesson())
            {
                ShowLesson();
                return;
            }

            BeginChallenge();
        }

        private void BeginChallenge()
        {
            if (challengePanel == null || barrelView == null || stageState == null)
            {
                return;
            }

            stageState.BeginChallenge();
            SetPanelVisible(lessonPanelGroup, false);
            SetPanelVisible(challengePanelGroup, true);
            SetPanelVisible(resultPanelGroup, false);

            challengePanel.ConfigureChoices(GetLessonEntriesArray(), OnChoiceSelected);
            challengePanel.SetPrompt("Pick the matching Chinese character.");
            challengePanel.ShowFeedback(string.Empty);
            challengePanel.SetChoiceButtonsInteractable(true);
            handwritingPanel?.Show(stageState.CurrentLesson);

            if (lifeDisplay != null)
            {
                lifeDisplay.SetLives(stageState.Lives);
            }

            challengeIndex = 0;
            ShowCurrentBarrel();
        }

        private void ShowCurrentBarrel()
        {
            if (stageState == null || challengeIndex >= challengeOrder.Count)
            {
                return;
            }

            Stage1CharacterEntry currentChallenge = challengeOrder[challengeIndex];
            stageState.SetCurrentBarrelLabel(currentChallenge.Meaning);
            barrelView.ResetEncounter(currentChallenge.Meaning);
            challengePanel.SetBarrelLabel(currentChallenge.Meaning);
            challengePanel.ShowFeedback($"Barrel shows {currentChallenge.Meaning}. Choose the matching character.");
            challengePanel.SetChoiceButtonsInteractable(true);
        }

        private void OnChoiceSelected(string chosenCharacter)
        {
            if (stageState == null || barrelView == null)
            {
                return;
            }

            Stage1AnswerResult result = stageState.SubmitAnswer(chosenCharacter);
            if (result == Stage1AnswerResult.Correct)
            {
                barrelView.Explode();
                challengePanel.ShowFeedback("Boom! Correct.");
                challengePanel.SetChoiceButtonsInteractable(false);
                if (advanceRoutine != null)
                {
                    StopCoroutine(advanceRoutine);
                }

                advanceRoutine = StartCoroutine(AdvanceToNextBarrel());
                return;
            }

            if (result == Stage1AnswerResult.Incorrect)
            {
                challengePanel.ShowFeedback("Oops! -1 life.");
                if (lifeDisplay != null)
                {
                    lifeDisplay.SetLives(stageState.Lives);
                }

                return;
            }

            if (result == Stage1AnswerResult.Failed)
            {
                challengePanel.ShowFeedback("Out of lives.");
                challengePanel.SetChoiceButtonsInteractable(false);
                if (lifeDisplay != null)
                {
                    lifeDisplay.SetLives(0);
                }

                ShowFailure();
            }
        }

        private IEnumerator AdvanceToNextBarrel()
        {
            yield return new WaitForSeconds(nextBarrelDelay);
            challengeIndex++;
            if (challengeIndex >= challengeOrder.Count)
            {
                advanceRoutine = null;
                ShowSuccess();
                yield break;
            }

            advanceRoutine = null;
            ShowCurrentBarrel();
        }

        private void ShowFailure()
        {
            StopAdvanceRoutine();
            challengePanel?.SetChoiceButtonsInteractable(false);
            SetPanelVisible(lessonPanelGroup, false);
            SetPanelVisible(challengePanelGroup, false);
            SetPanelVisible(resultPanelGroup, true);

            if (resultPanel != null)
            {
                resultPanel.ShowFailure(RetryStage);
            }
        }

        private void ShowSuccess()
        {
            StopAdvanceRoutine();
            challengePanel?.SetChoiceButtonsInteractable(false);
            stageState?.MarkCleared();
            SetPanelVisible(lessonPanelGroup, false);
            SetPanelVisible(challengePanelGroup, false);
            SetPanelVisible(resultPanelGroup, true);

            if (resultPanel != null)
            {
                resultPanel.ShowSuccess(RetryStage);
            }
        }

        private void BuildChallengeOrder()
        {
            challengeOrder.Clear();
            challengeOrder.AddRange(lessonEntries);

            for (int i = challengeOrder.Count - 1; i > 0; i--)
            {
                int swapIndex = Random.Range(0, i + 1);
                Stage1CharacterEntry temp = challengeOrder[i];
                challengeOrder[i] = challengeOrder[swapIndex];
                challengeOrder[swapIndex] = temp;
            }
        }

        private Stage1CharacterEntry[] GetLessonEntriesArray()
        {
            return lessonEntries.ToArray();
        }

        private void EnsureState()
        {
            NormalizeLessonEntries();

            if (stageState != null)
            {
                return;
            }

            stageState = Stage1StageState.CreateDefault(lessonEntries);
        }

        private void NormalizeLessonEntries()
        {
            if (lessonEntries != null &&
                lessonEntries.Count == 3 &&
                lessonEntries[0].Character == "一" &&
                lessonEntries[0].Meaning == "1" &&
                lessonEntries[1].Character == "二" &&
                lessonEntries[1].Meaning == "2" &&
                lessonEntries[2].Character == "三" &&
                lessonEntries[2].Meaning == "3")
            {
                return;
            }

            lessonEntries = new List<Stage1CharacterEntry>
            {
                new Stage1CharacterEntry("一", "1"),
                new Stage1CharacterEntry("二", "2"),
                new Stage1CharacterEntry("三", "3"),
            };
        }

        private static void SetPanelVisible(CanvasGroup group, bool visible)
        {
            if (group == null)
            {
                return;
            }

            group.alpha = visible ? 1f : 0f;
            group.interactable = visible;
            group.blocksRaycasts = visible;
            group.gameObject.SetActive(visible);
        }

        private void StopAdvanceRoutine()
        {
            if (advanceRoutine == null)
            {
                return;
            }

            StopCoroutine(advanceRoutine);
            advanceRoutine = null;
        }
    }
}
