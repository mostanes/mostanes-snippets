using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Planarity
{
	/// <summary>
	/// Tools for finding connected components, bridges and other leftovers.
	/// </summary>
	static class BridgeComp
	{
		/// <summary>Finds the connected components of a graph.</summary>
		public static List<List<int>> ComputeCC(Graph G)
		{
			BitArray processed = new BitArray(G.Size);
			List<List<int>> CCSet = new List<List<int>>();
			Stack<int> ToProcess = new Stack<int>();
			for (int i = 0; i < G.Size; i++)
			{
				if (processed[i]) continue;
				List<int> Component = new List<int>();
				ToProcess.Push(i); /* Always empty here from previous run */
				while (ToProcess.Count > 0)
				{
					int px = ToProcess.Pop();
					if (processed[px]) continue;
					processed[px] = true;
					Component.Add(px);
					foreach (int Ngh in G.AdjList[px]) ToProcess.Push(Ngh);
				}
				CCSet.Add(Component);
			}
			return CCSet;
		}

		/// <summary>
		/// Find bridges in a graph from a given cycle.
		/// </summary>
		/// <param name="G">Input graph.</param>
		/// <param name="SortedCycle">Separating cycle, with vertices sorted by index.</param>
		/// <param name="OrderedCycle">Separating cycle, with vertices in their order in the cycle.</param>
		/// <param name="AttVertexList">List of cycle attachment vertices.</param>
		/// <returns>The list of bridges in edge list from.</returns>
		public static List<List<Tuple<int, int>>> ComputeBridges(Graph G, List<int> SortedCycle, List<int> OrderedCycle, out List<List<int>> AttVertexList)
		{
			BitArray processed = new BitArray(G.Size);
			List<List<Tuple<int, int>>> BridgeSet = new List<List<Tuple<int, int>>>();
			AttVertexList = new List<List<int>>();
			Stack<Tuple<int, int>> ToProcess = new Stack<Tuple<int, int>>();
			for (int i = 0; i < G.Size; i++)
			{
				if (SortedCycle.BinarySearch(i) >= 0) continue;
				if (processed[i]) continue;
				List<Tuple<int, int>> Bridge = new List<Tuple<int, int>>();
				HashSet<int> AttVertices = new HashSet<int>();
				ToProcess.Push(new Tuple<int, int>(-1, i)); /* Always empty here from previous run */
				while (ToProcess.Count > 0)
				{
					var px = ToProcess.Pop();
					/* If in cycle, add to attvertices and bridge, but do not continue */
					if (SortedCycle.BinarySearch(px.Item2) >= 0)
					{
						System.Diagnostics.Debug.Assert(!processed[px.Item2]);
						AttVertices.Add(px.Item2);
					}
					/* Normal CC */
					else
					{
						if (processed[px.Item2]) continue;
						processed[px.Item2] = true;
						foreach (int Ngh in G.AdjList[px.Item2]) if (!processed[Ngh])
							{
								var ntp = new Tuple<int, int>(px.Item2, Ngh);
								ToProcess.Push(ntp);
								Bridge.Add(ntp);
							}
					}
				}
				BridgeSet.Add(Bridge);
				AttVertexList.Add(AttVertices.ToList());
			}
			foreach (var tp in G.EdgList)
			{
				if (SortedCycle.BinarySearch(tp.Item1) >= 0 && SortedCycle.BinarySearch(tp.Item2) >= 0)
				{
					int Idx1 = OrderedCycle.IndexOf(tp.Item1);
					int Idx2 = OrderedCycle.IndexOf(tp.Item2);
					int Dist = Math.Abs(Idx1 - Idx2);
					if (!(Dist == 1 || Dist == SortedCycle.Count - 1))
					{
						BridgeSet.Add(new List<Tuple<int, int>>() { tp });
						AttVertexList.Add(new List<int>() { tp.Item1, tp.Item2 });
					}
				}
			}

			return BridgeSet;
		}

		/// <summary>Basically Dijkstra.</summary>
		static int[] ShortestPath(Graph G, int Source, int Destination)
		{
			Queue<int> ToProcess = new Queue<int>();
			ToProcess.Enqueue(Source);
			int[] ShoPointer = new int[G.Size];
			int[] PathLength = new int[G.Size];
			for (int i = 0; i < G.Size; i++) PathLength[i] = -1;
			PathLength[Source] = 0;
			while(ToProcess.Count > 0)
			{
				int Node = ToProcess.Dequeue();
				if (Node == Destination) break;
				foreach (int Ngh in G.AdjList[Node])
					if (PathLength[Ngh] > 1 + PathLength[Node] | PathLength[Ngh] == -1)
					{
						PathLength[Ngh] = 1 + PathLength[Node];
						ShoPointer[Ngh] = Node;
						ToProcess.Enqueue(Ngh);
					}
			}

			if (PathLength[Destination] == -1) return null;
			int[] Path = new int[PathLength[Destination] + 1];
			for (int i = PathLength[Destination], Ptr = Destination; i >= 0; i--, Ptr = ShoPointer[Ptr])
				Path[i] = Ptr;
			return Path;
		}
	}
}
