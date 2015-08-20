// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Hunspell.cs" company="">
//   
// </copyright>
// <summary>
//   The hunspell.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Nikse.SubtitleEdit.Logic.SpellCheck
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// The hunspell.
    /// </summary>
    public abstract class Hunspell : IDisposable
    {
        /// <summary>
        /// The dispose.
        /// </summary>
        public virtual void Dispose()
        {
        }

        /// <summary>
        /// The get hunspell.
        /// </summary>
        /// <param name="dictionary">
        /// The dictionary.
        /// </param>
        /// <returns>
        /// The <see cref="Hunspell"/>.
        /// </returns>
        public static Hunspell GetHunspell(string dictionary)
        {
            if (Configuration.IsRunningOnLinux())
            {
                return new LinuxHunspell(dictionary + ".aff", dictionary + ".dic");
            }

            if (Configuration.IsRunningOnMac())
            {
                return new MacHunspell(dictionary + ".aff", dictionary + ".dic");
            }

            // Finnish uses Voikko (not available via hunspell)
            if (dictionary.EndsWith("fi_fi", StringComparison.OrdinalIgnoreCase))
            {
                return new VoikkoSpellCheck(Configuration.BaseDirectory, Configuration.DictionariesFolder);
            }

            return new WindowsHunspell(dictionary + ".aff", dictionary + ".dic");
        }

        /// <summary>
        /// The spell.
        /// </summary>
        /// <param name="word">
        /// The word.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public abstract bool Spell(string word);

        /// <summary>
        /// The suggest.
        /// </summary>
        /// <param name="word">
        /// The word.
        /// </param>
        /// <returns>
        /// The <see cref="List"/>.
        /// </returns>
        public abstract List<string> Suggest(string word);
    }
}