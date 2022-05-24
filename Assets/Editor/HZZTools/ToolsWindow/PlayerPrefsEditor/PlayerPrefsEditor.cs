using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;
using PlistCS;

namespace EditorModule
{
    public class PlayerPrefPair
    {
        public string Key { get; set; }

        public string Value { get; set; }

        //0 int  1 float  2 string
        public int type;
    }

    public class PlayerPrefsEditor : EditorWindow
    {
        private readonly string[] options = {"Add", "Add Int", "Add Float", "Add String"};

        private readonly Dictionary<int, string> prefTypeDic = new Dictionary<int, string>
            {{0, "Int"}, {1, "Float"}, {2, "String"}};

        public List<PlayerPrefPair> prefs;
        private Vector3 scrollPos = Vector2.zero;
        private Dictionary<string, PlayerPrefPair> playerPrefsDict = new Dictionary<string, PlayerPrefPair>();

        private static PlayerPrefsEditor _editor;


        [MenuItem("Tools/PlayerPrefs Editor")]
        public static void ShowWindow()
        {
            if (_editor == null)
            {
                _editor = GetWindow<PlayerPrefsEditor>();
            }
            else
            {
                _editor.Close();
                _editor = null;
            }
        }

        private void OnEnable()
        {
            Refresh();
        }

        private void AddPlayerPrefs(int index)
        {
            if (index != 0)
            {
                PlayerPrefPair playerPrefPair = new PlayerPrefPair();
                playerPrefPair.Key = options[index];
                playerPrefPair.Value = "0";
                playerPrefPair.type = index - 1;
                prefs.Insert(0, playerPrefPair);
            }
        }

        private void OnGUI()
        {
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Save(Ctrl+S)", EditorStyles.toolbarButton))
            {
                SaveUpdatePrefs();
            }

            if (GUILayout.Button("FormatAll", EditorStyles.toolbarButton))
            {
                for (int i = 0; i < prefs.Count; i++)
                {
                    prefs[i].Value = JsonFormatter.FromatOrCompress(prefs[i].Value);
                }
            }

            if (GUILayout.Button("Clear All", EditorStyles.toolbarButton))
            {
                if (EditorUtility.DisplayDialog("Clear", "Are you clear all PlayerPrefs info?", "confirm", "cancel"))
                {
                    PlayerPrefs.DeleteAll();
                    PlayerPrefs.Save();
                    Refresh();
                }
            }

            if (GUILayout.Button("Refresh", EditorStyles.toolbarButton))
            {
                Refresh();
            }

            int idx = EditorGUILayout.Popup(0, options, EditorStyles.toolbarDropDown, GUILayout.Width(50));
            AddPlayerPrefs(idx);

            GUILayout.EndHorizontal();

            scrollPos = EditorGUILayout.BeginScrollView(scrollPos);

            GUILayout.BeginVertical();
            for (int i = 0; i < prefs.Count; i++)
            {
                DrawItem(prefs[i]);
            }

            GUILayout.EndVertical();
            GUILayout.Space(10);
            EditorGUILayout.EndScrollView();
            if (Event.current.modifiers.Equals(Event.KeyboardEvent("^S").modifiers) &&
                Event.current.keyCode == Event.KeyboardEvent("^S").keyCode)
            {
                SaveUpdatePrefs();
            }
        }

        private void Refresh()
        {
            prefs = PlayerPrefsExtension.GetAll().ToList();
            for (int i = 0; i < prefs.Count; i++)
            {
                if (prefs[i].Key.ToLower().StartsWith("unity"))
                {
                    prefs.RemoveAt(i);
                    i--;
                }
            }

            prefs.Sort((x, y) => string.Compare(x.Key, y.Key, StringComparison.Ordinal));
        }

        private void SaveUpdatePrefs()
        {
            try
            {
                foreach (var playerPrefPair in playerPrefsDict)
                {
                    Debug.Log("Save:" + playerPrefPair.Key);
                    SetPlayerPrefs(playerPrefPair.Value);
                }

                playerPrefsDict.Clear();
                Refresh();
            }
            catch (Exception e)
            {
                EditorUtility.DisplayDialog("Error", "Type error:" + e, "confirm");
            }
        }

        private void AddUpdatePrefs(string key, PlayerPrefPair playerPrefPair)
        {
            if (playerPrefsDict.ContainsKey(key))
            {
                playerPrefsDict[key] = playerPrefPair;
            }
            else
            {
                playerPrefsDict.Add(key, playerPrefPair);
            }
        }

