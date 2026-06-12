using NUnit.Framework;

namespace ChineseZombieHunter.Tests
{
    public class Stage1StageStateTests
    {
        [Test]
        public void StartStage_InitializesThreeLives_AndLessonOrder()
        {
            var state = Stage1StageState.CreateDefault(new[]
            {
                new Stage1CharacterEntry("一", "1"),
                new Stage1CharacterEntry("二", "2"),
                new Stage1CharacterEntry("三", "3"),
            });

            Assert.AreEqual(3, state.Lives);
            Assert.AreEqual(Stage1Phase.Lesson, state.Phase);
            Assert.AreEqual("一", state.CurrentLesson.Character);
            Assert.AreEqual("1", state.CurrentLesson.Meaning);
        }

        [Test]
        public void SubmitAnswer_CorrectChoice_DoesNotReduceLives()
        {
            var state = Stage1StageState.CreateDefault(new[]
            {
                new Stage1CharacterEntry("一", "1"),
                new Stage1CharacterEntry("二", "2"),
                new Stage1CharacterEntry("三", "3"),
            });

            state.BeginChallenge();
            state.SetCurrentBarrelLabel("2");

            var result = state.SubmitAnswer("二");

            Assert.AreEqual(Stage1AnswerResult.Correct, result);
            Assert.AreEqual(3, state.Lives);
            Assert.AreEqual(Stage1Phase.Challenge, state.Phase);
        }

        [Test]
        public void SubmitAnswer_WrongChoice_LosesOneLife()
        {
            var state = Stage1StageState.CreateDefault(new[]
            {
                new Stage1CharacterEntry("一", "1"),
                new Stage1CharacterEntry("二", "2"),
                new Stage1CharacterEntry("三", "3"),
            });

            state.BeginChallenge();
            state.SetCurrentBarrelLabel("3");

            var result = state.SubmitAnswer("一");

            Assert.AreEqual(Stage1AnswerResult.Incorrect, result);
            Assert.AreEqual(2, state.Lives);
            Assert.AreEqual(Stage1Phase.Challenge, state.Phase);
        }

        [Test]
        public void SubmitAnswer_WrongChoiceAtZeroLives_EndsInFailure()
        {
            var state = Stage1StageState.CreateDefault(new[]
            {
                new Stage1CharacterEntry("一", "1"),
                new Stage1CharacterEntry("二", "2"),
                new Stage1CharacterEntry("三", "3"),
            });

            state.BeginChallenge();
            state.SetCurrentBarrelLabel("1");
            state.SubmitAnswer("二");
            state.SetCurrentBarrelLabel("2");
            state.SubmitAnswer("三");
            state.SetCurrentBarrelLabel("3");

            var result = state.SubmitAnswer("一");

            Assert.AreEqual(Stage1AnswerResult.Failed, result);
            Assert.AreEqual(0, state.Lives);
            Assert.AreEqual(Stage1Phase.Failed, state.Phase);
        }

        [Test]
        public void Retry_ResetsLivesAndReturnsToLessonPhase()
        {
            var state = Stage1StageState.CreateDefault(new[]
            {
                new Stage1CharacterEntry("一", "1"),
                new Stage1CharacterEntry("二", "2"),
                new Stage1CharacterEntry("三", "3"),
            });

            state.BeginChallenge();
            state.SetCurrentBarrelLabel("1");
            state.SubmitAnswer("二");

            state.Retry();

            Assert.AreEqual(3, state.Lives);
            Assert.AreEqual(Stage1Phase.Lesson, state.Phase);
            Assert.AreEqual("一", state.CurrentLesson.Character);
        }
    }
}
