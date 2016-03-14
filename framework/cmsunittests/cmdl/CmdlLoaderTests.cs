using System;
using System.Linq;
using NUnit.Framework;
using compartments.emod;
using compartments.cmdl;

namespace cmsunittests.cmdl
{
    [TestFixture, Description("CMDL loader tests")]
    class CmdlLoaderTests : AssertionHelper
    {
        private static readonly ModelInfo ModelData = CmdlLoader.LoadCMDLFile("resources\\testmodel.cmdl");

        [Test]
        public void LocaleInfoTest()
        {
            Console.WriteLine("CMDL Loader::LocaleInfo - testing for 'universal' compartment (locale)...");
            var univ = ModelData.Locales.First(li => li.Name == "univ");
            Expect(univ != null);

            Console.WriteLine("CMDL Loader::LocaleInfo - testing for 'solitary' compartment (locale)");
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
            Console.Write("ObservableInfoTest: Looking for species X1 as observable: ");
            var x1 = ModelData.Observables.First(oi => oi.Name == "X1");
            Expect(x1 != null);
            Console.WriteLine(x1);

            Console.Write("ObservableInfoTest: Looking for species Y3 as observable: ");
            var y3 = ModelData.Observables.First(oi => oi.Name == "Y3");
            Expect(y3 != null);
            Console.WriteLine(y3);
        }

        [Test]
        public void ParameterInfoTest()
        {
            Console.Write("ParameterInfoTest: Looking for parameter 'delta': ");
            var delta = ModelData.Parameters.First(pi => pi.Name == "delta");
            // ReSharper disable CompareOfFloatsByEqualityOperator
            Expect(delta.Value == 0.0001f);
            // ReSharper restore CompareOfFloatsByEqualityOperator
            Console.WriteLine(delta);

            Console.Write("ParameterInfoTest: Looking for parameter 'n': ");
            var n = ModelData.Parameters.First(pi => pi.Name == "n");
            // ReSharper disable CompareOfFloatsByEqualityOperator
            Expect(n.Value == 10.0f);
            // ReSharper restore CompareOfFloatsByEqualityOperator
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
            Console.WriteLine("\tVerifying reactant locale (univ)...");
            var univ = ModelData.Locales.First(li => li.Name == "univ");
            Expect(reactant.Locale == univ);
            Console.WriteLine("\tVerifying reactant name (X1)...");
            Expect(reactant.Name == "X1");

            Console.WriteLine("\tVerifying empty product list...");
            Expect(!deathX1.Products.Any());    // verifies that the Products list is empty


            Console.WriteLine("ReactionInfoTest: Looking for reaction 'latencyX2': ");
            var latencyX2 = ModelData.Reactions.First(ri => ri.Name == "latencyX2");
            Console.WriteLine("\tVerifying HasDelay (true)...");
            Expect(latencyX2.HasDelay);
            Console.WriteLine("\tVerifying IsDiffusion == false...");
            Expect(latencyX2.IsDiffusion == false);

            Console.WriteLine("\tVerifying one species in reactant list...");
            Expect(latencyX2.Reactants.Count() == 1);
            var x2 = latencyX2.Reactants.First();
            Console.WriteLine("\tVerifying reactant locale (univ)...");
            Expect(x2.Locale == univ);
            Console.WriteLine("\tVerifying reactant name (X2)...");
            Expect(x2.Name == "X2");

            Console.WriteLine("\tVerifying one species in product list...");
            Expect(latencyX2.Products.Count() == 1);
            var y1 = latencyX2.Products.First();
            Console.WriteLine("\tVerifying reactant locale (univ)...");
            Expect(y1.Locale == univ);
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
            var x1 = birth.Products.First();
            Console.WriteLine("\tVerifying product locale (univ)...");
            Expect(x1.Locale == univ);
            Console.WriteLine("\tVerifying product name (X1)...");
            Expect(x1.Name == "X1");
        }

        [Test]
        public void SpeciesInfoTest()
        {
            Console.Write("SpeciesInfoTest: Checking for species 'Y2': ");
            var y2 = ModelData.Species.First(si => si.Name == "Y2");
            Console.WriteLine(y2);
            var univ = ModelData.Locales.First(li => li.Name == "univ");
            Console.WriteLine("\tVerifying locale (univ)...");
            Expect(y2.Locale == univ);

            Console.Write("SpeciesInfoTest: Checking for species 'solitary': ");
            var solitary = ModelData.Locales.First(li => li.Name == "solitary");
            Console.WriteLine(solitary);
            var loner = ModelData.Species.First(si => si.Locale == solitary);
            Console.WriteLine("\tVerifying locale (loner)...");
            Expect(loner.Name == "loner");
        }
    }
}
