// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Fourier.cs" company="">
//   
// </copyright>
// <summary>
//   Fourier transform
//   *****************************************************************************
//   *
//   * Copyright (c) 2002, Wilhelm Kurz.  All Rights Reserved.
//   * wkurz@foni.net
//   *
//   * This file is provided for demonstration and educational uses only.
//   * Permission to use, copy, modify and distribute this file for
//   * any purpose and without fee is hereby granted.
//   *
//   *****************************************************************************
//   Converted/optimized by Nikse from vb code: http://www.wilhelm-kurz-software.de/dynaplot/applicationnotes/spectrogram.htm
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Nikse.SubtitleEdit.Logic
{
    using System;

    /// <summary>
    /// Fourier transform
    ///
    /// *****************************************************************************
    /// *
    /// * Copyright (c) 2002, Wilhelm Kurz.  All Rights Reserved.
    /// * wkurz@foni.net
    /// *
    /// * This file is provided for demonstration and educational uses only.
    /// * Permission to use, copy, modify and distribute this file for
    /// * any purpose and without fee is hereby granted.
    /// *
    /// *****************************************************************************
    /// Converted/optimized by Nikse from vb code: http://www.wilhelm-kurz-software.de/dynaplot/applicationnotes/spectrogram.htm
    /// </summary>
    internal class Fourier
    {
        /// <summary>
        /// The w 0 hanning.
        /// </summary>
        public const double W0Hanning = 0.5;

        /// <summary>
        /// The w 0 hamming.
        /// </summary>
        public const double W0Hamming = 0.54;

        /// <summary>
        /// The w 0 blackman.
        /// </summary>
        public const double W0Blackman = 0.42;

        /// <summary>
        /// The pi.
        /// </summary>
        private const double Pi = 3.14159265358979;

        /// <summary>
        /// The _array size.
        /// </summary>
        private int _arraySize;

        /// <summary>
        /// The _forward.
        /// </summary>
        private bool _forward;

        /// <summary>
        /// The _ld arraysize.
        /// </summary>
        private int _ldArraysize = 0;

        /// <summary>
        /// The cosarray.
        /// </summary>
        private double[] cosarray;

        /// <summary>
        /// The sinarray.
        /// </summary>
        private double[] sinarray;

        /// <summary>
        /// Initializes a new instance of the <see cref="Fourier"/> class.
        /// </summary>
        /// <param name="arraySize">
        /// The array size.
        /// </param>
        /// <param name="forward">
        /// The forward.
        /// </param>
        public Fourier(int arraySize, bool forward)
        {
            this._arraySize = arraySize;
            this._forward = forward;
            this.cosarray = new double[arraySize];
            this.sinarray = new double[arraySize];

            double sign = 1.0;
            if (forward)
            {
                sign = -1.0;
            }

            double phase0 = 2.0 * Pi / arraySize;
            for (int i = 0; i <= arraySize - 1; i++)
            {
                this.sinarray[i] = sign * Math.Sin(phase0 * i);
                this.cosarray[i] = Math.Cos(phase0 * i);
            }

            int j = this._arraySize;
            while (j != 1)
            {
                this._ldArraysize++;
                j /= 2;
            }
        }

        /// <summary>
        /// The magnitude spectrum.
        /// </summary>
        /// <param name="real">
        /// The real.
        /// </param>
        /// <param name="imag">
        /// The imag.
        /// </param>
        /// <param name="w0">
        /// The w 0.
        /// </param>
        /// <param name="magnitude">
        /// The magnitude.
        /// </param>
        public void MagnitudeSpectrum(double[] real, double[] imag, double w0, double[] magnitude)
        {
            int i;
            magnitude[0] = Math.Sqrt(SquareSum(real[0], imag[0]));
            for (i = 1; i <= (this._arraySize / 2 - 1); i++)
            {
                magnitude[i] = Math.Sqrt(SquareSum(real[i], imag[i]) + SquareSum(real[this._arraySize - i], imag[this._arraySize - i])) / w0;
            }
        }

        /// <summary>
        /// The hanning.
        /// </summary>
        /// <param name="n">
        /// The n.
        /// </param>
        /// <param name="j">
        /// The j.
        /// </param>
        /// <returns>
        /// The <see cref="double"/>.
        /// </returns>
        public static double Hanning(int n, int j)
        {
            return W0Hanning - 0.5 * Math.Cos(2.0 * Pi * j / n);
        }

        /// <summary>
        /// The hamming.
        /// </summary>
        /// <param name="n">
        /// The n.
        /// </param>
        /// <param name="j">
        /// The j.
        /// </param>
        /// <returns>
        /// The <see cref="double"/>.
        /// </returns>
        public static double Hamming(int n, int j)
        {
            return W0Hamming - 0.46 * Math.Cos(2.0 * Pi * j / n);
        }

        /// <summary>
        /// The blackman.
        /// </summary>
        /// <param name="n">
        /// The n.
        /// </param>
        /// <param name="j">
        /// The j.
        /// </param>
        /// <returns>
        /// The <see cref="double"/>.
        /// </returns>
        public static double Blackman(int n, int j)
        {
            return W0Blackman - 0.5 * Math.Cos(2.0 * Pi * j / n) + 0.08 * Math.Cos(4.0 * Pi * j / n);
        }

        /// <summary>
        /// The swap.
        /// </summary>
        /// <param name="a">
        /// The a.
        /// </param>
        /// <param name="b">
        /// The b.
        /// </param>
        private static void Swap(ref double a, ref double b)
        {
            double temp = a;
            a = b;
            b = temp;
        }

        /// <summary>
        /// The square sum.
        /// </summary>
        /// <param name="a">
        /// The a.
        /// </param>
        /// <param name="b">
        /// The b.
        /// </param>
        /// <returns>
        /// The <see cref="double"/>.
        /// </returns>
        private static double SquareSum(double a, double b)
        {
            return a * a + b * b;
        }

        /// <summary>
        /// The fourier transform.
        /// </summary>
        /// <param name="real">
        /// The real.
        /// </param>
        /// <param name="imag">
        /// The imag.
        /// </param>
        public void FourierTransform(double[] real, double[] imag)
        {
            int i;
            if (this._forward)
            {
                for (i = 0; i <= this._arraySize - 1; i++)
                {
                    real[i] /= this._arraySize;
                    imag[i] /= this._arraySize;
                }
            }

            int k;
            int j = 0;
            for (i = 0; i <= this._arraySize - 2; i++)
            {
                if (i < j)
                {
                    Swap(ref real[i], ref real[j]);
                    Swap(ref imag[i], ref imag[j]);
                }

                k = this._arraySize / 2;
                while (k <= j)
                {
                    j -= k;
                    k /= 2;
                }

                j += k;
            }

            int a = 2;
            int b = 1;
            for (int count = 1; count <= this._ldArraysize; count++)
            {
                int c0 = this._arraySize / a;
                int c1 = 0;
                for (k = 0; k <= b - 1; k++)
                {
                    i = k;
                    while (i < this._arraySize)
                    {
                        int arg = i + b;
                        double prodreal;
                        double prodimag;
                        if (k == 0)
                        {
                            prodreal = real[arg];
                            prodimag = imag[arg];
                        }
                        else
                        {
                            prodreal = real[arg] * this.cosarray[c1] - imag[arg] * this.sinarray[c1];
                            prodimag = real[arg] * this.sinarray[c1] + imag[arg] * this.cosarray[c1];
                        }

                        real[arg] = real[i] - prodreal;
                        imag[arg] = imag[i] - prodimag;
                        real[i] += prodreal;
                        imag[i] += prodimag;
                        i += a;
                    }

                    c1 += c0;
                }

                a *= 2;
                b *= 2;
            }
        }
    }
}