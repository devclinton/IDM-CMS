/***************************************************************************************************

Copyright (c) 2018 Intellectual Ventures Property Holdings, LLC (IVPH) All rights reserved.

EMOD is licensed under the Creative Commons Attribution-Noncommercial-ShareAlike 4.0 License.
To view a copy of this license, visit https://creativecommons.org/licenses/by-nc-sa/4.0/legalcode

***************************************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace compartments.emod
{
    public class ReactionInfo
    {
        private readonly String _name;
        private readonly List<SpeciesDescription> _reactants;
        private readonly List<SpeciesDescription> _products;
        private NumericExpressionTree _rate;
        private NumericExpressionTree _delay;

        public class ReactionBuilder
        {
            private readonly ReactionInfo _reaction;

            public ReactionBuilder()
            {
                _reaction = new ReactionInfo();
            }

            public ReactionBuilder(String name)
            {
                _reaction = new ReactionInfo(name);
            }

            public void AddReactant(SpeciesDescription si)
            {
                if (_reaction.Reactants.Any(r => r.Locale != si.Locale))
                    throw new ArgumentException("All reactants of a reaction must come from the same locale.");

                _reaction._reactants.Add(si);
            }

            public void AddProduct(SpeciesDescription si)
            {
                if (_reaction.Products.Any(p => p.Locale != si.Locale))
                    throw new ArgumentException("All products of a reaction must come from the same locale.");

                _reaction._products.Add(si);
            }

            public void SetRate(NumericExpressionTree expressionTree)
            {
                _reaction._rate = expressionTree;
            }

            public void SetDelay(NumericExpressionTree expressionTree)
            {
                _reaction._delay = expressionTree;
            }

            public ReactionInfo Reaction
            {
                get { return _reaction; }
            }
        }

        protected ReactionInfo()
        {
            _name = String.Empty;
            _reactants = new List<SpeciesDescription>();
            _products = new List<SpeciesDescription>();
            _rate = null;
            _delay = null;
        }

        protected ReactionInfo(String name)
        {
            _name = name;
            _reactants = new List<SpeciesDescription>();
            _products = new List<SpeciesDescription>();
            _rate = null;
            _delay = null;
        }

        public String Name
        {
            get { return _name; }
        }

        public IEnumerable<SpeciesDescription> Reactants
        {
            get { return _reactants; }
        }

        public IEnumerable<SpeciesDescription> Products
        {
            get { return _products; }
        }

        public bool IsDiffusion
        {
            get
            {
                bool isDiffusion = false;

                if ((_reactants.Count > 0) && (_products.Count > 0))
                    isDiffusion = (_products.First().Locale != _reactants.First().Locale);

                return isDiffusion;
            }
        }

        public bool IsLocal
        {
            get { return !IsDiffusion; }
        }

        public LocaleInfo Locale
        {
            get { return _reactants.Any() ? _reactants.First().Locale : _products.First().Locale; }
        }

        public NumericExpressionTree RateExpression
        {
            get { return _rate; }
        }

        public NumericExpressionTree DelayExpression
        {
            get { return _delay; }
        }

        public bool HasDelay
        {
            get { return _delay != null; }
        }

        public override string ToString()
        {
            var sb = new StringBuilder(string.Format("(reaction {0} (", Name));

            foreach (SpeciesDescription si in _reactants)
            {
                sb.Append(si.Name);
                sb.Append(' ');
            }
            sb.Remove(sb.Length - 1, 1);

            sb.Append(") (");

            foreach (SpeciesDescription si in _products)
            {
                sb.Append(si.Name);
                sb.Append(' ');
            }
            sb.Remove(sb.Length - 1, 1);
            sb.Append(") ");

            sb.Append(_rate != null ? _rate.ToString() : "???");

            if (HasDelay)
            {
                sb.AppendFormat(" {0}", _delay);
            }

            sb.Append(')');

            return sb.ToString();
        }
    }
}
