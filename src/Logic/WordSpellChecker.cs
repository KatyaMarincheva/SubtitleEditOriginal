// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WordSpellChecker.cs" company="">
//   
// </copyright>
// <summary>
//   Microsoft Word methods (late bound) for spell checking by Nikse
//   Mostly a bunch of hacks...
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Nikse.SubtitleEdit.Logic
{
    using System;
    using System.Reflection;
    using System.Runtime.InteropServices;
    using System.Windows.Forms;

    using Nikse.SubtitleEdit.Forms;

    /// <summary>
    /// Microsoft Word methods (late bound) for spell checking by Nikse
    /// Mostly a bunch of hacks...
    /// </summary>
    internal class WordSpellChecker
    {
        /// <summary>
        /// The hwn d_ bottom.
        /// </summary>
        private const int HWND_BOTTOM = 1;

        /// <summary>
        /// The sw p_ noactivate.
        /// </summary>
        private const int SWP_NOACTIVATE = 0x0010;

        /// <summary>
        /// The sw p_ nomove.
        /// </summary>
        private const short SWP_NOMOVE = 0X2;

        /// <summary>
        /// The sw p_ nosize.
        /// </summary>
        private const short SWP_NOSIZE = 1;

        /// <summary>
        /// The sw p_ nozorder.
        /// </summary>
        private const short SWP_NOZORDER = 0X4;

        /// <summary>
        /// The sw p_ showwindow.
        /// </summary>
        private const int SWP_SHOWWINDOW = 0x0040;

        /// <summary>
        /// The wd window state normal.
        /// </summary>
        private const int wdWindowStateNormal = 0;

        /// <summary>
        /// The wd window state maximize.
        /// </summary>
        private const int wdWindowStateMaximize = 1;

        /// <summary>
        /// The wd window state minimize.
        /// </summary>
        private const int wdWindowStateMinimize = 2;

        /// <summary>
        /// The _language id.
        /// </summary>
        private int _languageId = 1033; // English

        /// <summary>
        /// The _main handle.
        /// </summary>
        private IntPtr _mainHandle;

        /// <summary>
        /// The _word application.
        /// </summary>
        private object _wordApplication;

        /// <summary>
        /// The _word application type.
        /// </summary>
        private Type _wordApplicationType;

        /// <summary>
        /// The _word document.
        /// </summary>
        private object _wordDocument;

        /// <summary>
        /// The _word document type.
        /// </summary>
        private Type _wordDocumentType;

        /// <summary>
        /// Initializes a new instance of the <see cref="WordSpellChecker"/> class.
        /// </summary>
        /// <param name="main">
        /// The main.
        /// </param>
        /// <param name="languageId">
        /// The language id.
        /// </param>
        public WordSpellChecker(Main main, string languageId)
        {
            this._mainHandle = main.Handle;
            this.SetLanguageId(languageId);

            this._wordApplicationType = Type.GetTypeFromProgID("Word.Application");
            this._wordApplication = Activator.CreateInstance(this._wordApplicationType);

            Application.DoEvents();
            this._wordApplicationType.InvokeMember("WindowState", BindingFlags.SetProperty, null, this._wordApplication, new object[] { wdWindowStateNormal });
            this._wordApplicationType.InvokeMember("Top", BindingFlags.SetProperty, null, this._wordApplication, new object[] { -5000 }); // hide window - it's a hack
            Application.DoEvents();
        }

        /// <summary>
        /// Gets the version.
        /// </summary>
        public string Version
        {
            get
            {
                object wordVersion = this._wordApplicationType.InvokeMember("Version", BindingFlags.GetProperty, null, this._wordApplication, null);
                return wordVersion.ToString();
            }
        }

        /// <summary>
        /// The set language id.
        /// </summary>
        /// <param name="languageId">
        /// The language id.
        /// </param>
        private void SetLanguageId(string languageId)
        {
            try
            {
                var ci = new System.Globalization.CultureInfo(languageId);
                this._languageId = ci.LCID;
            }
            catch
            {
                this._languageId = System.Globalization.CultureInfo.CurrentUICulture.LCID;
            }
        }

        /// <summary>
        /// The new document.
        /// </summary>
        public void NewDocument()
        {
            this._wordDocumentType = Type.GetTypeFromProgID("Word.Document");
            this._wordDocument = Activator.CreateInstance(this._wordDocumentType);
        }

        /// <summary>
        /// The close document.
        /// </summary>
        public void CloseDocument()
        {
            object saveChanges = false;
            object p = Missing.Value;
            this._wordDocumentType.InvokeMember("Close", BindingFlags.InvokeMethod, null, this._wordDocument, new[] { saveChanges, p, p });
        }

        /// <summary>
        /// The quit.
        /// </summary>
        public void Quit()
        {
            object saveChanges = false;
            object originalFormat = Missing.Value;
            object routeDocument = Missing.Value;
            this._wordApplicationType.InvokeMember("Quit", BindingFlags.InvokeMethod, null, this._wordApplication, new[] { saveChanges, originalFormat, routeDocument });
            try
            {
                Marshal.ReleaseComObject(this._wordDocument);
                Marshal.ReleaseComObject(this._wordApplication);
            }
            finally
            {
                this._wordDocument = null;
                this._wordApplication = null;
            }
        }

        /// <summary>
        /// The check spelling.
        /// </summary>
        /// <param name="text">
        /// The text.
        /// </param>
        /// <param name="errorsBefore">
        /// The errors before.
        /// </param>
        /// <param name="errorsAfter">
        /// The errors after.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public string CheckSpelling(string text, out int errorsBefore, out int errorsAfter)
        {
            // insert text
            object words = this._wordDocumentType.InvokeMember("Words", BindingFlags.GetProperty, null, this._wordDocument, null);
            object range = words.GetType().InvokeMember("First", BindingFlags.GetProperty, null, words, null);
            range.GetType().InvokeMember("InsertBefore", BindingFlags.InvokeMethod, null, range, new object[] { text });

            // set language...
            range.GetType().InvokeMember("LanguageId", BindingFlags.SetProperty, null, range, new object[] { this._languageId });

            // spell check error count
            object spellingErrors = this._wordDocumentType.InvokeMember("SpellingErrors", BindingFlags.GetProperty, null, this._wordDocument, null);
            object spellingErrorsCount = spellingErrors.GetType().InvokeMember("Count", BindingFlags.GetProperty, null, spellingErrors, null);
            errorsBefore = int.Parse(spellingErrorsCount.ToString());
            Marshal.ReleaseComObject(spellingErrors);

            // perform spell check
            object p = Missing.Value;
            if (errorsBefore > 0)
            {
                this._wordApplicationType.InvokeMember("WindowState", BindingFlags.SetProperty, null, this._wordApplication, new object[] { wdWindowStateNormal });
                this._wordApplicationType.InvokeMember("Top", BindingFlags.SetProperty, null, this._wordApplication, new object[] { -10000 }); // hide window - it's a hack
                NativeMethods.SetWindowPos(this._mainHandle, HWND_BOTTOM, 0, 0, 0, 0, SWP_NOMOVE | SWP_NOSIZE | SWP_NOACTIVATE); // make sure c# form is behind spell check dialog
                this._wordDocumentType.InvokeMember("CheckSpelling", BindingFlags.InvokeMethod, null, this._wordDocument, new[] { p, p, p, p, p, p, p, p, p, p, p, p }); // 12 parameters
                NativeMethods.SetWindowPos(this._mainHandle, 0, 0, 0, 0, 0, SWP_SHOWWINDOW | SWP_NOMOVE | SWP_NOSIZE | SWP_NOACTIVATE); // bring c# form to front again
                this._wordApplicationType.InvokeMember("Top", BindingFlags.SetProperty, null, this._wordApplication, new object[] { -10000 }); // hide window - it's a hack
            }

            // spell check error count
            spellingErrors = this._wordDocumentType.InvokeMember("SpellingErrors", BindingFlags.GetProperty, null, this._wordDocument, null);
            spellingErrorsCount = spellingErrors.GetType().InvokeMember("Count", BindingFlags.GetProperty, null, spellingErrors, null);
            errorsAfter = int.Parse(spellingErrorsCount.ToString());
            Marshal.ReleaseComObject(spellingErrors);

            // Get spell check text
            object resultText = range.GetType().InvokeMember("Text", BindingFlags.GetProperty, null, range, null);
            range.GetType().InvokeMember("Delete", BindingFlags.InvokeMethod, null, range, null);

            Marshal.ReleaseComObject(words);
            Marshal.ReleaseComObject(range);

            return resultText.ToString().TrimEnd(); // result needs a trimming at the end
        }
    }
}