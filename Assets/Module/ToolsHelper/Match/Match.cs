using System.Collections.Generic;

namespace Module
{
    public class Match
    {
        #region Static

        private static Dictionary<object, Match> matchDic = new Dictionary<object, Match>();

        public static Match GetMatch(object key)
        {
            Match match = null;
            if (!matchDic.TryGetValue(key, out match))
            {
                match = new Match(key);
                matchDic.Add(key, match);
            }
            
            return match;
        }
        
        public static Match GetMatch(object key,object tag)
        {
            Match match = null;
            if (!matchDic.TryGetValue(key, out match))
            {
                match = new Match(key,tag);
                matchDic.Add(key, match);
            }
            
            return match;
        }

        public static void ClearAll()
        {
            foreach (KeyValuePair<object,Match> keyValuePair in matchDic)
            {
                keyValuePair.Value.ClearStore();
            }
            
            matchDic.Clear();
        }
        
        public static void ClearTag(object tag)
        {
            Dictionary<object, Match> res = new Dictionary<object, Match>(matchDic);
            
            foreach (KeyValuePair<object,Match> keyValuePair in res)
            {
                if (keyValuePair.Value.tag == tag)
                {
                    keyValuePair.Value.ClearStore();
                    matchDic.Remove(keyValuePair.Key);
                }
            }
        }
        
        public static void CleaMatch(object key)
        {
            matchDic.Remove(key);
        }

        #endregion

        private List<IMatch> matchStore;
        public object matchKey { get; }
        public object tag { get; }

        private Match(object key)
        {
            this.matchKey = key;
        }
        
        private Match(object key,object tag)
        {
            this.matchKey = key;
            this.tag = tag;
        }

        public bool CanMatch(params IMatch[] matchs)
        {
            bool temp = true;
            for (int i = 0; i < matchs.Length; i++)
            {
                temp = temp && matchStore.Contains(matchs[i]);
                if (!temp) return false;
            }

            return true;
        }

        public void TryAddToStore(IMatch target)
        {
            if (matchStore == null)
            {
                matchStore = new List<IMatch>();
            }
            else
            {
                for (int i = 0; i < matchStore.Count; i++)
                {
                    matchStore[i].OnMatchSuccess(new[] {target});
                }
                
                target.OnMatchSuccess(matchStore.ToArray());
            }
            
            matchStore.Add(target);
        }

        public void ClearStore()
        {
            matchStore.Clear();
        }

        public void RemoveMatch(IMatch match)
        {
            for (int i = 0; i < matchStore.Count; i++)
            {
                if (matchStore[i] == match)
                {
                    matchStore.RemoveAt(i);
                }
            }

            if (matchStore.Count == 0) matchDic.Remove(matchKey);
        }

        public override string ToString()
        {
            return matchKey.ToString();
        }

    }
}