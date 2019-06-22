using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace WiktionaryDataTools
{
	static class Splitter
	{
		static string WikiPath = "";
		static string ExtractPath = "";
		const int SelectorLength = 2;
		static Dictionary<string, StreamWriter> writers = new Dictionary<string, StreamWriter>();


		internal static void SplitWikiDump()
		{
			StreamReader sr = new StreamReader(WikiPath);
			StringBuilder sbr = new StringBuilder();
			while (!sr.EndOfStream)
			{
				sbr.Clear();
				string srl = sr.ReadLine();
				if (!srl.Contains("<page>")) continue;
				sbr.Append(srl);
				while (!srl.Contains("</page>")) sbr.Append(srl = sr.ReadLine() + "\n");
				srl = sbr.ToString();
				ProcessPage(srl);
			}
		}

		static void ProcessPage(string Page)
		{
			string Title = StringExtractor(Page, "<title>", "</title>", false);
			string Selector = GetSelectorFromTitle(Title);
			if (!writers.ContainsKey(Selector)) writers.Add(Selector, new StreamWriter(ExtractPath + Selector, false));
			writers[Selector].WriteLine(Page);
		}

		static string StringExtractor(string BigString, string StartTag, string EndTag, bool WithTags)
		{
			int index1 = BigString.IndexOf(StartTag);
			if (!WithTags) index1 += StartTag.Length;
			int index2 = BigString.IndexOf(EndTag, index1);
			if (WithTags) index2 += EndTag.Length;
			return BigString.Substring(index1, index2 - index1);
		}

		static string GetSelectorFromTitle(string Title)
		{
			string Selector;
			if (Title.Contains(':')) Selector = "special";
			else Selector = Title.ToLower().PadRight(SelectorLength).Substring(0, SelectorLength);
			StringBuilder sb = new StringBuilder(Selector);
			bool flag = false;
			for(int i=0;i<sb.Length;i++)
			{
				if (sb[i] < 64 | sb[i] > 122 | sb[i] == '\\') { flag = true; sb[i] = ((char) (((sb[i] - '^') % ('z' - '^') + ('z' - '^')) % ('z' - '^') + '^')); }
			}
			if (flag) Selector = "m" + sb.ToString();
			return Selector;
		}
	}
}