        private void DrawItem(PlayerPrefPair pref)
        {
            GUILayout.BeginHorizontal();
            string keyTemp = GUILayout.TextArea(pref.Key, GUILayout.Width(200));
            if (!string.Equals(keyTemp, pref.Key))
            {
                pref.Key = keyTemp;
                AddUpdatePrefs(keyTemp, pref);
            }

            string valTemp = GUILayout.TextArea(pref.Value);
            if (!string.Equals(valTemp, pref.Value))
            {
                pref.Value = valTemp;
                AddUpdatePrefs(keyTemp, pref);
            }

            GUIStyle style = new GUIStyle();
            style.normal.textColor = Color.green;
            style.fixedWidth = 40;
            GUILayout.Label(prefTypeDic[pref.type], style);
            if (GUILayout.Button("Delete", GUILayout.Width(50)))
            {
                if (EditorUtility.DisplayDialog("Delete", $"Do you want to delete【{pref.Key}】 ?", "confirm", "cancel"))
                {
                    prefs.Remove(pref);
                    PlayerPrefs.DeleteKey(pref.Key);
                    PlayerPrefs.Save();
                }
            }

            if (GUILayout.Button("Copy", GUILayout.Width(50)))
            {
                GUIUtility.systemCopyBuffer = pref.Value;
            }

            if (GUILayout.Button("Format", GUILayout.Width(50)))
            {
                pref.Value = JsonFormatter.FromatOrCompress(pref.Value);
            }

            GUILayout.EndHorizontal();
        }

        private void SetPlayerPrefs(PlayerPrefPair playerPrefs)
        {
            if (playerPrefs.type == 0)
            {
                PlayerPrefs.SetInt(playerPrefs.Key, Convert.ToInt32(playerPrefs.Value));
            }
            else if (playerPrefs.type == 1)
            {
                PlayerPrefs.SetFloat(playerPrefs.Key, Convert.ToSingle(playerPrefs.Value));
            }
            else
            {
                PlayerPrefs.SetString(playerPrefs.Key, playerPrefs.Value);
            }

            PlayerPrefs.Save();
        }
    }


    public static class PlayerPrefsExtension
    {
        public static PlayerPrefPair[] GetAll()
        {
            return GetAll(PlayerSettings.companyName, PlayerSettings.productName);
        }

