namespace Nikse.SubtitleEdit.Logic.SpellCheck
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Runtime.InteropServices;
    using System.Text;

    public class VoikkoSpellCheck : Hunspell
    {
        private IntPtr _libDll = IntPtr.Zero;

        private IntPtr _libVoikko = IntPtr.Zero;

        private VoikkoFreeCstrArray _voikkoFreeCstrArray;

        private VoikkoInit _voikkoInit;

        private VoikkoSpell _voikkoSpell;

        private VoikkoSuggest _voikkoSuggest;

        private VoikkoTerminate _voikkoTerminate;

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

        ~VoikkoSpellCheck()
        {
            this.Dispose(false);
        }

        // Voikko functions in dll
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate IntPtr VoikkoInit(ref IntPtr error, byte[] languageCode, byte[] path);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate void VoikkoTerminate(IntPtr libVlc);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate int VoikkoSpell(IntPtr handle, byte[] word);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate IntPtr VoikkoSuggest(IntPtr handle, byte[] word);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate IntPtr VoikkoFreeCstrArray(IntPtr array);

        public override bool Spell(string word)
        {
            if (string.IsNullOrEmpty(word))
            {
                return false;
            }

            return Convert.ToBoolean(this._voikkoSpell(this._libVoikko, S2N(word)));
        }

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

        private static string N2S(byte[] bytes)
        {
            if (bytes == null)
            {
                return null;
            }

            return Encoding.UTF8.GetString(bytes);
        }

        private static byte[] S2N(string str)
        {
            return S2Encoding(str, Encoding.UTF8);
        }

        private static byte[] S2Ansi(string str)
        {
            return S2Encoding(str, Encoding.Default);
        }

        private static byte[] S2Encoding(string str, Encoding encoding)
        {
            if (str == null)
            {
                return null;
            }

            return encoding.GetBytes(str + '\0');
        }

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
        ///     Load dll dynamic + set pointers to needed methods
        /// </summary>
        /// <param name="baseFolder"></param>
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
    }
}