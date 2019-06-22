using System;
using System.Collections.Generic;
using MersenneTwister;
using static System.Math;

namespace Ising
{
	/// <summary>
	/// Simulator for a 1D Ising chain.
	/// </summary>
	class IsingSimulator
	{
		/// <summary>Chain length.</summary>
		public int ProblemSize;
		/// <summary>Amount of time to warm up the system.</summary>
		public int WarmupFactor;
		/// <summary>Time between samples.</summary>
		public int SamplingTime;
		/// <summary>Amount of samples to collect.</summary>
		public int SamplingCount;

		/// <summary>Magnetic field factor.</summary>
		public double ReducedH;
		/// <summary>Coupling factor.</summary>
		public double ReducedJ;

		sbyte[] State;
		double[] MagSamp;
		Random Generator;

		/// <summary>
		/// Performs a set of updates (chain length - many)
		/// </summary>
		void UpdateStateN()
		{
			for (int i = 0; i < ProblemSize; i++)
			{
				int Pos = Generator.Next(0, ProblemSize);

				int Pp1 = (Pos + 1) % ProblemSize;
				int Pm1 = (Pos + ProblemSize - 1) % ProblemSize;
				double Kappa = ReducedH + ReducedJ * (State[Pp1] + State[Pm1]);

				double Outcome = Generator.NextDouble();

				State[Pos] = (Outcome * (1 + Exp(-2 * Kappa)) < 1) ? (sbyte) 1 : (sbyte) -1;
			}
		}

		double ComputeMagSample()
		{
			double M = 0;
			foreach (sbyte sb in State) M += sb;
			return M / ProblemSize;
		}

		/// <summary>
		/// Performs a simulation of the Ising system with given parameters.
		/// </summary>
		/// <param name="MagMean">Mean magnetization.</param>
		/// <param name="Susceptibility">Magnetic susceptibility of the system.</param>
		public void PerformSimulation(out double MagMean, out double Susceptibility)
		{
			Generator = MTRandom.Create(MTEdition.Original_19937);
			State = new sbyte[ProblemSize];
			MagSamp = new double[SamplingCount];
			for (int i = 0; i < ProblemSize; i++) State[i] = 1;
			for (int i = 0; i < WarmupFactor; i++) UpdateStateN();
			MagMean = 0;
			for (int i = 0; i < SamplingCount; i++)
			{
				for (int j = 0; j < SamplingTime; j++) UpdateStateN();
				MagSamp[i] = ComputeMagSample();
				MagMean += MagSamp[i];
			}
			MagMean /= SamplingCount;
			Susceptibility = 0;
			for (int i = 0; i < SamplingCount; i++) Susceptibility += (MagSamp[i] - MagMean) * (MagSamp[i] - MagMean);
			Susceptibility *= ((double) ProblemSize) / SamplingCount;
		}

		/// <summary>Expected magnetization for the system according to theory.</summary>
		public double ExpectedMagnet
		{
			get
			{
				double Sh = Sinh(ReducedH);
				double Ch = Cosh(ReducedH);
				double EJ = Exp(-4 * ReducedJ);
				return Sh * (Ch + Sqrt(Sh * Sh + EJ)) / (Sh * Sh + EJ + Ch * Sqrt(Sh * Sh + EJ));
			}
		}

		/// <summary>
		/// Magnetization samples from the last run. If the simulation has not been performed so far, run it.
		/// </summary>
		public double[] MagnetizationSamples
		{ get { if (MagSamp == null) PerformSimulation(out _, out _); return MagSamp; } }
	}
}
