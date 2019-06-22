using System.Collections;
using System.Text;

namespace ICNL
{
	public struct Codeword
	{
		public int Length;
		public BitArray CodeWord;

		public override string ToString()
		{
			StringBuilder sb = new StringBuilder(Length, Length);
			sb.Length = Length;
			for (int i = 0; i < Length; i++) sb[i] = CodeWord[i] ? '1' : '0';
			return sb.ToString();
		}
	}
}
