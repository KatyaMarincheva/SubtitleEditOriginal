// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Configuration.cs" company="">
//   
// </copyright>
// <summary>
//   Configuration settings via Singleton pattern
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Nikse.SubtitleEdit.Logic
{
    using System;
    using System.IO;

    using Nikse.SubtitleEdit.Core;

    /// <summary>
    /// Configuration settings via Singleton pattern
    /// </summary>
    public class Configuration
    {
        /// <summary>
        /// The _instance.
        /// </summary>
        private static readonly Lazy<Configuration> _instance = new Lazy<Configuration>(() => new Configuration());

        /// <summary>
        /// The _base dir.
        /// </summary>
        private readonly string _baseDir;

        /// <summary>
        /// The _data dir.
        /// </summary>
        private readonly string _dataDir;

        /// <summary>
        /// The _settings.
        /// </summary>
        private readonly Lazy<Settings> _settings;

        /// <summary>
        /// Prevents a default instance of the <see cref="Configuration"/> class from being created.
        /// </summary>
        private Configuration()
        {
            this._baseDir = GetBaseDirectory();
            this._dataDir = this.GetDataDirectory();
            this._settings = new Lazy<Settings>(Settings.GetSettings);
        }

        /// <summary>
        /// Gets the settings file name.
        /// </summary>
        public static string SettingsFileName
        {
            get
            {
                return DataDirectory + "Settings.xml";
            }
        }

        /// <summary>
        /// Gets the dictionaries folder.
        /// </summary>
        public static string DictionariesFolder
        {
            get
            {
                return DataDirectory + "Dictionaries" + Path.DirectorySeparatorChar;
            }
        }

        /// <summary>
        /// Gets the icons folder.
        /// </summary>
        public static string IconsFolder
        {
            get
            {
                return BaseDirectory + "Icons" + Path.DirectorySeparatorChar;
            }
        }

        /// <summary>
        /// Gets the tesseract data folder.
        /// </summary>
        public static string TesseractDataFolder
        {
            get
            {
                if (IsRunningOnLinux() || IsRunningOnMac())
                {
                    if (Directory.Exists("/usr/share/tesseract-ocr/tessdata"))
                    {
                        return "/usr/share/tesseract-ocr/tessdata";
                    }

                    if (Directory.Exists("/usr/share/tesseract/tessdata"))
                    {
                        return "/usr/share/tesseract/tessdata";
                    }

                    if (Directory.Exists("/usr/share/tessdata"))
                    {
                        return "/usr/share/tessdata";
                    }
                }

                return TesseractFolder + "tessdata";
            }
        }

        /// <summary>
        /// Gets the tesseract original folder.
        /// </summary>
        public static string TesseractOriginalFolder
        {
            get
            {
                return BaseDirectory + "Tesseract" + Path.DirectorySeparatorChar;
            }
        }

        /// <summary>
        /// Gets the tesseract folder.
        /// </summary>
        public static string TesseractFolder
        {
            get
            {
                return DataDirectory + "Tesseract" + Path.DirectorySeparatorChar;
            }
        }

        /// <summary>
        /// Gets the vob sub compare folder.
        /// </summary>
        public static string VobSubCompareFolder
        {
            get
            {
                return DataDirectory + "VobSub" + Path.DirectorySeparatorChar;
            }
        }

        /// <summary>
        /// Gets the ocr folder.
        /// </summary>
        public static string OcrFolder
        {
            get
            {
                return DataDirectory + "Ocr" + Path.DirectorySeparatorChar;
            }
        }

        /// <summary>
        /// Gets the waveforms folder.
        /// </summary>
        public static string WaveformsFolder
        {
            get
            {
                return DataDirectory + "Waveforms" + Path.DirectorySeparatorChar;
            }
        }

        /// <summary>
        /// Gets the spectrograms folder.
        /// </summary>
        public static string SpectrogramsFolder
        {
            get
            {
                return DataDirectory + "Spectrograms" + Path.DirectorySeparatorChar;
            }
        }

        /// <summary>
        /// Gets the auto backup folder.
        /// </summary>
        public static string AutoBackupFolder
        {
            get
            {
                return DataDirectory + "AutoBackup" + Path.DirectorySeparatorChar;
            }
        }

        /// <summary>
        /// Gets the plugins directory.
        /// </summary>
        public static string PluginsDirectory
        {
            get
            {
                return Path.Combine(DataDirectory, "Plugins");
            }
        }

        /// <summary>
        /// Gets the data directory.
        /// </summary>
        public static string DataDirectory
        {
            get
            {
                return _instance.Value._dataDir;
            }
        }

        /// <summary>
        /// Gets the base directory.
        /// </summary>
        public static string BaseDirectory
        {
            get
            {
                return _instance.Value._baseDir;
            }
        }

        /// <summary>
        /// Gets the settings.
        /// </summary>
        public static Settings Settings
        {
            get
            {
                return _instance.Value._settings.Value;
            }
        }

        /// <summary>
        /// The is running on linux.
        /// </summary>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public static bool IsRunningOnLinux()
        {
            return Environment.OSVersion.Platform == PlatformID.Unix;
        }

        /// <summary>
        /// The is running on mac.
        /// </summary>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public static bool IsRunningOnMac()
        {
            return Environment.OSVersion.Platform == PlatformID.MacOSX;
        }

        /// <summary>
        /// The get installer path.
        /// </summary>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        private static string GetInstallerPath()
        {
            const string valueName = "InstallLocation";
            var value = RegistryUtil.GetValue(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\SubtitleEdit_is1", valueName);
            if (value != null && Directory.Exists(value))
            {
                return value;
            }

            value = RegistryUtil.GetValue(@"SOFTWARE\Wow6432Node\Microsoft\Windows\CurrentVersion\Uninstall\SubtitleEdit_is1", valueName);
            if (value != null && Directory.Exists(value))
            {
                return value;
            }

            return null;
        }

        /// <summary>
        /// The get base directory.
        /// </summary>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        private static string GetBaseDirectory()
        {
            var assembly = System.Reflection.Assembly.GetEntryAssembly();
            var baseDirectory = Path.GetDirectoryName(assembly == null ? System.Reflection.Assembly.GetExecutingAssembly().Location : assembly.Location);

            return baseDirectory.EndsWith(Path.DirectorySeparatorChar) ? baseDirectory : baseDirectory + Path.DirectorySeparatorChar;
        }

        /// <summary>
        /// The get data directory.
        /// </summary>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        private string GetDataDirectory()
        {
            if (IsRunningOnLinux() || IsRunningOnMac())
            {
                return this._baseDir;
            }

            var installerPath = GetInstallerPath();
            var hasUninstallFiles = Directory.GetFiles(this._baseDir, "unins*.*").Length > 0;
            var hasDictionaryFolder = Directory.Exists(Path.Combine(this._baseDir, "Dictionaries"));
            var appDataRoamingPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Subtitle Edit");

            if ((installerPath == null || !installerPath.TrimEnd(Path.DirectorySeparatorChar).Equals(this._baseDir.TrimEnd(Path.DirectorySeparatorChar), StringComparison.OrdinalIgnoreCase)) && !hasUninstallFiles && (hasDictionaryFolder || !Directory.Exists(Path.Combine(appDataRoamingPath, "Dictionaries"))))
            {
                return this._baseDir;
            }

            if (Directory.Exists(appDataRoamingPath))
            {
                return appDataRoamingPath + Path.DirectorySeparatorChar;
            }

            try
            {
                Directory.CreateDirectory(appDataRoamingPath);
                Directory.CreateDirectory(Path.Combine(appDataRoamingPath, "Dictionaries"));
                return appDataRoamingPath + Path.DirectorySeparatorChar;
            }
            catch
            {
                System.Windows.Forms.MessageBox.Show("Please re-install Subtitle Edit (installer version)");
                System.Windows.Forms.Application.ExitThread();
                return this._baseDir;
            }
        }
    }
}