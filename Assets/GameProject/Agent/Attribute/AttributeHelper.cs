using System;
using System.Collections.Generic;
using System.Reflection;
using Module;
using Project.Data;

public class AttributeInfo: ITextObject
{
    public bool isGrowUp;
    public AttributeData data;
    public FloatField value;

    public AttributeInfo(string name, FloatField value,bool isGrowUp)
    {
        data = DataInit.Instance.GetSqlService<AttributeData>().Where(fd => fd.field == name);
        this.value = value;
        this.isGrowUp = isGrowUp;
    }


    public string GetText(string type)
    {
        if (isGrowUp)
        {
            return string.Format(Language.GetContent(data.des), (value.value * 100) + "%");
        }
        else
        {
            return string.Format(Language.GetContent(data.des), value.value);
        }
    }
}
public static class AttributeHelper
{
    #region 获取类型

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

    public static List<AttributeInfo> GetAttributeInfo(GameAttribute attribute,int type)
    {
        List<AttributeInfo> result = new List<AttributeInfo>();

        Type attType = typeof(GameAttribute);
        PropertyInfo[] property = attType.GetProperties();
        for (int i = 0; i < property.Length; i++)
        {
            var attValue = (FloatField) property[i].GetValue(attribute);
            if (attValue.value > 0)
            {
                AttributeInfo info = new AttributeInfo(property[i].Name, attValue, type != 0);
                result.Add(info);
            }
        }

        return result;
    }

}