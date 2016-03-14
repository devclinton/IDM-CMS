using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using compartments.emod.interfaces;
using compartments.emod.utils;
using distlib.randomvariates;

namespace compartments.emod.distributions
{
    public class Empirical : INumericOperator, IValue
    {
        private int _binCount;
        private float[] _binEdges;
        private float[] _probabilities;
        private float _sum;
        private readonly RandomVariateGenerator _rng;

        private Empirical()
        {
            Dimensionality = 0;
            _binCount       = 0;
            _binEdges       = null;
            _probabilities  = null;
            _sum            = 0;

            _rng = RNGFactory.GetRNG();
        }

        private enum Stage
        {
            ReadDimensionality,
            ReadBinCounts,
            ReadBinEdges,
            ReadProbabilities,
            Finished
        }

        public static Empirical FromFile(string fileName)
        {
            var empirical = new Empirical();

            using (var reader = new StreamReader(fileName))
            {
                var stage = Stage.ReadDimensionality;
                int binIndex = 0;

                while (stage != Stage.Finished)
                {
                    string input = reader.ReadLine();

                    if (input != null)
                    {
                        input = input.Split(new[] { '#' })[0];
                        input = input.Trim();

                        if (input != string.Empty)
                        {
                            switch (stage)
                            {
                                case Stage.ReadDimensionality:
                                    /* NOT YET
                                    empirical.Dimensionality = Int32.Parse(input);
                                    */
                                    stage = Stage.ReadBinCounts;
                                    break;

                                case Stage.ReadBinCounts:
                                    empirical.BinCount = Int32.Parse(input);
                                    stage = Stage.ReadBinEdges;
                                    break;

                                case Stage.ReadBinEdges:
                                    {
                                        string[] edgeValues = input.Split(new[] { ',' });

                                        if (edgeValues.Length != (empirical.BinCount + 1))
                                            throw new ArgumentException("Incorrect number of bin edges in data file.");

                                        var edges = new float[empirical.BinCount + 1];

                                        for (int iEdge = 0; iEdge <= empirical.BinCount; iEdge++)
                                        {
                                            edges[iEdge] = float.Parse(edgeValues[iEdge]);
                                        }

                                        empirical.BinEdges = edges;

                                        stage = Stage.ReadProbabilities;
                                    }
                                    break;

                                case Stage.ReadProbabilities:
                                    empirical[binIndex] = float.Parse(input);
                                    empirical._sum += empirical[binIndex];
                                    binIndex++;
                                    if (binIndex >= empirical.BinCount)
                                        stage = Stage.Finished;
                                    break;

                                default:
                                    throw new ApplicationException();
                            }
                        }
                    }
                    else
                    {
                        throw new ApplicationException("Badly formed data driven distribution '" + fileName + "'.");
                    }
                }
            }

            return empirical;
        }

        private int Dimensionality { get; set; }

        public int BinCount
        {
            get { return _binCount; }
            private set
            {
                if (value < 1) throw new ArgumentException();

                if (value != _binCount)
                {
                    _binCount      = value;
                    _binEdges      = new float[_binCount + 1];
                    _probabilities = new float[_binCount];
                }
            }
        }

        public float[] BinEdges
        {
            get { return _binEdges; }
            private set
            {
                if (value.Length != _binEdges.Length)
                    throw new ArgumentException();

                value.CopyTo(_binEdges, 0);
            }
        }

        public float this[int index]
        {
            get { return _probabilities[index]; }
            private set { _probabilities[index] = value; }
        }

        public float Value
        {
            get
            {
                float retval = BinEdges[0];
                float u = _rng.GenerateUniformOO() * _sum;

                for (int i = 0; (i < _binCount) && (u > 0.0f); i++)
                {
                    if (u <= _probabilities[i])
                    {
                        retval = _binEdges[i];
                        retval += (_binEdges[i + 1] - _binEdges[i]) * u / _probabilities[i];
                    }

                    u -= _probabilities[i];
                }

                return retval;
            }
        }

        public IValue ResolveReferences(IDictionary<string, IValue> map)
        {
            return this;
        }

        public override string ToString()
        {
            var sb = new StringBuilder(string.Format("(empirical {0} {1} (", Dimensionality, BinCount));

            foreach (var edge in BinEdges)
            {
                sb.Append(edge.ToString(CultureInfo.InvariantCulture));
                sb.Append(' ');
            }
            sb.Remove(sb.Length - 1, 1);

            sb.Append(") (");

            foreach (var value in _probabilities)
            {
                sb.Append(value.ToString(CultureInfo.InvariantCulture));
                sb.Append(' ');
            }
            sb.Remove(sb.Length - 1, 1);

            sb.Append("))");

            return sb.ToString();
        }
    }
}
