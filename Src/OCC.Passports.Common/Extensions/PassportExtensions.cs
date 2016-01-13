using System.Collections.Generic;
using System.Dynamic;
using OCC.Passports.Common.Contracts.Infrastructure;
using OCC.Passports.Common.Domains;
using System;

namespace OCC.Passports.Common.Extensions
{
    public static class PassportExtensions
    {
        public static Passport ResolveAsPassport(this object o)
        {
            var instance = o as IHasPassport;
            if (instance == null) return null;

            var thisPassport = instance.Passport;
            if (thisPassport == null) return null;

            var concretePassport = thisPassport as Passport;
            return concretePassport;
        }

        private static dynamic MessageContext(IPassport passport
                                                , PassportLevel level
                                                , string message
                                                , string eMail
                                                , string memberName
                                                , string sourceFilePath
                                                , int sourceLineNumber
            )
        {
            dynamic context = new ExpandoObject();
            var messageContext = context as IDictionary<string, Object>;

            if (passport.SessionId != null)
                messageContext[Constants.Passports.KeySession] = passport.SessionId;

            messageContext[Constants.Passports.KeyPassport] = passport.PassportId;
            
            messageContext[Constants.Passports.KeyTimestamp] = new DateTimeOffset(DateTime.UtcNow);
            messageContext[Constants.Passports.KeyLevel] = level.Name;
            messageContext[Constants.Passports.KeyMessage] = message;

            if (!string.IsNullOrWhiteSpace(eMail))
                messageContext[Constants.Passports.KeyEMail] = eMail;

            messageContext[Constants.Passports.KeyCallerMemberName] = memberName;
            messageContext[Constants.Passports.KeyCallerFilePath] = sourceFilePath;
            messageContext[Constants.Passports.KeyCallerLineNumber] = sourceLineNumber;

            return messageContext;
        }

        public static void Trace(this IPassport self
                                    , string message
                                    , string eMail = ""
                                    , bool includeContext = false
                                    , bool includeScopes = false
                                    , [System.Runtime.CompilerServices.CallerMemberName] string memberName = ""
                                    , [System.Runtime.CompilerServices.CallerFilePath] string sourceFilePath = ""
                                    , [System.Runtime.CompilerServices.CallerLineNumber] int sourceLineNumber = 0
            )
        {
            dynamic messageContext = null;

            if (!string.IsNullOrWhiteSpace(PassportLevel.Session))
            {
                var currentSession = PassportLevel.Session.ToLowerInvariant();
                var passportSession = self.SessionId.ToString().ToLowerInvariant();

                if (currentSession.Equals(passportSession, StringComparison.InvariantCultureIgnoreCase))
                {
                    messageContext = MessageContext(self
                        , PassportLevel.Trace
                        , message
                        , eMail
                        , memberName
                        , sourceFilePath
                        , sourceLineNumber
                        );
                }
            }

            if (messageContext == null && PassportLevel.Trace >= PassportLevel.Current)
            {
                messageContext = MessageContext(self
                    , PassportLevel.Trace
                    , message
                    , eMail
                    , memberName
                    , sourceFilePath
                    , sourceLineNumber
                    );
            }

            if (messageContext != null)
            {
                self.Stamp(messageContext, includeContext, includeScopes);
            }
        }

        public static void Debug(this IPassport self
                                    , string message
                                    , string eMail = ""
                                    , bool includeContext = false
                                    , bool includeScopes = false
                                    , [System.Runtime.CompilerServices.CallerMemberName] string memberName = ""
                                    , [System.Runtime.CompilerServices.CallerFilePath] string sourceFilePath = ""
                                    , [System.Runtime.CompilerServices.CallerLineNumber] int sourceLineNumber = 0
            )
        {
            dynamic messageContext = null;
            
            if (!string.IsNullOrWhiteSpace(PassportLevel.Session))
            {
                var currentSession = PassportLevel.Session.ToLowerInvariant();
                var passportSession = self.SessionId.ToString().ToLowerInvariant();

                if (currentSession.Equals(passportSession,StringComparison.InvariantCultureIgnoreCase))
                {
                    messageContext = MessageContext(self
                        , PassportLevel.Debug
                        , message
                        , eMail
                        , memberName
                        , sourceFilePath
                        , sourceLineNumber
                        );
                }
            }

            if (messageContext == null && PassportLevel.Debug >= PassportLevel.Current)
            {
                messageContext = MessageContext(self
                    , PassportLevel.Debug
                    , message
                    , eMail
                    , memberName
                    , sourceFilePath
                    , sourceLineNumber
                    );
            }

            if (messageContext != null)
            {
                self.Stamp(messageContext, includeContext, includeScopes);
            }
        }

