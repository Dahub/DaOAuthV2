using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace DaOAuthV2.Service.Test.Fake
{
    public class FakeStringLocalizerFactory : IStringLocalizerFactory
    {
        public IStringLocalizer Create(Type resourceSource)
        {
            return new FakeStringLocalizer();
        }

        public IStringLocalizer Create(string baseName, string location)
        {
            return new FakeStringLocalizer();
        }
    }

    public class FakeStringLocalizer : IStringLocalizer
    {
        public LocalizedString this[string name]
        {
            get
            {
                return new LocalizedString("fake", name);
            }
        }

        public LocalizedString this[string name, params object[] arguments]
        {
            get
            {
                return new LocalizedString("fake", name);
            }
        }

        public IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures)
        {
            return new List<LocalizedString>()
            {
                new LocalizedString("fake", "fake")
            };
        }

        public IStringLocalizer WithCulture(CultureInfo culture)
        {
            return new FakeStringLocalizer();
        }
    }
}
