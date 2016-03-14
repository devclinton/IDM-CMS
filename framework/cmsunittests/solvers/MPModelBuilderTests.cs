using System;
using System.Collections.Generic;
using System.Reflection;
using NUnit.Framework;
using compartments.emod;
using compartments.emod.interfaces;
using compartments.solvers.solverbase;

namespace cmsunittests.solvers
{
    [TestFixture, Description("MP Model Builder Tests")]
    class MPModelBuilderTests : AssertionHelper
    {
        [Test]
        public void ProcessSpeciesTestOneSpecies()
        {
            ModelInfo modelInfo;
            ModelInfo.ModelBuilder modelInfoEx;
            MPModelBuilder modelbuilder;
            MethodInfo processSpeciesMethod;
            Model model;
            IDictionary<SpeciesDescription, Species> speciesMap;
            IDictionary<string, IValue> nmap;
            IDictionary<string, IBoolean> bmap;
            IDictionary<string, IUpdateable> umap;

            // Need a Model Info, Species Info, modelbuilder
            modelInfoEx = new ModelInfo.ModelBuilder("model");
            var species = new SpeciesDescription("testSpecies", 10); 
            modelbuilder = new MPModelBuilder();

            speciesMap = InitializeMaps(out nmap, out bmap, out umap);
            var newSpecies = new SpeciesMP(species, nmap);

            modelInfoEx.AddSpecies(species);
            modelInfo = modelInfoEx.Model;
            model = new Model(modelInfo);

            processSpeciesMethod = ReflectionUtility.GetHiddenMethod("ProcessSpecies", modelbuilder);
            
            //Inputs to invoke hidden method ProcessSpecies (needs to be an array of objects)
            var inputArray1 = new object[2];
            inputArray1[0] = model;
            inputArray1[1] = modelInfo;

            //Test to make sure that the model has local variables that are null

            var compareSpeciesMap = ReflectionUtility.GetHiddenField<IDictionary<SpeciesDescription, Species>>("_speciesMap", modelbuilder);
            var compareNmap = ReflectionUtility.GetHiddenField<IDictionary<string, IValue>>("_nmap", modelbuilder);
            var compareBmap = ReflectionUtility.GetHiddenField<IDictionary<string, IBoolean>>("_bmap", modelbuilder);
            var compareUMap = ReflectionUtility.GetHiddenField<IDictionary<string, IUpdateable>>("_umap", modelbuilder);

            Assert.AreEqual(speciesMap, compareSpeciesMap);
            Assert.AreEqual(nmap, compareNmap);
            Assert.AreEqual(bmap, compareBmap);
            Assert.AreEqual(umap, compareUMap);

            processSpeciesMethod.Invoke(modelbuilder, inputArray1);

            //Begin Assertions about the species that was added to the model
            Assert.AreEqual("testSpecies", model.Species[0].Name);
            Assert.AreEqual(species, model.Species[0].Description);
            Assert.AreEqual(1, model.Species.Count);
               
            //Assert tests about size.

            compareSpeciesMap = ReflectionUtility.GetHiddenField<IDictionary<SpeciesDescription, Species>>("_speciesMap", modelbuilder);
            compareNmap = ReflectionUtility.GetHiddenField<IDictionary<string, IValue>>("_nmap", modelbuilder);
            compareUMap = ReflectionUtility.GetHiddenField<IDictionary<string, IUpdateable>>("_umap", modelbuilder);

            Assert.AreEqual(1, compareSpeciesMap.Count);
            Assert.AreEqual(1, compareNmap.Count);
            Assert.AreEqual(1, compareUMap.Count);

            //Assert tests about what was actually added to the dictionary. 

             var testSpecies = (SpeciesMP)compareNmap["testSpecies"];
           
            Assert.AreEqual(testSpecies.Description,newSpecies.Description);
            Assert.AreEqual(testSpecies.Locale,newSpecies.Locale);
            Assert.AreEqual(testSpecies.Name,newSpecies.Name);
        }

