﻿using System;
using OCC.Passports.Common.Contracts.Infrastructure;
using OCC.Passports.Common.Domains;
using OCC.Passports.Common.Infrastructure.Contexts;

namespace OCC.Passports.Common.Extensions
{
    public static class PassportExtensions
    {
        // Akka.Net Use the Debug, Info, Warn and Error methods to log.

        public static Passport ResolveAsPassport(this object o)
        {
            var instance = o as IHasPassport;
            if (instance == null) return null;

            var thisPassport = instance.Passport;
            if (thisPassport == null) return null;

            var concretePassport = thisPassport as Passport;
            return concretePassport;
        }

        private static MessageContext MessageContext(IPassport passport
                                                            , PassportLevel level
                                                            , string messageTemplate
                                                            , object[] messageTemplateParameters
                                                            , string user
                                                            , string memberName
                                                            , string sourceFilePath
                                                            , int sourceLineNumber
                                                            , bool logCaller = false
                                                            , int scopeDepth = -1
                                                            )
        {
            var scope = passport.Scope;

            var messageContext = new MessageContext
            {
                Session = passport.SessionId,
                Passport = passport.PassportId,
                Member = scope != null ? scope.Name ?? string.Empty : string.Empty,
                CallContext = scope != null ? scope.Id.ToString() : string.Empty,
                ParentContext = scope != null ? (scope.Parent != null ? scope.Parent.Id.ToString() : string.Empty) : string.Empty,
                Level = level,
                MessageTemplate = messageTemplate,
                MessageTemplateParameters = messageTemplateParameters,
                User = user,
                MemberName = memberName,
                SourceFilePath = sourceFilePath,
                SourceLineNumber = sourceLineNumber,
                LogCaller = logCaller,
                ScopeDepth = scopeDepth
            };

            Console.WriteLine("{0} {1}"
                                    , messageContext.Timestamp.ToString("HH:mm:ss.FFFFFFF")
                                    , messageContext.Id
                                    //, messageContext.Session
                                    //, messageContext.Passport
                                    //, messageContext.MessageTemplate
                                    );
            return messageContext;
        }

        public static void Debug(this IPassport self
                                    , string messageTemplate
                                    , object[] messageTemplateData = null
                                    , string user = ""
                                    , bool includeContext = false
                                    , bool includeScopes = false
                                    , int scopeDepth = -1
                                    , [System.Runtime.CompilerServices.CallerMemberName] string memberName = ""
                                    , [System.Runtime.CompilerServices.CallerFilePath] string sourceFilePath = ""
                                    , [System.Runtime.CompilerServices.CallerLineNumber] int sourceLineNumber = 0
            )
        {
            if (PassportLevel.Debug < PassportLevel.Current) return;

            var messageContext = MessageContext(self
                , PassportLevel.Debug
                , messageTemplate
                , messageTemplateData
                , user
                , memberName
                , sourceFilePath
                , sourceLineNumber
                , scopeDepth: scopeDepth
                );

            self.Stamp(messageContext, includeContext, includeScopes);
        }

        public static void Info(this IPassport self
                                    , string messageTemplate
                                    , object[] messageTemplateData = null
                                    , string user = ""
                                    , bool includeContext = false
                                    , bool includeScopes = false
                                    , int scopeDepth = -1
                                    , [System.Runtime.CompilerServices.CallerMemberName] string memberName = ""
                                    , [System.Runtime.CompilerServices.CallerFilePath] string sourceFilePath = ""
                                    , [System.Runtime.CompilerServices.CallerLineNumber] int sourceLineNumber = 0
            )
        {
            if (PassportLevel.Info < PassportLevel.Current) return;

            var messageContext = MessageContext(self
                , PassportLevel.Info
                , messageTemplate
                , messageTemplateData
                , user
                , memberName
                , sourceFilePath
                , sourceLineNumber
                , scopeDepth: scopeDepth
                );

            self.Stamp(messageContext, includeContext, includeScopes);
        }

        public static void Warn(this IPassport self
                                    , string messageTemplate
                                    , object[] messageTemplateData = null
                                    , string user = ""
                                    , bool includeContext = false
                                    , bool includeScopes = false
                                    , int scopeDepth = -1
                                    , [System.Runtime.CompilerServices.CallerMemberName] string memberName = ""
                                    , [System.Runtime.CompilerServices.CallerFilePath] string sourceFilePath = ""
                                    , [System.Runtime.CompilerServices.CallerLineNumber] int sourceLineNumber = 0
            )
        {
            if (PassportLevel.Warn < PassportLevel.Current) return;

            var messageContext = MessageContext(self
                , PassportLevel.Warn
                , messageTemplate
                , messageTemplateData
                , user
                , memberName
                , sourceFilePath
                , sourceLineNumber
                , scopeDepth: scopeDepth
                );

            self.Stamp(messageContext, includeContext, includeScopes);
        }

        public static void Error(this IPassport self
                                    , string messageTemplate
                                    , object[] messageTemplateData = null
                                    , string user = ""
                                    , bool includeContext = false
                                    , bool includeScopes = false
                                    , int scopeDepth = -1
                                    , [System.Runtime.CompilerServices.CallerMemberName] string memberName = ""
                                    , [System.Runtime.CompilerServices.CallerFilePath] string sourceFilePath = ""
                                    , [System.Runtime.CompilerServices.CallerLineNumber] int sourceLineNumber = 0
            )
        {
            if (PassportLevel.Error < PassportLevel.Current) return;

            var messageContext = MessageContext(self
                , PassportLevel.Error
                , messageTemplate
                , messageTemplateData
                , user
                , memberName
                , sourceFilePath
                , sourceLineNumber
                , scopeDepth: scopeDepth
                );

            self.Stamp(messageContext, includeContext, includeScopes);
        }

        public static void Exception(this IPassport self
                                    , Exception e
                                    , string user = ""
                                    , [System.Runtime.CompilerServices.CallerMemberName] string memberName = ""
                                    , [System.Runtime.CompilerServices.CallerFilePath] string sourceFilePath = ""
                                    , [System.Runtime.CompilerServices.CallerLineNumber] int sourceLineNumber = 0
            )
        {
            var messageContext = MessageContext(self
                , PassportLevel.Exception
                , null
                , null
                , null
                , memberName
                , sourceFilePath
                , sourceLineNumber
                );

            self.StampException(messageContext, e);
        }

        public static void RecordHistory(this IPassport self
                                    , string messageTemplate
                                    , object[] messageTemplateData = null
                                    , string user = ""
                                    , bool includeScopes = false
                                    , string memberName = ""
                                    , string sourceFilePath = ""
                                    , int sourceLineNumber = 0
            )
        {
            if (PassportLevel.Debug < PassportLevel.Current) return;

            var messageContext = MessageContext(self
                , PassportLevel.Debug
                , messageTemplate
                , messageTemplateData
                , user
                , memberName
                , sourceFilePath
                , sourceLineNumber
                , logCaller: true
                , scopeDepth: 0
                );

            self.Stamp(messageContext, includeCallContext: true, includeSnapshot: includeScopes);
        }
    }
}
