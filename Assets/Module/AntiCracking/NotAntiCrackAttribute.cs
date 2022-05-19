using System;
using Sirenix.OdinInspector;

namespace Module
{
    [AttributeUsage(AttributeTargets.Property| AttributeTargets.Field)]
    public class NotAntiCrackAttribute: Attribute
    {
    }
}