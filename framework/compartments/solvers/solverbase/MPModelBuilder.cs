/***************************************************************************************************

Copyright (c) 2018 Intellectual Ventures Property Holdings, LLC (IVPH) All rights reserved.

EMOD is licensed under the Creative Commons Attribution-Noncommercial-ShareAlike 4.0 License.
To view a copy of this license, visit https://creativecommons.org/licenses/by-nc-sa/4.0/legalcode

***************************************************************************************************/

using System.Collections.Generic;
using compartments.emod;

namespace compartments.solvers.solverbase
{
    public class MpModelBuilder : ModelBuilder
    {
        protected override void ProcessSpecies(Model model, ModelInfo modelInfo)
        {
            model.Species = new List<Species>();

            foreach (SpeciesDescription speciesDescription in modelInfo.Species)
            {
                var newSpecies = new SpeciesMP(speciesDescription, _nmap);
                model.Species.Add(newSpecies);
                _nmap.Add(newSpecies.Name, newSpecies);
                _umap.Add(newSpecies.Name, newSpecies);
                _speciesMap.Add(speciesDescription, newSpecies);
            }
        }
    }
}
