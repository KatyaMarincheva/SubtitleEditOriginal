﻿namespace Nikse.SubtitleEdit.Logic.Dictionaries
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Xml;

    using Nikse.SubtitleEdit.Core;

    public class NamesList
    {
        private readonly string _dictionaryFolder;

        private readonly string _languageName;

        private readonly HashSet<string> _namesList;

        private readonly HashSet<string> _namesMultiList;

        public NamesList(string dictionaryFolder, string languageName, bool useOnlineNamesEtc, string namesEtcUrl)
        {
            this._dictionaryFolder = dictionaryFolder;
            this._languageName = languageName;

            this._namesList = new HashSet<string>();
            this._namesMultiList = new HashSet<string>();

            if (useOnlineNamesEtc && !string.IsNullOrEmpty(namesEtcUrl))
            {
                try
                {
                    string xml = Utilities.DownloadString(Configuration.Settings.WordLists.NamesEtcUrl);
                    XmlDocument namesDoc = new XmlDocument();
                    namesDoc.LoadXml(xml);
                    LoadNames(this._namesList, this._namesMultiList, namesDoc);
                }
                catch (Exception exception)
                {
                    LoadNamesList(Path.Combine(this._dictionaryFolder, "names_etc.xml"), this._namesList, this._namesMultiList);
                    Debug.WriteLine(exception.Message);
                }
            }
            else
            {
                LoadNamesList(Path.Combine(this._dictionaryFolder, "names_etc.xml"), this._namesList, this._namesMultiList);
            }

            LoadNamesList(this.GetLocalNamesFileName(), this._namesList, this._namesMultiList);

            string userFile = this.GetLocalNamesUserFileName();
            LoadNamesList(userFile, this._namesList, this._namesMultiList);
            this.UnloadRemovedNames(userFile);
        }

        public List<string> GetAllNames()
        {
            List<string> list = new List<string>();
            foreach (string name in this._namesList)
            {
                list.Add(name);
            }

            foreach (string name in this._namesMultiList)
            {
                list.Add(name);
            }

            return list;
        }

        public HashSet<string> GetNames()
        {
            return this._namesList;
        }

        public HashSet<string> GetMultiNames()
        {
            return this._namesMultiList;
        }

        public bool Remove(string name)
        {
            name = name.Trim();
            if (name.Length > 1 && this._namesList.Contains(name) || this._namesMultiList.Contains(name))
            {
                if (this._namesList.Contains(name))
                {
                    this._namesList.Remove(name);
                }

                if (this._namesMultiList.Contains(name))
                {
                    this._namesMultiList.Remove(name);
                }

                HashSet<string> userList = new HashSet<string>();
                string fileName = this.GetLocalNamesUserFileName();
                LoadNamesList(fileName, userList, userList);

                XmlDocument namesDoc = new XmlDocument();
                if (File.Exists(fileName))
                {
                    namesDoc.Load(fileName);
                }
                else
                {
                    namesDoc.LoadXml("<ignore_words />");
                }

                if (userList.Contains(name))
                {
                    userList.Remove(name);
                    XmlNode nodeToRemove = null;
                    foreach (XmlNode node in namesDoc.DocumentElement.SelectNodes("name"))
                    {
                        if (node.InnerText == name)
                        {
                            nodeToRemove = node;
                            break;
                        }
                    }

                    if (nodeToRemove != null)
                    {
                        namesDoc.DocumentElement.RemoveChild(nodeToRemove);
                    }
                }
                else
                {
                    XmlNode node = namesDoc.CreateElement("removed_name");
                    node.InnerText = name;
                    namesDoc.DocumentElement.AppendChild(node);
                }

                namesDoc.Save(fileName);
                return true;
            }

            return false;
        }

        public bool Add(string name)
        {
            name = name.Trim();
            if (name.Length > 1 && name.ContainsLetter())
            {
                if (name.Contains(' '))
                {
                    if (!this._namesMultiList.Contains(name))
                    {
                        this._namesMultiList.Add(name);
                    }
                }
                else if (!this._namesList.Contains(name))
                {
                    this._namesList.Add(name);
                }

                string fileName = this.GetLocalNamesUserFileName();
                XmlDocument namesEtcDoc = new XmlDocument();
                if (File.Exists(fileName))
                {
                    namesEtcDoc.Load(fileName);
                }
                else
                {
                    namesEtcDoc.LoadXml("<ignore_words />");
                }

                XmlNode de = namesEtcDoc.DocumentElement;
                if (de != null)
                {
                    XmlNode node = namesEtcDoc.CreateElement("name");
                    node.InnerText = name;
                    de.AppendChild(node);
                    namesEtcDoc.Save(fileName);
                }

                return true;
            }

            return false;
        }

        public bool IsInNamesEtcMultiWordList(string text, string word)
        {
            if (string.IsNullOrEmpty(text))
            {
                return false;
            }

            text = text.Replace(Environment.NewLine, " ");
            text = text.FixExtraSpaces();

            foreach (string s in this._namesMultiList)
            {
                if (s.Contains(word) && text.Contains(s))
                {
                    if (s.StartsWith(word + " ", StringComparison.Ordinal) || s.EndsWith(" " + word, StringComparison.Ordinal) || s.Contains(" " + word + " "))
                    {
                        return true;
                    }

                    if (word == s)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private static void LoadNamesList(string fileName, HashSet<string> namesList, HashSet<string> namesMultiList)
        {
            if (string.IsNullOrEmpty(fileName) || !File.Exists(fileName))
            {
                return;
            }

            XmlDocument namesDoc = new XmlDocument();
            namesDoc.Load(fileName);
            if (namesDoc.DocumentElement == null)
            {
                return;
            }

            LoadNames(namesList, namesMultiList, namesDoc);
        }

        private static void LoadNames(HashSet<string> namesList, HashSet<string> namesMultiList, XmlDocument namesDoc)
        {
            foreach (XmlNode node in namesDoc.DocumentElement.SelectNodes("name"))
            {
                string s = node.InnerText.Trim();
                if (s.Contains(' ') && !namesMultiList.Contains(s))
                {
                    namesMultiList.Add(s);
                }
                else if (!namesList.Contains(s))
                {
                    namesList.Add(s);
                }
            }
        }

        private void UnloadRemovedNames(string fileName)
        {
            if (string.IsNullOrEmpty(fileName) || !File.Exists(fileName))
            {
                return;
            }

            XmlDocument namesDoc = new XmlDocument();
            namesDoc.Load(fileName);
            if (namesDoc.DocumentElement == null)
            {
                return;
            }

            foreach (XmlNode node in namesDoc.DocumentElement.SelectNodes("removed_name"))
            {
                string s = node.InnerText.Trim();
                if (s.Contains(' '))
                {
                    if (this._namesMultiList.Contains(s))
                    {
                        this._namesMultiList.Remove(s);
                    }
                }
                else if (this._namesList.Contains(s))
                {
                    this._namesList.Remove(s);
                }
            }
        }

        private string GetLocalNamesFileName()
        {
            if (this._languageName.Length == 2)
            {
                string[] files = Directory.GetFiles(this._dictionaryFolder, this._languageName + "_??_names_etc.xml");
                if (files.Length > 0)
                {
                    return files[0];
                }
            }

            return Path.Combine(this._dictionaryFolder, this._languageName + "_names_etc.xml");
        }

        private string GetLocalNamesUserFileName()
        {
            if (this._languageName.Length == 2)
            {
                string[] files = Directory.GetFiles(this._dictionaryFolder, this._languageName + "_??_names_etc_user.xml");
                if (files.Length > 0)
                {
                    return files[0];
                }
            }

            return Path.Combine(this._dictionaryFolder, this._languageName + "_names_etc_user.xml");
        }
    }
}