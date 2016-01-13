using PostSharp.Aspects;

namespace OCC.Passports.Common.Extensions
{
    public static class PostSharpExtensions
    {
        public static string ScopeName(this MethodExecutionArgs self)
        {
            return string.Format("{0}.{1}",
                self.Method.DeclaringType != null ? self.Method.DeclaringType.FullName : "unknown",
                self.Method.Name);
            
        }
    }
}
