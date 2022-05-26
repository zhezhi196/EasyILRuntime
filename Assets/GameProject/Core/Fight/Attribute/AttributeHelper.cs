using System.Reflection;
using Module;
using Project.Data;

public static class AttributeHelper
{
    #region 获取类型

    //2021年11月19日经潘宇要求,之后的天赋,武器,部件,属性都是按整条的成长属性或者整条的基础属性来做,如若再改,扫码
    public static GameAttribute[] GetAttributeByType(IAttribute<string> target)
    {
        GameAttribute[] result = new GameAttribute[2];
        object resut1 = new GameAttribute(0);
        object resut2 = new GameAttribute(0);
        
        var property = target.GetType().GetProperties();
        var type = typeof(GameAttribute);
        for (int i = 0; i < property.Length; i++)
        {
            var attProperty = type.GetProperty(property[i].Name);
            if (attProperty != null)
            {
                var propertyValue = property[i].GetValue(target).ToString();
                if (propertyValue.StartsWith("%"))
                {
                    attProperty.SetValue(resut2, new FloatField(propertyValue.Substring(1).ToFloat()));
                }
                else
                {
                    attProperty.SetValue(resut1, new FloatField(propertyValue.ToFloat()));
                }
            }
        }

        result[0] = (GameAttribute) resut1;
        result[1] = (GameAttribute) resut2;
        return result;
    }

    #endregion

    //获取GameAttribute字段
    public static System.Collections.Generic.List<string> GetAttFiled
    {
        get
        {
            System.Reflection.PropertyInfo[] property = typeof(GameAttribute).GetProperties();
            System.Collections.Generic.List<string> attFiled = new System.Collections.Generic.List<string>();
            for (int i = 0; i < property.Length; i++)
            {
                attFiled.Add(property[i].Name);
            }
            return attFiled;
        }
    }
}