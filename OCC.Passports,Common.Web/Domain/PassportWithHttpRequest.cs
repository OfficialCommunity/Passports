using System.Dynamic;
using Newtonsoft.Json.Linq;
using OCC.Passports.Common.Contracts;
using OCC.Passports.Common.Contracts.Services;
using OCC.Passports.Common.Domains;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Web;

namespace OCC.Passports.Common.Web.Domain
{
    public class PassportWithHttpRequest : Passport
    {
        private static readonly Regex IpAddressRegex =
            new Regex(
                @"\A(?:\b(?:(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.){3}(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\b)(:[1-9][0-9]{0,4})?\z",
                RegexOptions.Compiled);

        private const string KeyHttpContext = "MS_HttpContext";
        private const string KeyRemoteEndpointMessage = "System.ServiceModel.Channels.RemoteEndpointMessageProperty";

        private readonly KeyValuePair<string, dynamic>[] _extendedContexts;
        private readonly dynamic _httpRequestContext;

        public PassportWithHttpRequest(IPassportStorageService passportStorageService
            , HttpRequestMessage requestMessage
            )
            : base(passportStorageService)
        {
            if (requestMessage != null)
            {
                _httpRequestContext = BuildImmutableHttpRequestMessage(requestMessage);
            }
            else if (HttpContext.Current != null && HttpContext.Current.Request != null)
            {
                _httpRequestContext = BuildImmutableHttpRequest(HttpContext.Current.Request);
            }

            _extendedContexts = new []
            {
                new KeyValuePair<string, dynamic>(Constants.Passports.KeyHttpRequest, _httpRequestContext)
            };
        }

        protected override IEnumerable<KeyValuePair<string, dynamic>> ExtendedContexts()
        {
            return _extendedContexts;
        }

        private static dynamic BuildImmutableHttpRequestMessage(HttpRequestMessage request)
        {
            dynamic context = new ExpandoObject();

            context.HostName = request.RequestUri.Host;
            context.Url = request.RequestUri.AbsolutePath;
            context.HttpMethod = request.Method.ToString();
            context.IPAddress = GetIpAddress(request);
            var nvp = request.GetQueryNameValuePairs().ToList();
            context.Form = ToDictionary(nvp, PassportWebSettings.IsFormFieldIgnored);
            context.QueryString = ToDictionary(nvp, s => false);
            context.Headers = SetHeaders(request, PassportWebSettings.IsHeaderIgnored);

            dynamic response = new ExpandoObject();

            response.Request = context;

            return response;
        }

        private static dynamic BuildImmutableHttpRequest(HttpRequest request)
        {
            dynamic context = new ExpandoObject();

            try
            {
                context.HostName = request.Url.Host;
                context.Url = request.Url.AbsolutePath;
                context.HttpMethod = request.RequestType;
                context.IPAddress = GetIpAddress(request);
            }
            catch (Exception e)
            {
                System.Diagnostics.Trace.WriteLine("Failed to get basic request info: {0}", e.Message);
            }

            try
            {
                context.QueryString = ToDictionary(request.QueryString, null);
            }
            catch (Exception e)
            {
                if (context.QueryString == null)
                {
                    context.QueryString = new Dictionary<string, string>() {{"Failed to retrieve", e.Message}};
                }
            }

            try
            {
                context.Form = ToDictionary(request.Form, PassportWebSettings.IsFormFieldIgnored, true);
            }
            catch (Exception e)
            {
                if (context.Form == null)
                {
                    context.Form = new Dictionary<string, string>() {{"Failed to retrieve", e.Message}};
                }
            }

            try
            {
                context.Cookies = GetCookies(request.Cookies, PassportWebSettings.IsCookieIgnored);
            }
            catch (Exception e)
            {
                System.Diagnostics.Trace.WriteLine("Failed to get cookies: {0}", e.Message);
            }

            try
            {
                context.Data = ToDictionary(request.ServerVariables, PassportWebSettings.IsServerVariableIgnored);
                context.Data.Remove("ALL_HTTP");
                context.Data.Remove("HTTP_COOKIE");
                context.Data.Remove("ALL_RAW");
            }
            catch (Exception e)
            {
                if (context.Data == null)
                {
                    context.Data = new Dictionary<string, string>() {{"Failed to retrieve", e.Message}};
                }
            }

            dynamic response = new ExpandoObject();

            response.Request = context;

            return response;
        }

