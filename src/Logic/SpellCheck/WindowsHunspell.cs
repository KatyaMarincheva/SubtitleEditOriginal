// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WindowsHunspell.cs" company="">
//   
// </copyright>
// <summary>
//   The windows hunspell.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Nikse.SubtitleEdit.Logic.SpellCheck
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// The windows hunspell.
    /// </summary>
    public class WindowsHunspell : Hunspell
    {
        /// <summary>
        /// The _hunspell.
        /// </summary>
        private NHunspell.Hunspell _hunspell;

        /// <summary>
        /// Initializes a new instance of the <see cref="WindowsHunspell"/> class.
        /// </summary>
        /// <param name="affDictionary">
        /// The aff dictionary.
        /// </param>
        /// <param name="dicDictionary">
        /// The dic dictionary.
        /// </param>
        public WindowsHunspell(string affDictionary, string dicDictionary)
        {
            this._hunspell = new NHunspell.Hunspell(affDictionary, dicDictionary);
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
        public override bool Spell(string word)
        {
            return this._hunspell.Spell(word);
        }

        /// <summary>
        /// The suggest.
        /// </summary>
        /// <param name="word">
        /// The word.
        /// </param>
        /// <returns>
        /// The <see cref="List"/>.
        /// </returns>
        public override List<string> Suggest(string word)
        {
            return this._hunspell.Suggest(word);
        }

        /// <summary>
        /// The dispose.
        /// </summary>
        public override void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// The dispose.
        /// </summary>
        /// <param name="disposing">
        /// The disposing.
        /// </param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (this._hunspell != null && !this._hunspell.IsDisposed)
                {
                    this._hunspell.Dispose();
                }

                this._hunspell = null;
            }
        }
    }
}