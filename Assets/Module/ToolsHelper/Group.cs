using System.Collections.Generic;

namespace Module
{
    public class Group
    {
        private static List<Group> groupList = new List<Group>();

        public static Group GetGroup(string key, object tar)
        {
            for (int i = 0; i < groupList.Count; i++)
            {
                if (groupList[i].key == key)
                {
                    groupList[i].AddTar(tar);
                    return groupList[i];
                }
            }

            Group result = new Group(key);
            result.AddTar(tar);
            groupList.Add(result);
            return result;
        }
        
        
        public static void ChangeBattleScene()
        {
            for (int i = 0; i < groupList.Count; i++)
            {
                groupList[i].Dispose();
            }
            groupList.Clear();
        }
        
        private List<object> _group;
        
        public string key { get; }
        public object randomObject;
        private Group(string key)
        {
            this.key = key;
        }

        public void AddTar(object tar)
        {
            if (_group == null) _group = new List<object>();
            if (!_group.Contains(tar))
            {
                _group.Add(tar);
            }
        }

        public void Random()
        {
            if (randomObject == null)
            {
                randomObject = _group.Random();
            }
        }

        public void SetSetTar(object tar)
        {
            randomObject = tar;
        }

        public void Dispose()
        {
            randomObject = null;
            groupList.Clear();
        }

    }
}