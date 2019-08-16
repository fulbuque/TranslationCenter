﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace TranslationCenter.Services.Translation.Extensions
{
    public static class XElementExtensions
    {
        public static XElement GetElement(this XElement element, string attributeName, string attributeValue)
        {
            return element.Elements().Where(e => e.Attribute(attributeName)?.Value?.Equals(attributeValue, StringComparison.OrdinalIgnoreCase) ?? false).FirstOrDefault();
        }
    }
}
