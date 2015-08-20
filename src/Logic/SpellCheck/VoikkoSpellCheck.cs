// --------------------------------------------------------------------------------------------------------------------
// <copyright file="VoikkoSpellCheck.cs" company="">
//   
// </copyright>
// <summary>
//   The voikko spell check.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Nikse.SubtitleEdit.Logic.SpellCheck
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Runtime.InteropServices;
    using System.Text;

    /// <summary>
    /// The voikko spell check.
    /// </summary>
    public class VoikkoSpellCheck : Hunspell
    {
        /// <summary>
        /// The _lib dll.
        /// </summary>
        private IntPtr _libDll = IntPtr.Zero;

        /// <summary>
        /// The _lib voikko.
        /// </summary>
        private IntPtr _libVoikko = IntPtr.Zero;

        /// <summary>
        /// The _voikko free cstr array.
        /// </summary>
        private VoikkoFreeCstrArray _voikkoFreeCstrArray;

        /// <summary>
        /// The _voikko init.
        /// </summary>
        private VoikkoInit _voikkoInit;

        /// <summary>
        /// The _voikko spell.
        /// </summary>
        private VoikkoSpell _voikkoSpell;

        /// <summary>
        /// The _voikko suggest.
        /// </summary>
        private VoikkoSuggest _voikkoSuggest;

        /// <summary>
        /// The _voikko terminate.
        /// </summary>
        private VoikkoTerminate _voikkoTerminate;

        /// <summary>
        /// Initializes a new instance of the <see cref="VoikkoSpellCheck"/> class.
        /// </summary>
        /// <param name="baseFolder">
        /// The base folder.
        /// </param>
        /// <param name="dictionaryFolder">
        /// The dictionary folder.
        /// </param>
        /// <exception cref="Exception">
        /// </exception>
        public VoikkoSpellCheck(string baseFolder, string dictionaryFolder)
        {
            this.LoadLibVoikkoDynamic(baseFolder);

            IntPtr error = new IntPtr();
            this._libVoikko = this._voikkoInit(ref error, S2N("fi"), S2Ansi(dictionaryFolder));
            if (this._libVoikko == IntPtr.Zero && error != IntPtr.Zero)
            {
                throw new Exception(N2S(error));
            }
        }

        /// <summary>
        /// Finalizes an instance of the <see cref="VoikkoSpellCheck"/> class. 
        /// </summary>
        ~VoikkoSpellCheck()
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
            if (string.IsNullOrEmpty(word))
            {
                return false;
            }

            return Convert.ToBoolean(this._voikkoSpell(this._libVoikko, S2N(word)));
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
            List<string> suggestions = new List<string>();
            if (string.IsNullOrEmpty(word))
            {
                return suggestions;
            }

            IntPtr voikkoSuggestCstr = this._voikkoSuggest(this._libVoikko, S2N(word));
            if (voikkoSuggestCstr == IntPtr.Zero)
            {
                return suggestions;
            }

            unsafe
            {
                for (byte** cStr = (byte**)voikkoSuggestCstr; *cStr != (byte*)0; cStr++)
                {
                    suggestions.Add(N2S(new IntPtr(*cStr)));
                }
            }

            this._voikkoFreeCstrArray(voikkoSuggestCstr);
            return suggestions;
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
        /// The n 2 s.
        /// </summary>
        /// <param name="ptr">
        /// The ptr.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        private static string N2S(IntPtr ptr)
        {
            if (ptr == IntPtr.Zero)
            {
                return null;
            }

            List<byte> bytes = new List<byte>();
            unsafe
            {
                for (byte* p = (byte*)ptr; *p != 0; p++)
                {
                    bytes.Add(*p);
                }
            }

            return N2S(bytes.ToArray());
        }

        /// <summary>
        /// The n 2 s.
        /// </summary>
        /// <param name="bytes">
        /// The bytes.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        private static string N2S(byte[] bytes)
        {
            if (bytes == null)
            {
                return null;
            }

            return Encoding.UTF8.GetString(bytes);
        }

        /// <summary>
        /// The s 2 n.
        /// </summary>
        /// <param name="str">
        /// The str.
        /// </param>
        /// <returns>
        /// The <see cref="byte[]"/>.
        /// </returns>
        private static byte[] S2N(string str)
        {
            return S2Encoding(str, Encoding.UTF8);
        }

        /// <summary>
        /// The s 2 ansi.
        /// </summary>
        /// <param name="str">
        /// The str.
        /// </param>
        /// <returns>
        /// The <see cref="byte[]"/>.
        /// </returns>
        private static byte[] S2Ansi(string str)
        {
            return S2Encoding(str, Encoding.Default);
        }

        /// <summary>
        /// The s 2 encoding.
        /// </summary>
        /// <param name="str">
        /// The str.
        /// </param>
        /// <param name="encoding">
        /// The encoding.
        /// </param>
        /// <returns>
        /// The <see cref="byte[]"/>.
        /// </returns>
        private static byte[] S2Encoding(string str, Encoding encoding)
        {
            if (str == null)
            {
                return null;
            }

            return encoding.GetBytes(str + '\0');
        }

        /// <summary>
        /// The get dll type.
        /// </summary>
        /// <param name="type">
        /// The type.
        /// </param>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <returns>
        /// The <see cref="object"/>.
        /// </returns>
        private object GetDllType(Type type, string name)
        {
            IntPtr address = NativeMethods.GetProcAddress(this._libDll, name);
            if (address != IntPtr.Zero)
            {
                return Marshal.GetDelegateForFunctionPointer(address, type);
            }

            return null;
        }

        /// <summary>
        /// Load dll dynamic + set pointers to needed methods
        /// </summary>
        /// <param name="baseFolder">
        /// </param>
        private void LoadLibVoikkoDynamic(string baseFolder)
        {
            string dllFile = Path.Combine(baseFolder, "Voikkox86.dll");
            if (IntPtr.Size == 8)
            {
                dllFile = Path.Combine(baseFolder, "Voikkox64.dll");
            }

            if (!File.Exists(dllFile))
            {
                throw new FileNotFoundException(dllFile);
            }

            this._libDll = NativeMethods.LoadLibrary(dllFile);
            if (this._libDll == IntPtr.Zero)
            {
                throw new FileLoadException("Unable to load " + dllFile);
            }

            this._voikkoInit = (VoikkoInit)this.GetDllType(typeof(VoikkoInit), "voikkoInit");
            this._voikkoTerminate = (VoikkoTerminate)this.GetDllType(typeof(VoikkoTerminate), "voikkoTerminate");
            this._voikkoSpell = (VoikkoSpell)this.GetDllType(typeof(VoikkoSpell), "voikkoSpellCstr");
            this._voikkoSuggest = (VoikkoSuggest)this.GetDllType(typeof(VoikkoSuggest), "voikkoSuggestCstr");
            this._voikkoFreeCstrArray = (VoikkoFreeCstrArray)this.GetDllType(typeof(VoikkoFreeCstrArray), "voikkoFreeCstrArray");

            if (this._voikkoInit == null || this._voikkoTerminate == null || this._voikkoSpell == null || this._voikkoSuggest == null || this._voikkoFreeCstrArray == null)
            {
                throw new FileLoadException("Not all methods in Voikko dll could be found!");
            }
        }

        /// <summary>
        /// The release unmanged resources.
        /// </summary>
        private void ReleaseUnmangedResources()
        {
            try
            {
                if (this._libVoikko != IntPtr.Zero)
                {
                    this._voikkoTerminate(this._libVoikko);
                    this._libVoikko = IntPtr.Zero;
                }

                if (this._libDll != IntPtr.Zero)
                {
                    NativeMethods.FreeLibrary(this._libDll);
                    this._libDll = IntPtr.Zero;
                }
            }
            catch
            {
            }
        }

        // Voikko functions in dll
        /// <summary>
        /// The voikko init.
        /// </summary>
        /// <param name="error">
        /// The error.
        /// </param>
        /// <param name="languageCode">
        /// The language code.
        /// </param>
        /// <param name="path">
        /// The path.
        /// </param>
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate IntPtr VoikkoInit(ref IntPtr error, byte[] languageCode, byte[] path);

        /// <summary>
        /// The voikko terminate.
        /// </summary>
        /// <param name="libVlc">
        /// The lib vlc.
        /// </param>
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate void VoikkoTerminate(IntPtr libVlc);

        /// <summary>
        /// The voikko spell.
        /// </summary>
        /// <param name="handle">
        /// The handle.
        /// </param>
        /// <param name="word">
        /// The word.
        /// </param>
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate int VoikkoSpell(IntPtr handle, byte[] word);

        /// <summary>
        /// The voikko suggest.
        /// </summary>
        /// <param name="handle">
        /// The handle.
        /// </param>
        /// <param name="word">
        /// The word.
        /// </param>
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate IntPtr VoikkoSuggest(IntPtr handle, byte[] word);

        /// <summary>
        /// The voikko free cstr array.
        /// </summary>
        /// <param name="array">
        /// The array.
        /// </param>
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate IntPtr VoikkoFreeCstrArray(IntPtr array);
    }
}