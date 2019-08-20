using System;
using System.Linq;
using System.Reflection;
using TranslationCenter.Services.Translation.Enums;

namespace TranslationCenter.Services.Translation.Types
{
    public class AvaliableEngine
    {
         public AvaliableEngine(TypeInfo typeInfo)
        {
            var engineInfo = typeInfo.GetCustomAttributes(false).OfType<EngineInfoAttribute>().FirstOrDefault();
            Category = engineInfo.Category;
            DisplayName = engineInfo.DisplayName;
            Type = typeInfo.UnderlyingSystemType;
        }

        public EngineCategory Category { get; }
        public string DisplayName { get; }
        public Type Type { get; }
        public string Name => Type.Name;
    }
}