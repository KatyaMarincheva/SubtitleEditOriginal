// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Program.cs" company="">
//   
// </copyright>
// <summary>
//   The program.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace UpdateLanguageFiles
{
    using System;
    using System.IO;
    using System.Text;

    /// <summary>
    /// The program.
    /// </summary>
    internal class Program
    {
        /// <summary>
        /// The work in progress.
        /// </summary>
        private static string workInProgress = "Updating language files...";

        /// <summary>
        /// The main.
        /// </summary>
        /// <param name="args">
        /// The args.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        private static int Main(string[] args)
        {
            var myName = Environment.GetCommandLineArgs()[0];
            myName = Path.GetFileNameWithoutExtension(string.IsNullOrWhiteSpace(myName) ? System.Reflection.Assembly.GetEntryAssembly().Location : myName);

            if (args.Length != 2)
            {
                Console.Write("Usage: " + myName + @" <LanguageMaster> <LanguageDeserializer>
  <LanguageMaster>       Path to the LanguageMaster xml file (LanguageMaster.xml)
  <LanguageDeserializer> Path to the LanguageDeserializer source code file (LanguageDeserializer.cs)
");
                return 1;
            }

            Console.Write(workInProgress);

            try
            {
                int noOfChanges = 0;

                var language = new Nikse.SubtitleEdit.Logic.Language();
                language.General.Version = FindVersionNumber();
                var languageAsXml = language.GetCurrentLanguageAsXml();
                var oldLanguageAsXml = string.Empty;
                if (File.Exists(args[0]))
                {
                    oldLanguageAsXml = File.ReadAllText(args[0]);
                }

                if (oldLanguageAsXml != languageAsXml)
                {
                    language.Save(args[0]);
                    noOfChanges++;
                    Console.Write(" {0} generated...", Path.GetFileName(args[0]));
                }

                var languageDeserializerContent = Nikse.SubtitleEdit.Logic.LanguageDeserializerGenerator.GenerateCSharpXmlDeserializerForLanguage();
                var oldLanguageDeserializerContent = string.Empty;
                if (File.Exists(args[1]))
                {
                    oldLanguageDeserializerContent = File.ReadAllText(args[1]);
                }

                if (oldLanguageDeserializerContent != languageDeserializerContent)
                {
                    File.WriteAllText(args[1], languageDeserializerContent, Encoding.UTF8);
                    noOfChanges++;
                    Console.Write(" {0} generated...", Path.GetFileName(args[1]));
                }

                if (noOfChanges == 0)
                {
                    Console.WriteLine(" no changes");
                }
                else
                {
                    Console.WriteLine();
                }

                return 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine();
                Console.WriteLine(myName + ": " + ex.Message);

                return 2;
            }
        }

        /// <summary>
        /// The find version number.
        /// </summary>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        private static string FindVersionNumber()
        {
            var fileName = Path.Combine("src", "Properties", "AssemblyInfo.cs.template");
            if (!File.Exists(fileName))
            {
                fileName = Path.Combine("..", fileName);
            }

            if (!File.Exists(fileName))
            {
                fileName = Path.Combine("..", fileName);
            }

            if (!File.Exists(fileName))
            {
                fileName = Path.Combine("..", fileName);
            }

            if (!File.Exists(fileName))
            {
                fileName = Path.Combine("..", fileName);
            }

            if (!File.Exists(fileName))
            {
                fileName = Path.Combine("..", fileName);
            }

            string warning;

            if (File.Exists(fileName))
            {
                var text = File.ReadAllText(fileName);
                var pattern = @"\[assembly: AssemblyVersion\(""(\d+\.\d+\.\d+)\.\[REVNO]""\)]";
                var version = System.Text.RegularExpressions.Regex.Match(text, pattern);
                if (version.Success)
                {
                    return version.Groups[1].Value;
                }

                warning = "No valid AssemblyVersion in template file '" + Path.GetFullPath(fileName) + "'.";
            }
            else
            {
                warning = "Template file '" + Path.GetFileName(fileName) + "' not found.";
            }

            Console.WriteLine();
            Console.WriteLine("WARNING: " + warning);
            Console.Write(workInProgress);

            return "unknown";
        }
    }
}