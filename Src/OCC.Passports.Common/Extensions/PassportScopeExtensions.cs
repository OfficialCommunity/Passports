using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using OCC.Passports.Common.Contracts.Infrastructure;
using OCC.Passports.Common.Infrastructure;

namespace OCC.Passports.Common.Extensions
{
    public static class PassportScopeExtensions
    {
        public static async Task<T> ScopeAsync<T>(this IHasPassport self, Func<Task<T>> actionAsync)
            where T : StandardResponse
        {
            return await self.Passport.ScopeAsync(actionAsync);
        }

        public static T Scope<T>(this IHasPassport self, Func<T> action)
            where T : StandardResponse
        {
            return self.Passport.Scope(action);
        }

        public static async Task<T> ScopeAsync<T>(this IPassport self, Func<Task<T>> actionAsync)
            where T : StandardResponse
        {
            Stopwatch sw = null;
            try
            {
                sw = Stopwatch.StartNew();
                return await actionAsync();
            }
            catch (Exception e)
            {
                if (sw != null)
                {
                    sw.Stop();
                }

                if (!PassportSettings.Settings.ExcludedExceptions.Any(x => x.Equals(e.GetType().FullName, StringComparison.InvariantCultureIgnoreCase)))
                {
                    if (sw != null)
                    {
                        sw.Stop();
                        var ElapsedMilliseconds = sw.ElapsedMilliseconds;
                        self.Scope.Record(() => ElapsedMilliseconds, Constants.Passports.KeyOnExit);
                        sw = null;
                    }

                    self.Exception(e);
                }

                var standardError = new StandardError();

                standardError.Errors.Add(Constants.Passports.KeyError);

                var instance = Activator.CreateInstance<T>();
                instance.StandardError = standardError;

                return instance;
            }
            finally
            {
                if (self != null)
                {
                    if (sw != null)
                    {
                        sw.Stop();
                        var ElapsedMilliseconds = sw.ElapsedMilliseconds;
                        self.Scope.Record(() => ElapsedMilliseconds, Constants.Passports.KeyOnExit, debug: false);
                    }

                    self.Debug(Constants.Passports.KeyEndOfRequest, includeContext: true, includeScopes: true);
                    //self.PopScope();
                }
            }
        }

        public static T Scope<T>(this IPassport self, Func<T> action)
            where T : StandardResponse
        {
            Stopwatch sw = null;
            try
            {
                sw = Stopwatch.StartNew();
                return action();
            }
            catch (Exception e)
            {
                if (sw != null)
                {
                    sw.Stop();
                }

                if (!PassportSettings.Settings.ExcludedExceptions.Any(x => x.Equals(e.GetType().FullName, StringComparison.InvariantCultureIgnoreCase)))
                {
                    if (sw != null)
                    {
                        sw.Stop();
                        var ElapsedMilliseconds = sw.ElapsedMilliseconds;
                        self.Scope.Record(() => ElapsedMilliseconds, Constants.Passports.KeyOnExit);
                        sw = null;
                    }

                    self.Exception(e);
                }

                var standardError = new StandardError();

                standardError.Errors.Add(Constants.Passports.KeyError);

                var instance = Activator.CreateInstance<T>();
                instance.StandardError = standardError;

                return instance;
            }
            finally
            {
                if (self != null)
                {
                    if (sw != null)
                    {
                        sw.Stop();
                        var ElapsedMilliseconds = sw.ElapsedMilliseconds;
                        self.Scope.Record(() => ElapsedMilliseconds, Constants.Passports.KeyOnExit);
                    }

                    //self.PopScope();
                }
            }
        }
    }
}