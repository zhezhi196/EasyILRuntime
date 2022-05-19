using System;

namespace Module
{
    [AttributeUsage(AttributeTargets.Class)]
    public class SkillDescriptAttribute : Attribute
    {
        public string targetName;
        public string skillName;

        public SkillDescriptAttribute(string path)
        {
            string[] temp = path.Split('/');
            if (temp.Length == 1)
            {
                skillName = path;
            }
            else if (temp.Length == 2)
            {
                targetName = temp[0];
                skillName = temp[1];
            }
        }
    }
}