        private static IDictionary ToDictionary(IEnumerable<KeyValuePair<string, string>> kvPairs,
            Func<string, bool> ignored)
        {
            var dictionary = new Dictionary<string, string>();
            foreach (var pair in kvPairs.Where(kv => !ignored(kv.Key)))
            {
                dictionary[pair.Key] = pair.Value;
            }
            return dictionary;
        }

        private static IDictionary ToDictionary(NameValueCollection nameValueCollection, Func<string, bool> ignore,
            bool truncateValues = false)
        {
            IEnumerable<string> keys;

            try
            {
                keys = ignore == null ? nameValueCollection.AllKeys.Where(k => k != null) : nameValueCollection.AllKeys.Where(k => !ignore(k));
            }
            catch (Exception e)
            {
                return new Dictionary<string, string> {{"Failed to retrieve", e.Message}};
            }

            var dictionary = new Dictionary<string, string>();

            foreach (var key in keys)
            {
                try
                {
                    var keyToSend = key;
                    var valueToSend = nameValueCollection[key];

                    if (truncateValues)
                    {
                        if (keyToSend.Length > 256)
                        {
                            keyToSend = keyToSend.Substring(0, 256);
                        }

                        if (valueToSend != null && valueToSend.Length > 256)
                        {
                            valueToSend = valueToSend.Substring(0, 256);
                        }
                    }

                    dictionary.Add(keyToSend, valueToSend);
                }
                catch (Exception e)
                {
                    dictionary.Add(key, e.Message);
                }
            }

            return dictionary;
        }

        private static string GetIpAddress(HttpRequest request)
        {
            string ip = null;

            try
            {
                ip = request.ServerVariables["HTTP_X_FORWARDED_FOR"];

                if (ip != null && ip.Trim().Length > 0)
                {
                    var addresses = ip.Split(new char[] {',', ' '}, StringSplitOptions.RemoveEmptyEntries);
                    if (addresses.Length > 0)
                    {
                        // first one = client IP per http://en.wikipedia.org/wiki/X-Forwarded-For
                        ip = addresses[0];
                    }
                }

                if (ip != null && IpAddressRegex.IsMatch(ip.Trim()))
                {
                    ip = string.Empty;
                }

                if (ip == null || ip.Trim().Length == 0)
                {
                    ip = request.ServerVariables["REMOTE_ADDR"];
                }

                if (ip != null && IpAddressRegex.IsMatch(ip.Trim()))
                {
                    ip = string.Empty;
                }

                if (ip == null || ip.Trim().Length == 0)
                {
                    ip = request.UserHostAddress;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine("Failed to get IP address: {0}", ex.Message);
            }

            return ip;
        }

        private static string GetIpAddress(HttpRequestMessage request)
        {
            try
            {
                if (request.Properties.ContainsKey(KeyHttpContext))
                {
                    dynamic ctx = request.Properties[KeyHttpContext];
                    if (ctx != null)
                    {
                        return ctx.Request.UserHostAddress;
                    }
                }

                if (request.Properties.ContainsKey(KeyRemoteEndpointMessage))
                {
                    dynamic remoteEndpoint = request.Properties[KeyRemoteEndpointMessage];
                    if (remoteEndpoint != null)
                    {
                        return remoteEndpoint.Address;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Failed to get IP address: {0}", ex.Message);
            }
            return null;
        }

        private static IList GetCookies(HttpCookieCollection cookieCollection, Func<string, bool> ignore)
        {
            return Enumerable.Range(0, cookieCollection.Count)
                .Select(i => cookieCollection[i])
                .Where(c => !ignore(c.Name))
                .Select(c => new KeyValuePair<string, string>(c.Name, c.Value))
                .ToList();
        }

        private static Dictionary<string, string> SetHeaders(HttpRequestMessage request, Func<string, bool> ignored)
        {
            var headers = new Dictionary<string, string>();

            foreach (var header in request.Headers.Where(h => !ignored(h.Key)))
            {
                headers[header.Key] = string.Join(",", header.Value);
            }

            try
            {
                if (request.Content.Headers.ContentLength.HasValue && request.Content.Headers.ContentLength.Value > 0)
                {
                    foreach (var header in request.Content.Headers)
                    {
                        headers[header.Key] = string.Join(",", header.Value);
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine("Error retrieving Headers: {0}", ex.Message);
            }

            return headers;
        }
    }
}