using System.Collections.Generic;
using System.Linq;

namespace OCC.Passports.Common.Web
{
    public class PassportWebSettings : PassportSettings
    {
        private static readonly List<string> _ignoredFormFieldNames = new List<string>();
        private static readonly List<string> _ignoreHeaderNames = new List<string>();
        private static readonly List<string> _ignoreCookieNames = new List<string>();
        private static readonly List<string> _ignoreServerVariableNames = new List<string>();
        private static readonly List<int> _excludedStatusCodes = new List<int>();

        static PassportWebSettings()
        {
            var settings = new PassportSettings();

            if (!string.IsNullOrEmpty(settings.IgnoreFormFieldNames))
            {
                var ignoredNames = settings.IgnoreFormFieldNames.Split(',')
                                            .Where(s => !string.IsNullOrWhiteSpace(s))
                                            .Select(s => s.ToLowerInvariant());

                _ignoredFormFieldNames.AddRange(ignoredNames);
            }
            if (!string.IsNullOrEmpty(settings.IgnoreHeaderNames))
            {
                var ignoredNames = settings.IgnoreHeaderNames.Split(',')
                                            .Where(s => !string.IsNullOrWhiteSpace(s))
                                            .Select(s => s.ToLowerInvariant());
                _ignoreHeaderNames.AddRange(ignoredNames);
            }
            if (!string.IsNullOrEmpty(settings.IgnoreCookieNames))
            {
                var ignoredNames = settings.IgnoreCookieNames.Split(',')
                                            .Where(s => !string.IsNullOrWhiteSpace(s))
                                            .Select(s => s.ToLowerInvariant());
                _ignoreCookieNames.AddRange(ignoredNames);
            }

            if (!string.IsNullOrEmpty(settings.IgnoreServerVariableNames))
            {
                var ignoredNames = settings.IgnoreServerVariableNames.Split(',')
                                            .Where(s => !string.IsNullOrWhiteSpace(s))
                                            .Select(s => s.ToLowerInvariant());
                _ignoreServerVariableNames.AddRange(ignoredNames);
            }

            _excludedStatusCodes.AddRange(settings.ExcludedStatusCodes);
        }
        public static bool IsFormFieldIgnored(string name)
        {
            return IsIgnored(name, _ignoredFormFieldNames);
        }

        public static bool IsHeaderIgnored(string name)
        {
            return IsIgnored(name, _ignoreHeaderNames);
        }

        public static bool IsCookieIgnored(string name)
        {
            return IsIgnored(name, _ignoreCookieNames);
        }

        public static bool IsServerVariableIgnored(string name)
        {
            return IsIgnored(name, _ignoreServerVariableNames);
        }

        public static bool IsStatusCodeIgnored(int code)
        {
            return _excludedStatusCodes.Contains(code);
        }

        private static bool IsIgnored(string name, IReadOnlyList<string> list)
        {
            if (name == null || (list.Count == 1 && "*".Equals(list[0])))
            {
                return true;
            }

            foreach (var ignore in list)
            {
                var lowerName = name.ToLower();
                if (ignore.StartsWith("*"))
                {
                    if (ignore.EndsWith("*"))
                    {
                        if (lowerName.Contains(ignore.Substring(1, ignore.Length - 2)))
                        {
                            return true;
                        }
                    }
                    else
                    {
                        if (lowerName.EndsWith(ignore.Substring(1)))
                        {
                            return true;
                        }
                    }
                }
                else if (ignore.EndsWith("*") && lowerName.StartsWith(ignore.Substring(0, ignore.Length - 1)))
                {
                    return true;
                }
                else if (lowerName.Equals(ignore))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
