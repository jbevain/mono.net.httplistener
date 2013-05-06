using System;
using System.Net;
using System.Text;

namespace Mono.Net {

	static class Extensions {

		public static string ToClientString (this Cookie cookie)
		{
			if (cookie == null)
				throw new ArgumentNullException ("cookie");

			if (cookie.Name.Length == 0) 
				return String.Empty;

			StringBuilder result = new StringBuilder (64);
	
			if (cookie.Version > 0) 
				result.Append ("Version=").Append (cookie.Version).Append (";");
				
			result.Append (cookie.Name).Append ("=").Append (cookie.Value);

			if (cookie.Path != null && cookie.Path.Length != 0)
				result.Append (";Path=").Append (QuotedString (cookie, cookie.Path));
				
			if (cookie.Domain != null && cookie.Domain.Length != 0)
				result.Append (";Domain=").Append (QuotedString (cookie, cookie.Domain));			
	
			if (cookie.Port != null && cookie.Port.Length != 0)
				result.Append (";Port=").Append (cookie.Port);
						
			return result.ToString ();
		}

		// See par 3.6 of RFC 2616
		static string QuotedString (Cookie cookie, string value)
		{
			if (cookie.Version == 0 || IsToken (value))
				return value;
			else
				return "\"" + value.Replace("\"", "\\\"") + "\"";
		}

		static string tspecials = "()<>@,;:\\\"/[]?={} \t";   // from RFC 2965, 2068

		static bool IsToken (string value)
		{
			int len = value.Length;
			for (int i = 0; i < len; i++) {
				char c = value [i];
				if (c < 0x20 || c >= 0x7f || tspecials.IndexOf (c) != -1)
					return false;
			}
			return true;
		}

		public static bool MaybeUri (string s)
		{
			int p = s.IndexOf (':');
			if (p == -1)
				return false;

			if (p >= 10)
				return false;

			return IsPredefinedScheme (s.Substring (0, p));
		}

		private static bool IsPredefinedScheme (string scheme)
		{
			if (scheme == null || scheme.Length < 3)
				return false;
			
			char c = scheme [0];
			if (c == 'h')
				return (scheme == "http" || scheme == "https");
			if (c == 'f')
				return (scheme == "file" || scheme == "ftp");
				
			if (c == 'n'){
				c = scheme [1];
				if (c == 'e')
					return (scheme == "news" || scheme == "net.pipe" || scheme == "net.tcp");
				if (scheme == "nntp")
					return true;
				return false;
			}
			if ((c == 'g' && scheme == "gopher") || (c == 'm' && scheme == "mailto"))
				return true;

			return false;
		}
	}
}