        public static void Info(this IPassport self
                                    , string message
                                    , string eMail = ""
                                    , bool includeContext = false
                                    , bool includeScopes = false
                                    , [System.Runtime.CompilerServices.CallerMemberName] string memberName = ""
                                    , [System.Runtime.CompilerServices.CallerFilePath] string sourceFilePath = ""
                                    , [System.Runtime.CompilerServices.CallerLineNumber] int sourceLineNumber = 0
            )
        {
            dynamic messageContext = null;

            if (!string.IsNullOrWhiteSpace(PassportLevel.Session))
            {
                var currentSession = PassportLevel.Session.ToLowerInvariant();
                var passportSession = self.SessionId.ToString().ToLowerInvariant();

                if (currentSession.Equals(passportSession, StringComparison.InvariantCultureIgnoreCase))
                {
                    messageContext = MessageContext(self
                        , PassportLevel.Info
                        , message
                        , eMail
                        , memberName
                        , sourceFilePath
                        , sourceLineNumber
                        );
                }
            }

            if (messageContext == null && PassportLevel.Info >= PassportLevel.Current)
            {
                messageContext = MessageContext(self
                    , PassportLevel.Info
                    , message
                    , eMail
                    , memberName
                    , sourceFilePath
                    , sourceLineNumber
                    );
            }

            if (messageContext != null)
            {
                self.Stamp(messageContext, includeContext, includeScopes);
            }
        }
        public static void Warn(this IPassport self
                                    , string message
                                    , string eMail = ""
                                    , bool includeContext = false
                                    , bool includeScopes = false
                                    , [System.Runtime.CompilerServices.CallerMemberName] string memberName = ""
                                    , [System.Runtime.CompilerServices.CallerFilePath] string sourceFilePath = ""
                                    , [System.Runtime.CompilerServices.CallerLineNumber] int sourceLineNumber = 0
            )
        {
            dynamic messageContext = null;

            if (!string.IsNullOrWhiteSpace(PassportLevel.Session))
            {
                var currentSession = PassportLevel.Session.ToLowerInvariant();
                var passportSession = self.SessionId.ToString().ToLowerInvariant();

                if (currentSession.Equals(passportSession, StringComparison.InvariantCultureIgnoreCase))
                {
                    messageContext = MessageContext(self
                        , PassportLevel.Warn
                        , message
                        , eMail
                        , memberName
                        , sourceFilePath
                        , sourceLineNumber
                        );
                }
            }

            if (messageContext == null && PassportLevel.Warn >= PassportLevel.Current)
            {
                messageContext = MessageContext(self
                    , PassportLevel.Warn
                    , message
                    , eMail
                    , memberName
                    , sourceFilePath
                    , sourceLineNumber
                    );
            }

            if (messageContext != null)
            {
                self.Stamp(messageContext, includeContext, includeScopes);
            }
        }

        public static void Error(this IPassport self
                                    , string message
                                    , string eMail = ""
                                    , bool includeContext = false
                                    , bool includeScopes = false
                                    , [System.Runtime.CompilerServices.CallerMemberName] string memberName = ""
                                    , [System.Runtime.CompilerServices.CallerFilePath] string sourceFilePath = ""
                                    , [System.Runtime.CompilerServices.CallerLineNumber] int sourceLineNumber = 0
            )
        {
            dynamic messageContext = null;

            if (!string.IsNullOrWhiteSpace(PassportLevel.Session))
            {
                var currentSession = PassportLevel.Session.ToLowerInvariant();
                var passportSession = self.SessionId.ToString().ToLowerInvariant();

                if (currentSession.Equals(passportSession, StringComparison.InvariantCultureIgnoreCase))
                {
                    messageContext = MessageContext(self
                        , PassportLevel.Error
                        , message
                        , eMail
                        , memberName
                        , sourceFilePath
                        , sourceLineNumber
                        );
                }
            }

            if (messageContext == null && PassportLevel.Error >= PassportLevel.Current)
            {
                messageContext = MessageContext(self
                    , PassportLevel.Error
                    , message
                    , eMail
                    , memberName
                    , sourceFilePath
                    , sourceLineNumber
                    );
            }

            if (messageContext != null)
            {
                self.Stamp(messageContext, includeContext, includeScopes);
            }
        }

        public static void Fatal(this IPassport self
                                    , string message
                                    , string eMail = ""
                                    , bool includeContext = false
                                    , bool includeScopes = false
                                    , [System.Runtime.CompilerServices.CallerMemberName] string memberName = ""
                                    , [System.Runtime.CompilerServices.CallerFilePath] string sourceFilePath = ""
                                    , [System.Runtime.CompilerServices.CallerLineNumber] int sourceLineNumber = 0
            )
        {
            dynamic messageContext = null;

            if (!string.IsNullOrWhiteSpace(PassportLevel.Session))
            {
                var currentSession = PassportLevel.Session.ToLowerInvariant();
                var passportSession = self.SessionId.ToString().ToLowerInvariant();

                if (currentSession.Equals(passportSession, StringComparison.InvariantCultureIgnoreCase))
                {
                    messageContext = MessageContext(self
                        , PassportLevel.Fatal
                        , message
                        , eMail
                        , memberName
                        , sourceFilePath
                        , sourceLineNumber
                        );
                }
            }

            if (messageContext == null && PassportLevel.Fatal >= PassportLevel.Current)
            {
                messageContext = MessageContext(self
                    , PassportLevel.Fatal
                    , message
                    , eMail
                    , memberName
                    , sourceFilePath
                    , sourceLineNumber
                    );
            }

            if (messageContext != null)
            {
                self.Stamp(messageContext, includeContext, includeScopes);
            }
        }
    }
}
