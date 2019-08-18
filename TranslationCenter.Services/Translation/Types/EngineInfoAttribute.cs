using System;
using TranslationCenter.Services.Translation.Enums;

namespace TranslationCenter.Services.Translation.Types
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class EngineInfoAttribute : Attribute
    {
        public EngineInfoAttribute(EngineCategory engineType, string displayName)
        {
            Category = engineType;
            DisplayName = displayName;
        }

        public string DisplayName { get; }
        public EngineCategory Category { get; }
    }
}