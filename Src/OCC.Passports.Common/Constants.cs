namespace OCC.Passports.Common
{
    public class Constants
    {
        public const string KeyPassportTimestamp = "PassportTimestamp";

        public class Contexts
        {
            public const string Machine = "Machine";
            public const string Exception = "Exception";
        }

        public class Passports
        {
            public const string KeyPassport = "Passport";
            public const string KeySession = "Session";
            public const string KeyTimestamp = "Timestamp";
            public const string KeyLevel = "Level";
            public const string KeyMessage = "Message";
            public const string KeyUser = "User";

            public const string KeyCallContext = "Context";
            public const string KeyHttpRequest = "HttpRequest";
            public const string KeyScopes =  "Scope";
            
            public const string KeyScopesShort = "5";

            public const string KeyCallerMemberName = "CallerMemberName";
            public const string KeyCallerFilePath = "CallerFilePath";
            public const string KeyCallerLineNumber = "CallerLineNumber";

            public const string KeyError = "Error has occured";
            public const string KeyOnExit = "On Exit";

            public const string KeyCurrentCallContext = "CurrentCallContext";
            public const string KeyParentCallContext = "ParentCallContext";

            public const string KeyEndOfRequest = "End of request ";
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
                public const string Name = "Name";
                public const string Timestamp = "Timestamp";
                public const string Operation = "Operation";
                public const string History = "History";
            }
        }
    }
}
