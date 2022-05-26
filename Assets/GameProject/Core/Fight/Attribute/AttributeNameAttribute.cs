using System;

[AttributeUsage(AttributeTargets.Property| AttributeTargets.Field)]
public class AttributeNameAttribute: Attribute
{
    public string langKey;

    public AttributeNameAttribute(string key)
    {
        this.langKey = key;
    }
}