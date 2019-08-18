using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using TranslationCenter.Services.Translation.Enums;

namespace TranslationCenter.Services.Translation.Types
{
    public class AvaliableEngine
    {
        private TypeInfo i;

        public AvaliableEngine(TypeInfo typeInfo)
        {
            Category = typeInfo.GetCustomAttributes(false).OfType<EngineInfoAttribute>().FirstOrDefault().Category;
            Type = typeInfo.UnderlyingSystemType;
        }

        public EngineCategory Category { get; }
        public Type Type { get; }
        public string Name => Type.Name;

    }
}
