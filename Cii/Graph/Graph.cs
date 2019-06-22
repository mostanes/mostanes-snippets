using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Planarity
{
	class Graph
	{
		public int Size;
		public List<Tuple<int, int>> EdgList;
		public List<int>[] AdjList;

		public Graph(int Size, List<Tuple<int, int>> EdgeList)
		{
			this.Size = Size;
			AdjList = new List<int>[Size];
			for (int i = 0; i < Size; i++) AdjList[i] = new List<int>();
			EdgList = EdgeList;
			foreach(var tp in EdgeList)
			{
				AdjList[tp.Item1].Add(tp.Item2);
				AdjList[tp.Item2].Add(tp.Item1);
			}
		}

		private Graph() { }

		/// <summary>
		/// Parses a graph from a list of strings of the two vertices of an edge (ex. "23 43").
		/// </summary>
		public static Graph FromString(IEnumerable<string> EdgeListLines)
		{
			List<Tuple<int, int>> EdgList = new List<Tuple<int, int>>();
			int Size = -1;
			foreach(string line in EdgeListLines)
			{
				string[] el = line.Split((char[]) null, StringSplitOptions.RemoveEmptyEntries);
				if (el.Length == 0) continue;
				if (el.Length != 2) throw new FormatException("Found " + el.Length + " vertices in an edge declaration.");
				int v1, v2;
				try { v1 = int.Parse(el[0]); v2 = int.Parse(el[1]); }
				catch (Exception ex) { throw new FormatException("Expected vertice numbers, got " + el[0] + ";" + el[1] + ".", ex); }
				if (v1 > Size) Size = v1;
				if (v2 > Size) Size = v2;
				EdgList.Add(new Tuple<int, int>(v1 - 1, v2 - 1));
			}
			return new Graph(Size, EdgList);
		}

		/// <summary>Reads a file and parses it with <see cref="FromString"/>.</summary>
		public static Graph FromFile(string Path) => FromString(System.IO.File.ReadAllLines(Path));

		/// <summary>Creates a shallow clone of the current graph, with the specified edges removed from the graph.</summary>
		public static Graph operator -(Graph G, List<Tuple<int, int>> EdgeList)
		{
			List<Tuple<int, int>> NewEdges = G.EdgList.Where((x) => !EdgeList.Any((y) =>
			((y.Item1 == x.Item1 & y.Item2 == x.Item2) || (y.Item1 == x.Item2 & y.Item2 == x.Item1))
			)).ToList();
			return new Graph(G.Size, NewEdges);
		}

		/// <summary>Creates a shallow clone of the current graph, with the specified edges added to the graph.</summary>
		public static Graph operator +(Graph G, List<Tuple<int, int>> EdgeList)
		{
			List<Tuple<int, int>> NewEdges = G.EdgList.ToList();
			foreach (var x in EdgeList)
				if (!NewEdges.Any((y) => (y.Item1 == x.Item1 & y.Item2 == x.Item2) || (y.Item1 == x.Item2 & y.Item2 == x.Item1)))
					NewEdges.Add(x);
			return new Graph(G.Size, NewEdges);
		}

		/// <summary>Shallow clone.</summary>
		public static Graph operator +(Graph G) => new Graph(G.Size, G.EdgList.ToList());
	}
}
