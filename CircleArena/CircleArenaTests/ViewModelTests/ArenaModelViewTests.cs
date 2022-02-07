using System.Linq;
using CircleArena.Models.Events;
using CircleArena.ViewModels;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CircleArenaTests.ViewModelTests
{
    [TestClass]
    public class ArenaModelViewTests
    {
        /// <summary>
        /// The purpose of this test is to ensure that our event stack resets appropriately when our pointer is midway
        /// </summary>
        [TestMethod]
        public void LastEvent_StackResetTest()
        {
            // Arrange
            var arenaViewModel = new ArenaViewModel();

            // Act
            arenaViewModel.LastEvent = new ArenaEventCreate();

            arenaViewModel.LastEvent = new ArenaEventCreate();
            arenaViewModel.LastEvent = new ArenaEventCreate();
            arenaViewModel.Undo(null);
            arenaViewModel.Undo(null);

            arenaViewModel.LastEvent = new ArenaEventMove();
            
            // Assert
            Assert.AreEqual(arenaViewModel.Events.Count, 2);
            Assert.IsTrue(arenaViewModel.Events.Last() is ArenaEventMove);
        }

        [TestMethod]
        public void EventStack_UndoButtonVisibility_Default()
        {
            // Arrange
            var arenaViewModel = new ArenaViewModel();

            // Act

            // Assert
            Assert.IsFalse(arenaViewModel.HasRedoActions);
            Assert.IsFalse(arenaViewModel.HasUndoActions);
        }

        [TestMethod]
        public void EventStack_UndoButtonVisibility_HasUndo()
        {
            // Arrange
            var arenaViewModel = new ArenaViewModel();

            // Act
            arenaViewModel.LastEvent = new ArenaEventCreate();

            // Assert
            Assert.IsFalse(arenaViewModel.HasRedoActions);
            Assert.IsTrue(arenaViewModel.HasUndoActions);
        }

        [TestMethod]
        public void EventStack_RedoButtonVisibility_HasRedo()
        {
            // Arrange
            var arenaViewModel = new ArenaViewModel();

            // Act
            arenaViewModel.LastEvent = new ArenaEventCreate();
            arenaViewModel.Undo(null);

            // Assert
            Assert.IsTrue(arenaViewModel.HasRedoActions);
            Assert.IsFalse(arenaViewModel.HasUndoActions);
        }

        [TestMethod]
        public void EventStack_CreateAndUndo()
        {
            // Arrange
            var arenaViewModel = new ArenaViewModel();

            // Act and assert
            arenaViewModel.AddCircle(null);

            Assert.IsTrue(arenaViewModel.Circles.Count == 1);

            arenaViewModel.Undo(null);

            Assert.IsTrue(arenaViewModel.Circles.Count == 0);

            arenaViewModel.Redo(null);

            Assert.IsTrue(arenaViewModel.Circles.Count == 1);
        }
    }
}
