using System.Configuration;
using System.Linq;

namespace OCC.Passports.Common
{
    public class PassportSettings : ConfigurationSection
    {
        private static readonly PassportSettings settings = ConfigurationManager.GetSection("PassportSettings") as PassportSettings ?? new PassportSettings();
        public static PassportSettings Settings
        {
            get { return settings; }
        }

        public override bool IsReadOnly()
        {
            return false;
        }

        [ConfigurationProperty("throwOnError", IsRequired = false, DefaultValue = false)]
        public bool ThrowOnError
        {
            get { return (bool)this["throwOnError"]; }
            set { this["throwOnError"] = value; }
        }

        [ConfigurationProperty("excludeHttpStatusCodes", IsRequired = false, DefaultValue = "")]
        [RegexStringValidator(@"^(\d+(,\s?\d+)*)?$")]
        public string ExcludeHttpStatusCodesList
        {
            get { return (string)this["excludeHttpStatusCodes"]; }
            set { this["excludeHttpStatusCodes"] = value; }
        }

        public int[] ExcludedStatusCodes
        {
            get { return string.IsNullOrEmpty(ExcludeHttpStatusCodesList) ? new int[0] : ExcludeHttpStatusCodesList.Split(',').Select(int.Parse).ToArray(); }
        }

        [ConfigurationProperty("excludeHttpStatusCodes", IsRequired = false, DefaultValue = "")]
        public string ExcludeExceptionsList
        {
            get { return (string)this["excludeHttpStatusCodes"]; }
            set { this["excludeHttpStatusCodes"] = value; }
        }

        public string[] ExcludedExceptions
        {
            get { return string.IsNullOrEmpty(ExcludeExceptionsList) ? new string[0] : ExcludeExceptionsList.Split(',').ToArray(); }
        }

        [ConfigurationProperty("ignoreFormFieldNames", IsRequired = false, DefaultValue = "")]
        public string IgnoreFormFieldNames
        {
            get { return (string)this["ignoreFormFieldNames"]; }
            set { this["ignoreFormFieldNames"] = value; }
        }

        [ConfigurationProperty("ignoreHeaderNames", IsRequired = false, DefaultValue = "")]
        public string IgnoreHeaderNames
        {
            get { return (string)this["ignoreHeaderNames"]; }
            set { this["ignoreHeaderNames"] = value; }
        }

        [ConfigurationProperty("ignoreCookieNames", IsRequired = false, DefaultValue = "")]
        public string IgnoreCookieNames
        {
            get { return (string)this["ignoreCookieNames"]; }
            set { this["ignoreCookieNames"] = value; }
        }

        [ConfigurationProperty("ignoreServerVariableNames", IsRequired = false, DefaultValue = "")]
        public string IgnoreServerVariableNames
        {
            get { return (string)this["ignoreServerVariableNames"]; }
            set { this["ignoreServerVariableNames"] = value; }
        }
    }
}
