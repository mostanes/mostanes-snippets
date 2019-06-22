using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WiktionaryDataTools
{
	class CultureData
	{
		internal struct Language
		{
			internal string[] Names;
			internal string DisplayName;

			internal Language(string[] Names, string DisplayName) { this.Names = Names; this.DisplayName = DisplayName; }
		}

		internal static Dictionary<string, Language> ShortCodes;
		internal static Dictionary<string, Language> LongCodes;
		internal static Dictionary<string, Language> AllCodes;
		internal static Language[] Languages = Populator();

		static Language[] Populator()
		{
			CultureInfo[] cuinfo = CultureInfo.GetCultures(CultureTypes.AllCultures);
			List<Language> langslist = new List<Language>();
			ShortCodes = new Dictionary<string, Language>();
			LongCodes = new Dictionary<string, Language>();
			AllCodes = new Dictionary<string, Language>();
			Language l;
			for (int i = 0; i < cuinfo.Length; i++)
			{
				CultureInfo ci = cuinfo[i];
				if (ShortCodes.ContainsKey(ci.TwoLetterISOLanguageName)) continue;
				List<string> Names = new List<string>() { ci.DisplayName, ci.TwoLetterISOLanguageName, ci.ThreeLetterISOLanguageName, ci.EnglishName, ci.NativeName };
				for (int j = 0; j < Names.Count; j++) for (int k = Names.Count - 1; k > j; k--) if (Names[j] == Names[k]) Names.RemoveAt(k);
				langslist.Add(l = new Language(Names.ToArray(), ci.EnglishName));
				foreach (string cd in l.Names) AllCodes.Add(cd, l);
				ShortCodes.Add(ci.TwoLetterISOLanguageName, l);
				if (ci.ThreeLetterISOLanguageName != ci.TwoLetterISOLanguageName) ShortCodes.Add(ci.ThreeLetterISOLanguageName, l);
				LongCodes.Add(l.DisplayName, l);
			}
			return langslist.ToArray();
		}
	}
}