        public static PlayerPrefPair[] GetAll(string companyName, string productName)
        {
            if (Application.platform == RuntimePlatform.OSXEditor)
            {
                // From Unity docs: On Mac OS X PlayerPrefs are stored in ~/Library/Preferences folder, in a file named unity.[company name].[product name].plist, where company and product names are the names set up in Project Settings. The same .plist file is used for both Projects run in the Editor and standalone players.

                // Construct the plist filename from the project's settings
                string plistFilename = string.Format("unity.{0}.{1}.plist", companyName, productName);
                // Now construct the fully qualified path
                string playerPrefsPath = Path.Combine(
                    Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "Library/Preferences"),
                    plistFilename);

                // Parse the player prefs file if it exists
                if (File.Exists(playerPrefsPath))
                {
                    // Parse the plist then cast it to a Dictionary
                    object plist = Plist.readPlist(playerPrefsPath);

                    Dictionary<string, object> parsed = plist as Dictionary<string, object>;

                    // Convert the dictionary data into an array of PlayerPrefPairs
                    PlayerPrefPair[] tempPlayerPrefs = new PlayerPrefPair[parsed.Count];
                    int i = 0;
                    foreach (KeyValuePair<string, object> pair in parsed)
                    {
                        int tempType = 0;
                        if (pair.Value.GetType() == typeof(double))
                        {
                            // Some float values may come back as double, so convert them back to floats
                            // tempPlayerPrefs[i] = new PlayerPrefPair() { Key = pair.Key, Value = (float) (double) pair.Value };
                            tempType = 1;
                        }
                        else if (pair.Value.GetType() == typeof(byte[]))
                        {
                            tempType = 2;
                            //tempPlayerPrefs[i] = new PlayerPrefPair() { Key = pair.Key, Value = pair.Value.ToString() };
                        }

                        tempPlayerPrefs[i] = new PlayerPrefPair()
                            {Key = pair.Key, Value = pair.Value.ToString(), type = tempType};

                        i++;
                    }

                    // Return the results
                    return tempPlayerPrefs;
                }
                else
                {
                    // No existing player prefs saved (which is valid), so just return an empty array
                    return new PlayerPrefPair[0];
                }
            }
            else if (Application.platform == RuntimePlatform.WindowsEditor)
            {
                // From Unity docs: On Windows, PlayerPrefs are stored in the registry under HKCU\Software\[company name]\[product name] key, where company and product names are the names set up in Project Settings.
#if UNITY_5_5_OR_NEWER
                // From Unity 5.5 editor player prefs moved to a specific location
                Microsoft.Win32.RegistryKey registryKey =
                    Microsoft.Win32.Registry.CurrentUser.OpenSubKey(
                        "Software\\Unity\\UnityEditor\\" + companyName + "\\" + productName);
#else
                Microsoft.Win32.RegistryKey registryKey =
 Microsoft.Win32.Registry.CurrentUser.OpenSubKey("Software\\" + companyName + "\\" + productName);
#endif

                // Parse the registry if the specified registryKey exists
                if (registryKey != null)
                {
                    // Get an array of what keys (registry value names) are stored
                    string[] valueNames = registryKey.GetValueNames();

                    // Create the array of the right size to take the saved player prefs
                    PlayerPrefPair[] tempPlayerPrefs = new PlayerPrefPair[valueNames.Length];

                    // Parse and convert the registry saved player prefs into our array
                    int i = 0;
                    foreach (string valueName in valueNames)
                    {
                        string key = valueName;

                        // Remove the _h193410979 style suffix used on player pref keys in Windows registry
                        int index = key.LastIndexOf("_");
                        key = key.Remove(index, key.Length - index);

                        // Get the value from the registry
                        object ambiguousValue = registryKey.GetValue(valueName);
                        int tempType = 0;

                        // Unfortunately floats will come back as an int (at least on 64 bit) because the float is stored as
                        // 64 bit but marked as 32 bit - which confuses the GetValue() method greatly! 
                        if (ambiguousValue.GetType() == typeof(int))
                        {
                            // If the player pref is not actually an int then it must be a float, this will evaluate to true
                            // (impossible for it to be 0 and -1 at the same time)
                            if (PlayerPrefs.GetInt(key, -1) == -1 && PlayerPrefs.GetInt(key, 0) == 0)
                            {
                                // Fetch the float value from PlayerPrefs in memory
                                ambiguousValue = PlayerPrefs.GetFloat(key);
                                tempType = 1;
                            }
                        }
                        else if (ambiguousValue.GetType() == typeof(byte[]))
                        {
                            // On Unity 5 a string may be stored as binary, so convert it back to a string
                            ambiguousValue = System.Text.Encoding.Default.GetString((byte[]) ambiguousValue);
                            tempType = 2;
                        }

                        // Assign the key and value into the respective record in our output array
                        tempPlayerPrefs[i] = new PlayerPrefPair()
                            {Key = key, Value = ambiguousValue.ToString(), type = tempType};
                        i++;
                    }

                    // Return the results
                    return tempPlayerPrefs;
                }
                else
                {
                    // No existing player prefs saved (which is valid), so just return an empty array
                    return new PlayerPrefPair[0];
                }
            }
            else
            {
                throw new NotSupportedException("PlayerPrefsEditor doesn't support this Unity Editor platform");
            }
        }
    }


    public class JsonFormatter
    {
        private const string INDENT_STRING = "    ";

        public static string FormatJson(string str)
        {
            var indent = 0;
            var quoted = false;
            var sb = new StringBuilder();
            for (var i = 0; i < str.Length; i++)
            {
                var ch = str[i];
                switch (ch)
                {
                    case '{':
                    case '[':
                        sb.Append(ch);
                        if (!quoted)
                        {
                            sb.AppendLine();
                            foreach (var item in Enumerable.Range(0, ++indent))
                            {
                                sb.Append(INDENT_STRING);
                            }
                        }

                        break;
                    case '}':
                    case ']':
                        if (!quoted)
                        {
                            sb.AppendLine();
                            foreach (var item in Enumerable.Range(0, --indent))
                            {
                                sb.Append(INDENT_STRING);
                            }
                        }

                        sb.Append(ch);
                        break;
                    case '"':
                        sb.Append(ch);
                        bool escaped = false;
                        var index = i;
                        while (index > 0 && str[--index] == '\\')
                        {
                            escaped = !escaped;
                        }

                        if (!escaped)
                        {
                            quoted = !quoted;
                        }

                        break;
                    case ',':
                        sb.Append(ch);
                        if (!quoted)
                        {
                            sb.AppendLine();
                            foreach (var item in Enumerable.Range(0, indent))
                            {
                                sb.Append(INDENT_STRING);
                            }
                        }

                        break;
                    case ':':
                        sb.Append(ch);
                        if (!quoted)
                        {
                            sb.Append(" ");
                        }

                        break;
                    default:
                        sb.Append(ch);
                        break;
                }
            }

            return sb.ToString();
        }

        public static string CompressJson(string json)
        {
            StringBuilder sb = new StringBuilder();
            using (StringReader reader = new StringReader(json))
            {
                int ch = -1;
                int lastch = -1;
                bool isQuoteStart = false;
                while ((ch = reader.Read()) > -1)
                {
                    if ((char) lastch != '\\' && (char) ch == '\"')
                    {
                        if (!isQuoteStart)
                        {
                            isQuoteStart = true;
                        }
                        else
                        {
                            isQuoteStart = false;
                        }
                    }

                    if (!Char.IsWhiteSpace((char) ch) || isQuoteStart)
                    {
                        sb.Append((char) ch);
                    }

                    lastch = ch;
                }
            }

            return sb.ToString();
        }

        public static string FromatOrCompress(string json)
        {
            if (json.Contains("\n"))
            {
                return CompressJson(json);
            }

            return FormatJson(json);
        }
    }
}