// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LinuxHunspell.cs" company="">
//   
// </copyright>
// <summary>
//   The linux hunspell.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Nikse.SubtitleEdit.Logic.SpellCheck
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;
    using System.Windows.Forms;

    /// <summary>
    /// The linux hunspell.
    /// </summary>
    public class LinuxHunspell : Hunspell
    {
        /// <summary>
        /// The _hunspell handle.
        /// </summary>
        private IntPtr _hunspellHandle = IntPtr.Zero;

        /// <summary>
        /// Initializes a new instance of the <see cref="LinuxHunspell"/> class.
        /// </summary>
        /// <param name="affDirectory">
        /// The aff directory.
        /// </param>
        /// <param name="dicDictory">
        /// The dic dictory.
        /// </param>
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

        /// <summary>
        /// Finalizes an instance of the <see cref="LinuxHunspell"/> class. 
        /// </summary>
        ~LinuxHunspell()
        {
            this.Dispose(false);
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
            return NativeMethods.Hunspell_spell(this._hunspellHandle, word) != 0;
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
                // ReleaseManagedResources();
            }

            this.ReleaseUnmangedResources();
        }

        /// <summary>
        /// The release unmanged resources.
        /// </summary>
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