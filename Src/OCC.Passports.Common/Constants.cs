namespace OCC.Passports.Common
{
    public class Constants
    {
        public class Passports
        {
            public const string KeyPassport = "Passport";
            public const string KeySession = "Session";
            public const string KeyTimestamp = "Timestamp";
            public const string KeyLevel = "Level";
            public const string KeyMessage = "Message";
            public const string KeyEMail = "EMail";

            public const string KeyCallContext = "Context";
            public const string KeyHttpRequest = "HttpRequest";
            public const string KeyScopes=  "Scopes";

            public const string KeyCallerMemberName = "CallerMemberName";
            public const string KeyCallerFilePath = "CallerFilePath";
            public const string KeyCallerLineNumber = "CallerLineNumber";

            public const string KeyError = "Error has occured";
            public const string KeyOnExit = "On Exit";
        }

        public class PassportLevel
        {
            public const string Debug = "Debug";
            public const string Info = "Info";
            public const string Warn = "Warn";
            public const string Error = "Error";
            public const string Exception = "Exception";            
        }

        public class PassportScope
        {
            public const string ReturnValue = "ReturnValue";
            public const string Parameters = "Parameters";
            public const string Enter = "Entry";
            public const string Exit = "Exit";
            public class Entry
            {
                public const string Scope = "Scope";
                public const string Timestamp = "Timestamp";
                public const string Operation = "Operation";
            }
        }
    }
}