        [Test]
        public void ProcessSpeciesTestNSpecies()
        {
            ModelInfo modelInfo;
            ModelInfo.ModelBuilder modelInfoEx;
            MPModelBuilder modelbuilder;
            MethodInfo processSpeciesMethod;
            Model model;
            IDictionary<SpeciesDescription, Species> speciesMap;
            IDictionary<string, IValue> nmap;
            IDictionary<string, IBoolean> bmap;
            IDictionary<string, IUpdateable> umap;
            SpeciesDescription[] speciesDescArray;
            SpeciesMP[] speciesArray;

            // Need a Model Info, Species Info, modelbuilder, Species
            modelInfoEx = new ModelInfo.ModelBuilder("model");
            speciesDescArray = new SpeciesDescription[100];
            speciesArray = new SpeciesMP[100];
            modelbuilder = new MPModelBuilder();
            speciesMap = InitializeMaps(out nmap, out bmap, out umap);

            for (var i = 0; i < 100; i++)
            {
                string nameStr = "testSpecies" + i;
                speciesDescArray[i] = new SpeciesDescription(nameStr, i);
                speciesArray[i] = new SpeciesMP(speciesDescArray[i],nmap);
                modelInfoEx.AddSpecies(speciesDescArray[i]);
            }
            
            modelInfo = modelInfoEx.Model;
            model = new Model(modelInfo);

            processSpeciesMethod = ReflectionUtility.GetHiddenMethod("ProcessSpecies", modelbuilder);

            //Inputs to invoke hidden method ProcessSpecies (needs to be an array of objects)
            var inputArray1 = new object[2];
            inputArray1[0] = model;
            inputArray1[1] = modelInfo;

            //Test to make sure that the model has local variables that are null

            var compareSpeciesMap = ReflectionUtility.GetHiddenField<IDictionary<SpeciesDescription, Species>>("_speciesMap", modelbuilder);
            var compareNmap = ReflectionUtility.GetHiddenField<IDictionary<string, IValue>>("_nmap", modelbuilder);
            var compareBmap = ReflectionUtility.GetHiddenField<IDictionary<string, IBoolean>>("_bmap", modelbuilder);
            var compareUMap = ReflectionUtility.GetHiddenField<IDictionary<string, IUpdateable>>("_umap", modelbuilder);

            Assert.AreEqual(speciesMap, compareSpeciesMap);
            Assert.AreEqual(nmap, compareNmap);
            Assert.AreEqual(bmap, compareBmap);
            Assert.AreEqual(umap, compareUMap);

            processSpeciesMethod.Invoke(modelbuilder, inputArray1);

            //Begin Assertions about all of the species that was added to the model
            Assert.AreEqual("testSpecies0", model.Species[0].Name);
            Assert.AreEqual(speciesDescArray[0], model.Species[0].Description);
            Assert.AreEqual(100, model.Species.Count);

            //Assert tests about size.

            compareSpeciesMap = ReflectionUtility.GetHiddenField<IDictionary<SpeciesDescription, Species>>("_speciesMap", modelbuilder);
            compareNmap = ReflectionUtility.GetHiddenField<IDictionary<string, IValue>>("_nmap", modelbuilder);
            compareUMap = ReflectionUtility.GetHiddenField<IDictionary<string, IUpdateable>>("_umap", modelbuilder);

            Assert.AreEqual(100, compareSpeciesMap.Count);
            Assert.AreEqual(100, compareNmap.Count);
            Assert.AreEqual(100, compareUMap.Count);

            //Assert tests about what was actually added to the dictionary. 

            for (var i = 0; i < 100; i++)
            {
                var testSpecies = (SpeciesMP)compareNmap["testSpecies" + i];

                Assert.AreEqual(testSpecies.Description, speciesArray[i].Description);
                Assert.AreEqual(testSpecies.Locale, speciesArray[i].Locale);
                Assert.AreEqual(testSpecies.Name, speciesArray[i].Name);
            }
        }

        private static IDictionary<SpeciesDescription, Species> InitializeMaps(out IDictionary<string, IValue> nmap, out IDictionary<string, IBoolean> bmap, out IDictionary<string, IUpdateable> umap)
        {
            IDictionary<SpeciesDescription, Species> speciesMap;
            speciesMap = new Dictionary<SpeciesDescription, Species>();
            nmap = new Dictionary<string, IValue>();
            bmap = new Dictionary<string, IBoolean>();
            umap = new Dictionary<string, IUpdateable>();
            return speciesMap;
        }
    }
}
