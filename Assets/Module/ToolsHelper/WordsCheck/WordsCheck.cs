using System;
using System.Collections.Generic;
using LitJson;
using UnityEngine;

namespace Module
{
    public static class WordsCheck
    {
        private enum WordCheckType
        {
            Dot,
            Json,
            Enter
        }

        private static string[] content;

        private const WordCheckType checkType = WordCheckType.Enter;
        private static bool isInit;
        private static Queue<Action<string[]>> cache = new Queue<Action<string[]>>();

        public static void CheckWord(string words, Action<bool> callback)
        {
            Init(ss =>
            {
                for (int i = 0; i < ss.Length; i++)
                {
                    if (words.Contains(ss[i]))
                    {
                        callback?.Invoke(false);
                        return;
                    }
                }

                callback?.Invoke(true);
            });
        }

        private static void Init(Action<string[]> words)
        {
            if (!isInit)
            {
                cache.Enqueue(words);
                if (cache.Count == 1)
                {
                    AssetLoad.PreloadAsset<TextAsset>($"{ConstKey.GetFolder(AssetLoad.AssetFolderType.Config)}/{ConstKey.GetFolder(AssetLoad.AssetFolderType.WordsCheck)}",
                        ass =>
                        {
                            if (ass.Result != null)
                            {
                                if (checkType == WordCheckType.Dot)
                                {
                                    content = ass.Result.text.Split(',');
                                }
                                else if (checkType == WordCheckType.Json)
                                {
                                    content = JsonMapper.ToObject<string[]>(ass.Result.text);
                                }
                                else if (checkType == WordCheckType.Enter)
                                {
                                    content = ass.Result.text.Split('\n');
                                }

                                int count = cache.Count;
                                for (int i = 0; i < count; i++)
                                {
                                    cache.Dequeue()?.Invoke(content);
                                }
                                isInit = true;
                            }
                        });
                }
            }
            else
            {
                words?.Invoke(content);
            }
        }
    }
}