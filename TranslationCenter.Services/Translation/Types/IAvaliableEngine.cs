using System;
using TranslationCenter.Services.Translation.Enums;

namespace TranslationCenter.Services.Translation.Types
{
    public interface IAvaliableEngine
    {
        EngineCategory Category { get; }
        string DisplayName { get; }
        string Name { get; }
        Type Type { get; }
    }
}