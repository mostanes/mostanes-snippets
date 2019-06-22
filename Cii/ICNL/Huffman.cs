using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ICNL
{
	static class Huffman
	{
		class HNode : IComparer<HNode>
		{
			internal double Probability;
			internal HNode Zero;
			internal HNode One;
			internal int LeafNumber;

			public int Compare(HNode x, HNode y)
			{
				if (x.Probability == y.Probability)
					return 0;
				else if (x.Probability < y.Probability)
					return -1;
				else
					return 1;
			}

			public override string ToString() => "Code " + LeafNumber + " [" + Probability + "]";
		}

		public static Codeword[] PrepareHuffmanCode(double[] Probabilities)
		{
			List<HNode> Roots = new List<HNode>();
			
			for (int i = 0; i < Probabilities.Length; i++)
			{
				HNode hn = new HNode() { Probability = Probabilities[i], LeafNumber = i };
				Roots.Add(hn);
			}
			double SmallestProb = Roots[0].Probability;
			while(Roots.Count != 1)
			{
				HNode a = Roots[0];
				HNode b = Roots[1];
				Roots.RemoveRange(0, 2);
				HNode Ha = new HNode() { Probability = a.Probability + b.Probability, LeafNumber = -1, Zero = a, One = b };
				Roots.Add(Ha);
				Roots.Sort(Ha);
			}
			HNode Rootnode = Roots[0];

			System.Diagnostics.Debug.Assert(Math.Abs(Rootnode.Probability - 1.0) < SmallestProb / 10);

			Codeword[] Result = new Codeword[Probabilities.Length];
			BitArray BA = new BitArray(Probabilities.Length);
			GenerateCodewordList(Rootnode, BA, Result, 0);
			return Result;
		}

		static void GenerateCodewordList(HNode Tree, BitArray CurrentPattern, Code[] Array, int Depth)
		{
			if (Tree.LeafNumber == -1)
			{
				CurrentPattern[Depth] = false; GenerateCodewordList(Tree.Zero, CurrentPattern, Array, Depth + 1);
				CurrentPattern[Depth] = true; GenerateCodewordList(Tree.One, CurrentPattern, Array, Depth + 1);
			}
			else
			{ Array[Tree.LeafNumber] = new Codeword() { Length = Depth, CodeWord = (BitArray) CurrentPattern.Clone() }; Array[Tree.LeafNumber].CodeWord.Length = Depth; }
		}
	}
}
