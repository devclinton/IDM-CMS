/***************************************************************************************************

Copyright (c) 2018 Intellectual Ventures Property Holdings, LLC (IVPH) All rights reserved.

EMOD is licensed under the Creative Commons Attribution-Noncommercial-ShareAlike 4.0 License.
To view a copy of this license, visit https://creativecommons.org/licenses/by-nc-sa/4.0/legalcode

***************************************************************************************************/

using System.Collections.Generic;
using NUnit.Framework;
using compartments.emod;
using compartments.emod.interfaces;
using compartments.solvers.solverbase;

namespace cmsunittests.solvers
{
    [TestFixture, Description("SpeciesMP Tests")]
    class SpeciesMPTests : AssertionHelper
    {
        [Test]
        public void TestSpeciesMPConstructorWithDefaults()
        {
            //Parameters in order to create a SpeciesMP
            const string speciesName = "species1";
            const int initialPopulation = 10;
            
            //Species Info and Species Object
            var info1 = new SpeciesDescription(speciesName, initialPopulation);
            var species = new SpeciesMP(info1, new Dictionary<string, IValue>());
            species.Reset();

            //Test Constructor
            Assert.AreEqual(initialPopulation, species.Count);
            Assert.AreEqual(initialPopulation, (int)species.Value);
        }

        [Test]
        public void TestSpeciesMPCount()
        {
            //Parameters in order to create a SpeciesMP
            SpeciesDescription info1;
            string speciesName = "species1";
            int initialPopulation = 10;

            //Species Info Object
            info1 = new SpeciesDescription(speciesName, initialPopulation);
            var species = new SpeciesMP(info1, new Dictionary<string, IValue>());
            species.Reset();

            //Test get
            Assert.AreEqual(initialPopulation, species.Count);

            //Test set
            species.Count = 20;
            Assert.AreEqual(20, species.Count);
            Assert.AreEqual((double)20, species.Value);

            //Test fractional value of species.
            species.Value = 30.2;
            Assert.AreEqual((int)30.2, species.Count);

            //Test get through Update.
            species.Update(30.0);
            Assert.AreEqual(30,species.Count);
        }

        [Test]
        public void TestSpeciesMPValue()
        {
            //Parameters in order to create a SpeciesMP
            SpeciesDescription info1;
            string speciesName = "species1";
            int initialPopulation = 10;

            //Species Info Object
            info1 = new SpeciesDescription(speciesName, initialPopulation);
            var species = new SpeciesMP(info1, new Dictionary<string, IValue>());
            species.Reset();

            //Test get
            Assert.AreEqual((double)initialPopulation, species.Value);

            //Test set
            species.Value = 20.0;
            Assert.AreEqual(20.0, species.Value);
            species.Value = 21.5;
            Assert.AreEqual(21.5, species.Value);

            //Test set via Update
            species.Update(30.0);
            Assert.AreEqual(30.0, species.Value);
        }

        [Test]
        public void TestSpeciesMPUpdate()
        {
            //Parameters in order to create a SpeciesMP
            SpeciesDescription info1;
            string speciesName = "species1";
            int initialPopulation = 10;

            //Species Info Object
            info1 = new SpeciesDescription(speciesName, initialPopulation);
            var species = new SpeciesMP(info1, new Dictionary<string, IValue>());
            species.Reset();

            //Test Update
            species.Update(30.0);
            Assert.AreEqual(30.0, species.Value);

            species.Update(0.0);
            Assert.AreEqual(0.0, species.Value);

            species.Update(-30.0);
            Assert.AreEqual(-30.0, species.Value);
        }

        [Test]
        public void TestSpeciesMPIncrement()
        {
            //Parameters in order to create a SpeciesMP
            SpeciesDescription info1;
            string speciesName = "species1";
            int initialPopulation = 20;

            //Species Info Object
            info1 = new SpeciesDescription(speciesName, initialPopulation);
            var species = new SpeciesMP(info1, new Dictionary<string, IValue>());
            species.Reset();

            //Test Increment
            Assert.AreEqual(initialPopulation, species.Count);
            
            int returnCount;
            double returnValue;
            int deltaInt = 5;
            var deltaDouble = (double)deltaInt;

            returnCount = species.Increment();
            Assert.AreEqual(initialPopulation + 1, species.Count);
            Assert.AreEqual(initialPopulation + 1, returnCount);

            returnCount = ((Species)species).Increment(deltaInt);
            Assert.AreEqual(initialPopulation + 6, species.Count);
            Assert.AreEqual(initialPopulation + 6, returnCount);

            returnValue = species.Increment(deltaDouble);
            Assert.AreEqual(initialPopulation + 11, species.Value);
            Assert.AreEqual(initialPopulation + 11, returnValue);

            deltaInt = -5;
            deltaDouble = (double)deltaInt;

            returnCount = ((Species)species).Increment(deltaInt);
            Assert.AreEqual(initialPopulation + 6, species.Count);
            Assert.AreEqual(initialPopulation + 6, returnCount);

            returnValue = species.Increment(deltaDouble);
            Assert.AreEqual(initialPopulation + 1, species.Value);
            Assert.AreEqual(initialPopulation + 1, returnValue); 
        }

        [Test]
        public void TestSpeciesMPDecrement()
        {
            //Parameters in order to create a SpeciesMP
            SpeciesDescription info1;
            string speciesName = "species1";
            int initialPopulation = 20;

            //Species Info Object
            info1 = new SpeciesDescription(speciesName, initialPopulation);
            var species = new SpeciesMP(info1, new Dictionary<string, IValue>());
            species.Reset();

            //Test Increment
            Assert.AreEqual(initialPopulation, species.Count);

            int returnCount;
            double returnValue;
            int deltaInt = 5;
            double deltaDouble = 5.0;

            returnCount = species.Decrement();
            Assert.AreEqual(initialPopulation - 1, species.Count);
            Assert.AreEqual(initialPopulation - 1, returnCount);

            returnCount = ((Species)species).Decrement(deltaInt);
            Assert.AreEqual(initialPopulation - 6, species.Count);
            Assert.AreEqual(initialPopulation - 6, returnCount);

            returnValue = species.Decrement(deltaDouble);
            Assert.AreEqual(initialPopulation - 11, species.Value);
            Assert.AreEqual(initialPopulation - 11, returnValue);

            deltaInt = -5;
            deltaDouble = -5.0;

            returnCount = ((Species)species).Decrement(deltaInt);
            Assert.AreEqual(initialPopulation - 6, species.Count);
            Assert.AreEqual(initialPopulation - 6, returnCount);

            returnValue = species.Decrement(deltaDouble);
            Assert.AreEqual(initialPopulation - 1, species.Value);
            Assert.AreEqual(initialPopulation - 1, returnValue);
        }
    }
}
