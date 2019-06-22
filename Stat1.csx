#r "System.Drawing"
#r "System.Windows.Forms"

using System.IO;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;
using static System.Math;

struct Point
{
	public double X, Y;

	public override string ToString()
	{ return "X=" + X.ToString("G6") + " Y=" + Y.ToString("G6"); }

	public static double operator ^(Point a, Point b) => Sqrt((a.X - b.X) * (a.X - b.X) + (a.Y - b.Y) * (a.Y - b.Y));
}

class QuadTree<T>
{
	readonly int Depth;
	readonly QuadTreeNode Root;

	public QuadTree(int Depth, double Top, double Bottom, double Left, double Right)
	{
		this.Depth = Depth;
		Root = new QuadTreeNode(Top, Bottom, Left, Right);
	}

	public void Add(T Object, double X, double Y)
	{
		QuadTreeNode[] QTNList = new QuadTreeNode[Depth];
		QTNList[0] = Root;
		for (int i = 1; i < Depth; i++)
		{
			QuadTreeNode CNode = QTNList[i - 1];
			bool Bottom = Y > (CNode.Tp + CNode.Bt) / 2;
			bool Right = X > (CNode.Lf + CNode.Rg) / 2;
			if (Bottom)
			{
				if (Right)
				{
					if (CNode.nBR == null) CNode.nBR = new QuadTreeNode((CNode.Tp + CNode.Bt) / 2, CNode.Bt, (CNode.Lf + CNode.Rg) / 2, CNode.Rg);
					QTNList[i] = CNode.nBR;
				}
				else
				{
					if (CNode.nBL == null) CNode.nBL = new QuadTreeNode((CNode.Tp + CNode.Bt) / 2, CNode.Bt, CNode.Lf, (CNode.Lf + CNode.Rg) / 2);
					QTNList[i] = CNode.nBL;
				}
			}
			else
			{
				if (Right)
				{
					if (CNode.nTR == null) CNode.nTR = new QuadTreeNode(CNode.Tp, (CNode.Tp + CNode.Bt) / 2, (CNode.Lf + CNode.Rg) / 2, CNode.Rg);
					QTNList[i] = CNode.nTR;
				}
				else
				{
					if (CNode.nTL == null) CNode.nTL = new QuadTreeNode(CNode.Tp, (CNode.Tp + CNode.Bt) / 2, CNode.Lf, (CNode.Lf + CNode.Rg) / 2);
					QTNList[i] = CNode.nTL;
				}
			}
		}
		if (QTNList[Depth - 1].Bucket == null) QTNList[Depth - 1].Bucket = new List<T>();
		QTNList[Depth - 1].Bucket.Add(Object);
	}

	public List<T> Query(double Top, double Bottom, double Left, double Right)
	{
		List<T> Result = new List<T>();
		Root.Query(Top, Bottom, Left, Right, Result);
		return Result;
	}

	public List<T> Query(double X, double Y, double SquareSemiside)
	{ return Query(X - SquareSemiside, X + SquareSemiside, Y - SquareSemiside, Y + SquareSemiside); }



	private class QuadTreeNode
	{
		internal readonly double Tp, Bt, Lf, Rg;
		internal QuadTreeNode nTL, nTR, nBL, nBR;
		internal List<T> Bucket;

		public QuadTreeNode(double Top, double Bottom, double Left, double Right)
		{
			Tp = Top;
			Bt = Bottom;
			Lf = Left;
			Rg = Right;
		}

		public void Query(double Top, double Bottom, double Left, double Right, List<T> Accumulator)
		{
			bool YOK = (Tp > Top && Tp < Bottom) || (Bt > Top && Bt < Bottom) || (Tp < Top && Bt > Bottom);
			bool XOK = (Lf > Left && Lf < Right) || (Rg > Left && Rg < Right) || (Lf < Left && Rg > Right);
			if (!XOK || !YOK) return;
			if (Bucket != null)
				Accumulator.AddRange(Bucket);
			else
			{
				if (nTL != null) nTL.Query(Top, Bottom, Left, Right, Accumulator);
				if (nTR != null) nTR.Query(Top, Bottom, Left, Right, Accumulator);
				if (nBL != null) nBL.Query(Top, Bottom, Left, Right, Accumulator);
				if (nBR != null) nBR.Query(Top, Bottom, Left, Right, Accumulator);
			}
		}
	}
}

OpenFileDialog fdn = new OpenFileDialog();
fdn.ShowDialog();
Bitmap bnp = new Bitmap(fdn.FileName);
var xe = bnp.LockBits(new Rectangle(0, 0, 200, 200), System.Drawing.Imaging.ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format8bppIndexed);
byte[] Besp = new byte[bnp.Width * bnp.Height];
System.Runtime.InteropServices.Marshal.Copy(xe.Scan0, Besp, 0, Besp.Length);
List<Point> LP = new List<Point>();
for (int i = 0; i < bnp.Height; i++) for (int j = 0; j < bnp.Width; j++) if (Besp[i * bnp.Width + j] == 0) LP.Add(new Point() { X = j, Y = i });


public struct LinearRegressionParameters
{
	public double R;
	public double y0;
	public double slope;
}

/// <summary>
/// Fits a line to a collection of points.
/// </summary>
/// <param name="X">The x-axis values.</param>
/// <param name="Y">The y-axis values.</param>
[System.Diagnostics.Contracts.Pure]
public static LinearRegressionParameters ComputeLinearRegression(double[] X, double[] Y)
{
	System.Diagnostics.Contracts.Contract.Requires(X.Length == Y.Length);
	double sumX = 0;
	double sumY = 0;
	double sumXSq = 0;
	double sumYSq = 0;
	double ssX = 0;
	double ssY = 0;
	double sumCodev = 0;
	double sCo = 0;
	int i;
	for (i = 0; i < X.Length; i++)
	{
		double x = X[i];
		double y = Y[i];
		sumCodev += x * y;
		sumX += x;
		sumY += y;
		sumXSq += x * x;
		sumYSq += y * y;
	}
	ssX = sumXSq - ((sumX * sumX) / X.Length);
	ssY = sumYSq - ((sumY * sumY) / X.Length);
	double Rup = (X.Length * sumCodev) - (sumX * sumY);
	double Rdown = (X.Length * sumXSq - (sumX * sumX))
		* (X.Length * sumYSq - (sumY * sumY));
	sCo = sumCodev - ((sumX * sumY) / X.Length);

	double meanX = sumX / X.Length;
	double meanY = sumY / X.Length;
	LinearRegressionParameters lrp;
	lrp.R = Rup / Math.Sqrt(Rdown);
	lrp.y0 = meanY - ((sCo / ssX) * meanX);
	lrp.slope = sCo / ssX;
	return lrp;
}

List<(Point, double)> SelectAndSort(List<Point> jex, Func<Point, Point, double> jey)
{
	List<(Point, double)> Lst = new List<(Point, double)>();
	foreach (Point x in jex)
	{
		double sum = 0;
		foreach (Point w in jex) if (!w.Equals(x)) sum += 1 / jey(w, x);
		Lst.Add((x, sum));
	}
	Lst.Sort((x, y) => x.Item2 > y.Item2 ? 1 : -1);
	return Lst;
}

