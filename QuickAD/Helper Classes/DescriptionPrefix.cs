using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace QuickAD.Helper_Classes
{
	public static class DescriptionPrefix
	{
		public static List<string> PrefixList;

		/// <summary>
		/// Initializes the Prefix list
		/// Returns a bool to catch initialization errors in settings which will be implemented later.
		/// </summary>
		public static bool Initialize(string file = "PrefixData.json")
		{
			try
			{
				PrefixList = new List<string>();
				var json = new JsonTextReader(File.OpenText(Path.Combine(Directory.GetCurrentDirectory(), file)));
				var serialize = new JsonSerializer();
				var parsedData = (JArray)serialize.Deserialize(json);
				foreach (string s in parsedData)
				{
					PrefixList.Add(s);
				}
				return true;
			}
			catch
			{
				return false;
			}
		}

		/// <summary>
		/// Get the full description including the old description's prefix
		/// </summary>
		/// <param name="oldDesc">Old full description</param>
		/// <param name="newDesc">New trimmed description</param>
		/// <returns>New full description</returns>
		public static string GetNewDescriptionWithPrefix(string oldDesc, string newDesc)
		{
			var prefix = GetDescriptionPrefix(oldDesc);
			var sb = new StringBuilder();

			//Ensure we don't overwrite a description which could help identify a prefix to
			//add to the PrefixList
			if (prefix == string.Empty) prefix = oldDesc;
			sb.Insert(0, prefix);
			sb.Append(" ");
			sb.Append(newDesc);
			return sb.ToString();
		}

		/// <summary>
		/// Get full description including description prefix
		/// </summary>
		/// <param name="trimmed">Trimmed description</param>
		/// <param name="prefix">Pre-determined description prefix</param>
		/// <returns>Full description ready to send to server</returns>
		public static string GetFullDescription(string trimmed, string prefix)
		{
			StringBuilder sb = new StringBuilder();
			sb.Insert(0, prefix);
			sb.Append(" ");
			sb.Append(trimmed);
			return sb.ToString();
		}

		/// <summary>
		/// Returns the computer's description minus the description prefix
		/// </summary>
		/// <param name="original">Description from server</param>
		/// <param name="prefix">Predetermined prefix</param>
		/// <returns>Trimmed Description string</returns>
		public static string GetTrimmedDescription(string original, string prefix)
		{
			return original.Substring(prefix.Length).Trim();
		}

		/// <summary>
		/// Determines which prefix was used in the original description
		/// </summary>
		/// <param name="oldDesc">Description pulled from server</param>
		/// <returns>prefix used in original description</returns>
		public static string GetDescriptionPrefix(string oldDesc)
		{
			if (oldDesc == null) return string.Empty;
			foreach(var s in PrefixList)
			{
				var sLength = s.Length;
				if (string.Equals(s, oldDesc.Substring(0, sLength), StringComparison.CurrentCultureIgnoreCase)) return s;
			}
			return string.Empty;
		}
	}
}
