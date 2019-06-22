using System;
using System.Collections.Generic;
using System.Linq;
using NGraphics;

namespace Planarity
{
	/// <summary>
	/// Creates images of graphs.
	/// </summary>
	static class GraphImage
	{
		static double VertexSize = 0.05;
		static double LineThickness = 0.005;
		static Color VertexColor = Color.FromRGB(0, 0, 1);
		static Color LineColor = Color.FromRGB(0, 0, 0);
		static Font VertexFont = new Font("Courier New Bold", 0.75 * VertexSize);
		static Color TextColor = Color.FromRGB(1, 1, 0);

		/// <summary>
		/// Creates and saves an image of a graph.
		/// </summary>
		/// <param name="G">Input graph.</param>
		/// <param name="Vertices">Positions of the vertices.</param>
		/// <param name="Filename">File path to save to.</param>
		public static void ImageGraph(Graph G, List<Point> Vertices, string Filename)
		{
			var Canvas = Platforms.Current.CreateImageCanvas(new Size(2), 1000);
			double rvDelta = VertexSize / 2;

			foreach (var tp in G.EdgList)
				Canvas.DrawLine(Vertices[tp.Item1], Vertices[tp.Item2], LineColor, LineThickness);

			for (int i = 0; i < Vertices.Count; i++)
			{
				Point vtx = (Point) Vertices[i];
				vtx.X -= rvDelta;
				vtx.Y -= rvDelta;
				Canvas.FillEllipse(vtx.X, vtx.Y, VertexSize, VertexSize, VertexColor);
				string Exp = (i + 1).ToString();
				var measure = Canvas.MeasureText(Exp, VertexFont);
				vtx.X -= measure.Size.Width / 2 - rvDelta;
				vtx.Y += 1.75 * rvDelta;
				Canvas.DrawText((i + 1).ToString(), vtx, VertexFont, TextColor);
			}				


			Canvas.GetImage().SaveAsPng(Filename);
		}
	}
}
