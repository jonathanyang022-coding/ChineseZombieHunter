using NUnit.Framework;

namespace ChineseZombieHunter.Tests
{
    public class Stage1ManagerTests
    {
        [Test]
        public void CreateForTests_StartsOnFirstLesson()
        {
            var manager = Stage1Manager.CreateForTests();

            Assert.AreEqual("一", manager.CurrentLessonCharacter);
            Assert.AreEqual("1", manager.CurrentLessonMeaning);
            Assert.AreEqual(3, manager.CurrentLives);
        }
    }
}
