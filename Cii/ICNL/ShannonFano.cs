using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using static System.Math;

namespace ICNL
{
	static class ShannonFano
	{
		public static Codeword[] PrepareSFCode(double[] Probabilities)
		{
			double[] Probs = (double[]) Probabilities.Clone();
			int[] Values = new int[Probs.Length];
			for (int i = 0; i < Values.Length; i++) Values[i] = i;
			Array.Sort(Probs, Values);

			int[] Lengths = Probs.Select((x) => (int) Ceiling(-Log(x) / Log(2))).ToArray();
			BigInteger counter = new BigInteger(0);
			int Clength = Lengths[Lengths.Length - 1];

			Codeword[] CSet = new Codeword[Probabilities.Length];

			for (int i = Lengths.Length - 1; i >= 0; i--)
			{
				if (Lengths[i] > Clength) { counter >>= (Clength - Lengths[i]); Clength = Lengths[i]; }
				CSet[Values[i]] = new Codeword() { Length = Clength, CodeWord = GetBits(counter, Clength) };
				CSet[Values[i]].CodeWord.Length = Clength;
				counter++;
			}

			return CSet;
		}

		static System.Collections.BitArray GetBits(BigInteger number, int BitCount)
		{
			System.Collections.BitArray array = new System.Collections.BitArray(BitCount);
			for (int i = 0; i < BitCount; i++) { array[BitCount - i - 1] = !number.IsEven; number >>= 1; }
			return array;
		}
	}
}
