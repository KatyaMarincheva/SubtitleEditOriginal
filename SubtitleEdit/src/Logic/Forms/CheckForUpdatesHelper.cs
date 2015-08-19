﻿namespace Nikse.SubtitleEdit.Logic.Forms
{
    using System;
    using System.IO;
    using System.Net;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Threading;

    using Nikse.SubtitleEdit.Core;

    public class CheckForUpdatesHelper
    {
        // private const string ReleasesUrl = "https://api.github.com/repos/SubtitleEdit/subtitleedit/releases";
        private const string ChangeLogUrl = "https://raw.githubusercontent.com/SubtitleEdit/subtitleedit/master/Changelog.txt";

        private static readonly Regex regex = new Regex(@"\d\.\d", RegexOptions.Compiled); // 3.4.0 (xth June 2014)

        // private string _jsonReleases;
        private string _changeLog;

        private int _successCount;

        public CheckForUpdatesHelper()
        {
            this.Error = null;
            this._successCount = 0;
        }

        public string Error { get; set; }

        public bool Done
        {
            get
            {
                return this._successCount == 1;
            }
        }

        public string LatestVersionNumber { get; set; }

        public string LatestChangeLog { get; set; }

        public void CheckForUpdates()
        {
            // load github release json
            // StartDownloadString(ReleasesUrl, "application/json", FinishWebRequestReleases);

            // load change log
            this.StartDownloadString(ChangeLogUrl, null, this.FinishWebRequestChangeLog);
        }

        public bool IsUpdateAvailable()
        {
            try
            {
                // string[] currentVersionInfo = "3.3.14".Split('.'); // for testing...
                string[] currentVersionInfo = Utilities.AssemblyVersion.Split('.');
                string minorMinorVersion = string.Empty;
                if (currentVersionInfo.Length >= 3 && currentVersionInfo[2] != "0")
                {
                    minorMinorVersion = "." + currentVersionInfo[2];
                }

                string currentVersion = string.Format("{0}.{1}{2}", currentVersionInfo[0], currentVersionInfo[1], minorMinorVersion);
                if (currentVersion == this.LatestVersionNumber)
                {
                    return false;
                }

                string[] latestVersionInfo = this.LatestVersionNumber.Split('.');
                if (int.Parse(latestVersionInfo[0]) > int.Parse(currentVersionInfo[0]))
                {
                    return true;
                }

                if (int.Parse(latestVersionInfo[0]) == int.Parse(currentVersionInfo[0]) && int.Parse(latestVersionInfo[1]) > int.Parse(currentVersionInfo[1]))
                {
                    return true;
                }

                if (int.Parse(latestVersionInfo[0]) == int.Parse(currentVersionInfo[0]) && int.Parse(latestVersionInfo[1]) == int.Parse(currentVersionInfo[1]) && int.Parse(latestVersionInfo[2]) > int.Parse(currentVersionInfo[2]))
                {
                    return true;
                }

                return false;
            }
            catch
            {
                return false;
            }
        }

        private static string GetLastestVersionNumber(string latestChangeLog)
        {
            foreach (string line in latestChangeLog.Replace(Environment.NewLine, "\n").Split('\n'))
            {
                string s = line.Trim();
                if (!s.Contains("BETA", StringComparison.OrdinalIgnoreCase) && !s.Contains('x') && !s.Contains('*') && s.Contains('(') && s.Contains(')') && regex.IsMatch(s))
                {
                    int indexOfSpace = s.IndexOf(' ');
                    if (indexOfSpace > 0)
                    {
                        return s.Substring(0, indexOfSpace).Trim();
                    }
                }
            }

            return null;
        }

        private static string GetLastestChangeLog(string changeLog)
        {
            bool releaseOn = false;
            StringBuilder sb = new StringBuilder();
            foreach (string line in changeLog.Replace(Environment.NewLine, "\n").Split('\n'))
            {
                string s = line.Trim();
                if (s.Length == 0 && releaseOn)
                {
                    return sb.ToString();
                }

                if (!releaseOn)
                {
                    if (!s.Contains('x') && !s.Contains('*') && s.Contains('(') && s.Contains(')') && regex.IsMatch(s))
                    {
                        releaseOn = true;
                    }
                }

                if (releaseOn)
                {
                    sb.AppendLine(line);
                }
            }

            return sb.ToString();
        }

        private void StartDownloadString(string url, string contentType, AsyncCallback callback)
        {
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.UserAgent = "SubtitleEdit";
                request.ContentType = contentType;
                request.Timeout = Timeout.Infinite;
                request.Method = "GET";
                request.AllowAutoRedirect = true;
                request.Accept = contentType;
                request.BeginGetResponse(callback, request);
            }
            catch (Exception exception)
            {
                if (this.Error == null)
                {
                    this.Error = exception.Message;
                }
            }
        }

        // void FinishWebRequestReleases(IAsyncResult result)
        // {
        // try
        // {
        // _jsonReleases = GetStringFromResponse(result);
        // }
        // catch (Exception exception)
        // {
        // if (Error == null)
        // {
        // Error = exception.Message;
        // }
        // }
        // }
        private void FinishWebRequestChangeLog(IAsyncResult result)
        {
            try
            {
                this._changeLog = this.GetStringFromResponse(result);
                this.LatestChangeLog = GetLastestChangeLog(this._changeLog);
                this.LatestVersionNumber = GetLastestVersionNumber(this.LatestChangeLog);
            }
            catch (Exception exception)
            {
                if (this.Error == null)
                {
                    this.Error = exception.Message;
                }
            }
        }

        private string GetStringFromResponse(IAsyncResult result)
        {
            HttpWebResponse response = (result.AsyncState as HttpWebRequest).EndGetResponse(result) as HttpWebResponse;
            Stream responseStream = response.GetResponseStream();
            byte[] buffer = new byte[5000000];
            int count = 1;
            int index = 0;
            while (count > 0)
            {
                count = responseStream.Read(buffer, index, 2048);
                index += count;
            }

            if (index > 0)
            {
                this._successCount++;
            }

            return Encoding.UTF8.GetString(buffer, 0, index);
        }
    }
}