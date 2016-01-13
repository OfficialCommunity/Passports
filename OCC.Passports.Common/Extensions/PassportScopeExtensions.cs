using System;
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
            try
            {
                return await actionAsync();
            }
            catch (Exception e)
            {
                if (!PassportSettings.Settings.ExcludedExceptions.Any(x => x.Equals(e.GetType().FullName,StringComparison.InvariantCultureIgnoreCase)))
                {
                    self.Passport.StampException(e);    
                }

                var standardError = new StandardError();

                standardError.Errors.Add(Constants.Passports.KeyError);
                
                var instance = Activator.CreateInstance<T>();
                instance.StandardError = standardError;

                return instance;
            }
            finally
            {
                if (self.Passport != null)
                {
                    self.Passport.PopScope();
                }
            }
        }

        public static T Scope<T>(this IHasPassport self, Func<T> action)
            where T : StandardResponse
        {
            try
            {
                return action();
            }
            catch (Exception e)
            {
                if (!PassportSettings.Settings.ExcludedExceptions.Any(x => x.Equals(e.GetType().FullName, StringComparison.InvariantCultureIgnoreCase)))
                {
                    self.Passport.StampException(e);
                }

                var standardError = new StandardError();

                standardError.Errors.Add(Constants.Passports.KeyError);

                var instance = Activator.CreateInstance<T>();
                instance.StandardError = standardError;

                return instance;
            }
            finally
            {
                if (self.Passport != null)
                {
                    self.Passport.PopScope();
                }
            }
        }
    }
}