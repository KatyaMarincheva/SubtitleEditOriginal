// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LanguageTest.cs" company="">
//   
// </copyright>
// <summary>
//   Summary description for languageTest
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Test.Logic
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Xml;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Nikse.SubtitleEdit.Logic;

    /// <summary>
    /// Summary description for languageTest
    /// </summary>
    [TestClass]
    public class LanguageTest
    {
        /// <summary>
        /// The _list.
        /// </summary>
        private List<string> _list; // Store the list of existing languages

        /// <summary>
        /// Initializes a new instance of the <see cref="LanguageTest"/> class. 
        /// Load a list of currently existing languages
        /// </summary>
        public LanguageTest()
        {
            this._list = new List<string>();
            if (Directory.Exists(Path.Combine(Configuration.BaseDirectory, "Languages")))
            {
                string[] versionInfo = Utilities.AssemblyVersion.Split('.');
                string currentVersion = string.Format("{0}.{1}.{2}", versionInfo[0], versionInfo[1], versionInfo[2]);

                foreach (string fileName in Directory.GetFiles(Path.Combine(Configuration.BaseDirectory, "Languages"), "*.xml"))
                {
                    var doc = new XmlDocument();
                    doc.Load(fileName);
                    string version = doc.DocumentElement.SelectSingleNode("General/Version").InnerText;
                    if (version == currentVersion)
                    {
                        string cultureName = Path.GetFileNameWithoutExtension(fileName);
                        this._list.Add(cultureName);
                    }
                }
            }

            this._list.Sort();
        }
    }
}