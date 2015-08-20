// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NamesListTest.cs" company="">
//   
// </copyright>
// <summary>
//   The names list test.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Test.Logic.Dictionaries
{
    using System.IO;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Nikse.SubtitleEdit.Logic.Dictionaries;

    /// <summary>
    /// The names list test.
    /// </summary>
    [TestClass]
    public class NamesListTest
    {
        /// <summary>
        /// The names list add word.
        /// </summary>
        [TestMethod]
        public void NamesListAddWord()
        {
            // Arrange
            var namesList = new NamesList(Directory.GetCurrentDirectory(), "en", false, null);

            // Act
            namesList.Add("Jones");
            var exists = namesList.GetNames().Contains("Jones");

            // Assert
            Assert.IsTrue(exists);
        }

        /// <summary>
        /// The names list add multi word.
        /// </summary>
        [TestMethod]
        public void NamesListAddMultiWord()
        {
            // Arrange
            var namesList = new NamesList(Directory.GetCurrentDirectory(), "en", false, null);

            // Act
            namesList.Add("Charlie Parker");
            var exists = namesList.GetMultiNames().Contains("Charlie Parker");

            // Assert
            Assert.IsTrue(exists);
        }

        /// <summary>
        /// The names list is in names etc multi word list.
        /// </summary>
        [TestMethod]
        public void NamesListIsInNamesEtcMultiWordList()
        {
            // Arrange
            var namesList = new NamesList(Directory.GetCurrentDirectory(), "en", false, null);

            // Act
            namesList.Add("Charlie Parker");
            var exists = namesList.IsInNamesEtcMultiWordList("This is Charlie Parker!", "Charlie Parker");

            // Assert
            Assert.IsTrue(exists);
        }

        /// <summary>
        /// The names list not in list.
        /// </summary>
        [TestMethod]
        public void NamesListNotInList()
        {
            // Arrange
            var namesList = new NamesList(Directory.GetCurrentDirectory(), "en", false, null);

            // Act
            var exists = namesList.GetNames().Contains("JonesASDFLKJCKJXFLKJSLDKFJASDF");

            // Assert
            Assert.IsFalse(exists);
        }

        /// <summary>
        /// The names list add word reload.
        /// </summary>
        public void NamesListAddWordReload()
        {
            // Arrange
            var namesList = new NamesList(Directory.GetCurrentDirectory(), "en", false, null);
            namesList.Add("Jones");

            // Act
            namesList = new NamesList(Directory.GetCurrentDirectory(), "en", false, null);

            // Assert
            Assert.IsTrue(namesList.GetNames().Contains("Jones"));
        }

        /// <summary>
        /// The names list remove.
        /// </summary>
        [TestMethod]
        public void NamesListRemove()
        {
            // Arrange
            var namesList = new NamesList(Directory.GetCurrentDirectory(), "en", false, null);
            namesList.Add("Jones");

            // Act
            namesList.Remove("Jones");

            // Assert
            Assert.IsFalse(namesList.GetNames().Contains("Jones"));
        }

        /// <summary>
        /// The names list remove reload.
        /// </summary>
        [TestMethod]
        public void NamesListRemoveReload()
        {
            // Arrange
            var namesList = new NamesList(Directory.GetCurrentDirectory(), "da", false, null);
            namesList.Add("Jones");

            // Act
            namesList.Remove("Jones");
            namesList = new NamesList(Directory.GetCurrentDirectory(), "da", false, null);

            // Assert
            Assert.IsFalse(namesList.GetNames().Contains("Jones"));
        }
    }
}