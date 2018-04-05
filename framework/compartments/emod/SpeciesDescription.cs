/***************************************************************************************************

Copyright (c) 2018 Intellectual Ventures Property Holdings, LLC (IVPH) All rights reserved.

EMOD is licensed under the Creative Commons Attribution-Noncommercial-ShareAlike 4.0 License.
To view a copy of this license, visit https://creativecommons.org/licenses/by-nc-sa/4.0/legalcode

***************************************************************************************************/

using System;
using compartments.emod.expressions;

namespace compartments.emod
{
    public class SpeciesDescription
    {
        private readonly String _name;
        private readonly LocaleInfo _locale;
        private readonly NumericExpressionTree _initialPopulation;

        public SpeciesDescription(String name)
        {
            _name = name;
            _locale = null;
            _initialPopulation = new NumericExpressionTree(new Constant(0.0));
        }

        public SpeciesDescription(String name, int initialPopulation)
        {
            _name = name;
            _locale = null;
            _initialPopulation = new NumericExpressionTree(new Constant(initialPopulation));
        }

        public SpeciesDescription(String name, NumericExpressionTree initialPopulation)
        {
            _name = name;
            _locale = null;
            _initialPopulation = initialPopulation;
        }

        public SpeciesDescription(String name, LocaleInfo locale)
        {
            _name = name;
            _locale = locale;
            _initialPopulation = new NumericExpressionTree(new Constant(0.0));
        }

        public SpeciesDescription(String name, NumericExpressionTree initialPopulation, LocaleInfo locale)
        {
            _name = name;
            _locale = locale;
            _initialPopulation = initialPopulation;
        }

        public SpeciesDescription(String name, int initialPopulation, LocaleInfo locale)
        {
            _name = name;
            _locale = locale;
            _initialPopulation = new NumericExpressionTree(new Constant(initialPopulation));
        }

        public NumericExpressionTree InitialPopulation
        {
            get { return _initialPopulation; }
        }

        public LocaleInfo Locale
        {
            get { return _locale; }
        }

        public String Name
        {
            get { return _name; }
        }

        public override string ToString()
        {
            return string.Format("(species {0} {1})", Name, _initialPopulation);
        }
    }
}
