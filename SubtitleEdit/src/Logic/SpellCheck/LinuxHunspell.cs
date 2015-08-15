namespace Nikse.SubtitleEdit.Logic.SpellCheck
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;
    using System.Windows.Forms;

    public class LinuxHunspell : Hunspell
    {
        private IntPtr _hunspellHandle = IntPtr.Zero;

        public LinuxHunspell(string affDirectory, string dicDictory)
        {
            // Also search - /usr/share/hunspell
            try
            {
                this._hunspellHandle = NativeMethods.Hunspell_create(affDirectory, dicDictory);
            }
            catch
            {
                MessageBox.Show("Unable to start hunspell spell checker - make sure hunspell is installed!");
                throw;
            }
        }

        ~LinuxHunspell()
        {
            this.Dispose(false);
        }

        public override bool Spell(string word)
        {
            return NativeMethods.Hunspell_spell(this._hunspellHandle, word) != 0;
        }

        public override List<string> Suggest(string word)
        {
            IntPtr pointerToAddressStringArray = Marshal.AllocHGlobal(IntPtr.Size);
            int resultCount = NativeMethods.Hunspell_suggest(this._hunspellHandle, pointerToAddressStringArray, word);
            IntPtr addressStringArray = Marshal.ReadIntPtr(pointerToAddressStringArray);
            List<string> results = new List<string>();
            for (int i = 0; i < resultCount; i++)
            {
                IntPtr addressCharArray = Marshal.ReadIntPtr(addressStringArray, i * IntPtr.Size);
                string suggestion = Marshal.PtrToStringAuto(addressCharArray);
                if (!string.IsNullOrEmpty(suggestion))
                {
                    results.Add(suggestion);
                }
            }

            NativeMethods.Hunspell_free_list(this._hunspellHandle, pointerToAddressStringArray, resultCount);
            Marshal.FreeHGlobal(pointerToAddressStringArray);

            return results;
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
                // ReleaseManagedResources();
            }

            this.ReleaseUnmangedResources();
        }

        private void ReleaseUnmangedResources()
        {
            if (this._hunspellHandle != IntPtr.Zero)
            {
                NativeMethods.Hunspell_destroy(this._hunspellHandle);
                this._hunspellHandle = IntPtr.Zero;
            }
        }
    }
}