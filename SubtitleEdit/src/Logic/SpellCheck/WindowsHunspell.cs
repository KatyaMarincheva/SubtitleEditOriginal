namespace Nikse.SubtitleEdit.Logic.SpellCheck
{
    using System;
    using System.Collections.Generic;

    public class WindowsHunspell : Hunspell
    {
        private NHunspell.Hunspell _hunspell;

        public WindowsHunspell(string affDictionary, string dicDictionary)
        {
            this._hunspell = new NHunspell.Hunspell(affDictionary, dicDictionary);
        }

        public override bool Spell(string word)
        {
            return this._hunspell.Spell(word);
        }

        public override List<string> Suggest(string word)
        {
            return this._hunspell.Suggest(word);
        }

        public override void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

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