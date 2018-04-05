/***************************************************************************************************

Copyright (c) 2018 Intellectual Ventures Property Holdings, LLC (IVPH) All rights reserved.

EMOD is licensed under the Creative Commons Attribution-Noncommercial-ShareAlike 4.0 License.
To view a copy of this license, visit https://creativecommons.org/licenses/by-nc-sa/4.0/legalcode

***************************************************************************************************/

using System;
using System.Linq;
using NUnit.Framework;
using compartments.emod;
using compartments.emodl;

namespace cmsunittests.emodl
{
    [TestFixture, Description("EMODL Loader Tests")]
    class EmodlTests : AssertionHelper
    {
        private static readonly ModelInfo ModelData = EmodlLoader.LoadEMODLFile("resources\\testmodel.emodl");

        [Test]
        public void LocaleInfoTest()
        {
            Console.WriteLine("EMODL Loader::LocaleInfo - testing for 'global' compartment (locale)...");
            var global = ModelData.Locales.First(li => li.Name == "global");
            Expect(global != null);

            Console.WriteLine("EMODL Loader::LocaleInfo - testing for 'solitary' compartment (locale)");
            var solitary = ModelData.Locales.First(li => li.Name == "solitary");
            Expect(solitary != null);
        }

        [Test]
        public void NumericExpressionInfoTest()
        {
            Console.Write("NumericExpressionTreeTest: Checking for expression 'C': ");
            var cee = ModelData.Expressions.First(net => net.Name == "C");
            Expect(cee != null);
            Console.WriteLine(cee);

            Console.Write("NumericExpressionTreeTest: Checking for expression 'h': ");
            var aitch = ModelData.Expressions.First(net => net.Name == "h");
            Expect(aitch != null);
            Console.WriteLine(aitch);
        }

        [Test]
        public void ObservableInfoTest()
        {
            Console.Write("ObservableInfoTest: Looking for observable 'Susceptible': ");
            var susceptible = ModelData.Observables.First(oi => oi.Name == "Susceptible");
            Expect(susceptible != null);
            Console.WriteLine(susceptible);

            Console.Write("ObservableInfoTest: Looking for observable 'RecoverFast': ");
            var recoverFast = ModelData.Observables.First(oi => oi.Name == "RecoverFast");
            Expect(recoverFast != null);
            Console.WriteLine(recoverFast);
        }

        [Test]
        public void ParameterInfoTest()
        {
            Console.Write("ParameterInfoTest: Looking for parameter 'delta': ");
            var delta = ModelData.Parameters.First(pi => pi.Name == "delta");
            Expect(delta.Value == 0.0001);
            Console.WriteLine(delta);

            Console.Write("ParameterInfoTest: Looking for parameter 'n': ");
            var n = ModelData.Parameters.First(pi => pi.Name == "n");
            Expect(n.Value == 10.0);
            Console.WriteLine(n);
        }

        [Test]
        public void ReactionInfoTest()
        {
            Console.WriteLine("ReactionInfoTest: Looking for reaction 'deathX1': ");
            var deathX1 = ModelData.Reactions.First(ri => ri.Name == "deathX1");
            Console.WriteLine(deathX1);
            Console.WriteLine("\tVerifying HasDelay == false...");
            Expect(deathX1.HasDelay == false);
            Console.WriteLine("\tVerifying IsDiffusion == false...");
            Expect(deathX1.IsDiffusion == false);

            Console.WriteLine("\tVerifying one species in reactant list...");
            Expect(deathX1.Reactants.Count() == 1);
            var reactant = deathX1.Reactants.First();
            Console.WriteLine("\tVerifying reactant locale (global)...");
            var global = ModelData.Locales.First(li => li.Name == "global");
            Expect(reactant.Locale == global);
            Console.WriteLine("\tVerifying reactant name (X1)...");
            Expect(reactant.Name == "X1");

            Console.WriteLine("\tVerifying empty product list...");
            Expect(!deathX1.Products.Any());    // verifies that the Products list is empty


            Console.WriteLine("ReactionInfoTest: Looking for reaction 'infectX1': ");
            var infectX1 = ModelData.Reactions.First(ri => ri.Name == "infectX1");
            Console.WriteLine("\tVerifying HasDelay (true)...");
            Expect(infectX1.HasDelay);
            Console.WriteLine("\tVerifying IsDiffusion == false...");
            Expect(infectX1.IsDiffusion == false);

            Console.WriteLine("\tVerifying one species in reactant list...");
            Expect(infectX1.Reactants.Count() == 1);
            var x1 = infectX1.Reactants.First();
            Console.WriteLine("\tVerifying reactant locale (global)...");
            Expect(x1.Locale == global);
            Console.WriteLine("\tVerifying reactant name (X1)...");
            Expect(x1.Name == "X1");

            Console.WriteLine("\tVerifying one species in product list...");
            Expect(infectX1.Products.Count() == 1);
            var y1 = infectX1.Products.First();
            Console.WriteLine("\tVerifying reactant locale (global)...");
            Expect(y1.Locale == global);
            Console.WriteLine("\tVerifying reactant name (Y1)...");
            Expect(y1.Name == "Y1");


            Console.WriteLine("ReactionInfoTest: Looking for reaction 'birth': ");
            var birth = ModelData.Reactions.First(ri => ri.Name == "birth");
            Console.WriteLine("\tVerifying HasDelay == false...");
            Expect(birth.HasDelay == false);
            Console.WriteLine("\tVerifying IsLocal (true)...");
            Expect(birth.IsLocal);

            Console.WriteLine("\tVerifying empty reactant list...");
            Expect(!birth.Reactants.Any()); // verifies that the Reactants list is empty

            Console.WriteLine("\tVerifying one species in product list...");
            Expect(birth.Products.Count() == 1);
            var x1Too = birth.Products.First();
            Console.WriteLine("\tVerifying product locale (global)...");
            Expect(x1Too.Locale == global);
            Console.WriteLine("\tVerifying product name (X1)...");
            Expect(x1Too.Name == "X1");
        }

        [Test]
        public void SpeciesInfoTest()
        {
            Console.Write("SpeciesInfoTest: Checking for species 'Y2': ");
            var y2 = ModelData.Species.First(si => si.Name == "Y2");
            Console.WriteLine(y2);
            var global = ModelData.Locales.First(li => li.Name == "global");
            Console.WriteLine("\tVerifying locale (global)...");
            Expect(y2.Locale == global);

            Console.Write("SpeciesInfoTest: Checking for species 'solitary': ");
            var solitary = ModelData.Locales.First(li => li.Name == "solitary");
            Console.WriteLine(solitary);
            var loner = ModelData.Species.First(si => si.Locale == solitary);
            Console.WriteLine("\tVerifying locale (loner)...");
            Expect(loner.Name == "loner");
        }
    }
}
