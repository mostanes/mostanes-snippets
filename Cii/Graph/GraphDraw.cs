using System;
using System.Collections.Generic;
using System.Linq;
using NGraphics;

namespace Planarity
{
	/// <summary>
	/// Tools for drawing planar graphs.
	/// </summary>
	static class GraphDraw
	{
		/// <summary>
		/// Solve positions for the vertices of a connected planar given an outer face and its positions.
		/// </summary>
		/// <param name="G">Graph to be drawn.</param>
		/// <param name="KnownPoints">Positions of the vertices on the outer face.</param>
		/// <returns>A list of the positions of the graph's vertices.</returns>
		public static List<Point> SolvePositions(Graph G, Dictionary<int, Point> KnownPoints)
		{
			int MSize = G.Size;
			float[,] PointMatrix = GenerateMatrix(G, KnownPoints, MSize);
			GaussReduce(PointMatrix);
			return CreateList(PointMatrix, KnownPoints, MSize);
		}

		private static float[,] GenerateMatrix(Graph G, Dictionary<int, Point> KnownPoints, int MSize)
		{
			float[,] Matrix = new float[MSize, MSize + 2];
			for (int i = 0; i < G.Size; i++) if (!KnownPoints.ContainsKey(i))
				{
					foreach (int farVertex in G.AdjList[i])
					{
						if (KnownPoints.ContainsKey(farVertex))
						{ Matrix[i, MSize] += (float) KnownPoints[farVertex].X; Matrix[i, MSize + 1] += (float) KnownPoints[farVertex].Y; }
						else
							Matrix[i, farVertex] = -1;

					}
					Matrix[i, i] = G.AdjList[i].Count;
				}
			return Matrix;
		}

		/// <summary>Cheapo Gaussian reduction for solving the system of linear equations of the vertices' positions. No pivots here.</summary>
		private static void GaussReduce(float[,] Matrix)
		{
			for (int i = 1; i < Matrix.GetLength(0); i++)
			{
				if (Matrix[i, i] == 0) continue;
				for (int j = 0; j < Matrix.GetLength(0); j++)
				{
					if (i == j) continue;
					float Mx = Matrix[j, i] / Matrix[i, i];
					for (int k = i; k < Matrix.GetLength(1); k++)
						Matrix[j, k] -= Mx * Matrix[i, k];
				}
			}
		}

		private static List<Point> CreateList(float[,] Matrix, Dictionary<int, Point> KnownPoints, int MSize)
		{
			int GSize = MSize;
			List<Point> Vertices = new List<Point>(MSize + KnownPoints.Count);
			for (int i = 0; i < GSize; i++)
			{
				if (KnownPoints.ContainsKey(i))
					Vertices.Add(KnownPoints[i]);
				else
				{
					double V = Matrix[i, i];
					Point px = new Point(Matrix[i, MSize] / V, Matrix[i, MSize + 1] / V);
					Vertices.Add(px);
				}
			}
			return Vertices;
		}

		public static List<Point> GenerateDrawing(Graph G, int OuterCycle, Point? Center = null, double Radius = 0.8)
		{
			Point cx = Center ?? new Point(1, 1);
			Dictionary<int, Point> Cycle = new Dictionary<int, Point>();
			for (int i = 0; i < OuterCycle; i++)
			{
				double Angle = i * 2 * Math.PI / OuterCycle;
				Point px = new Point(cx.X + Radius * Math.Cos(Angle), cx.Y + Radius * Math.Sin(Angle));
				Cycle.Add(i, px);
			}
			return SolvePositions(G, Cycle);
		}

		/// <summary>
		/// Computes a drawing for a graph, given the outer face for the drawing.
		/// </summary>
		/// <param name="G">Graph to be drawn.</param>
		/// <param name="OuterCycle">Outer face of the drawing.</param>
		/// <param name="Center">Center of graph's drawing. Defaults to (1, 1).</param>
		/// <param name="Radius">Radius of the graph's drawing. Defaults to 0.8.</param>
		/// <returns>The positions of the graph's vertices.</returns>
		public static List<Point> GenerateDrawing(Graph G, List<int> OuterCycle, Point? Center = null, double Radius = 0.8)
		{
			Point cx = Center ?? new Point(1, 1);
			Dictionary<int, Point> Cycle = new Dictionary<int, Point>();
			for (int i = 0; i < OuterCycle.Count; i++)
			{
				double Angle = i * 2 * Math.PI / OuterCycle.Count;
				Point px = new Point(cx.X + Radius * Math.Cos(Angle), cx.Y + Radius * Math.Sin(Angle));
				Cycle.Add(OuterCycle[i], px);
			}
			return SolvePositions(G, Cycle);
		}
	}
}
