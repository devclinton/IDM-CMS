using System;
using compartments.emod.interfaces;
using compartments.solvers;

namespace compartments.emod.utils
{
    public class SolverFactory
    {
        public static ISolver CreateSolver(string solverName, ModelInfo model, int repeats, double duration, int samples)
        {
            ISolver solver;

            switch (solverName.ToUpper())
            {
                /*
                 * SSA and variants
                 */
                case "SSA":
                case "GILLESPIE":
                case "GILLESPIEDIRECT":
                    solver = new Gillespie(model, duration, repeats, samples);
                    break;

                case "FIRST":
                case "FIRSTREACTION":
                case "GILLESPIEFIRSTREACTION":
                    solver = new GillespieFirstReaction(model, duration, repeats, samples);
                    break;

                case "NEXT":
                case "NEXTREACTION":
                case "GIBSONBRUCK":
                    solver = new GibsonBruck(model, duration, repeats, samples);
                    break;

                case "HYBRID":
                    solver = new HybridSSA(model, duration, repeats, samples);
                    break;

                /*
                 * Leaping solvers
                 */
                case "TAU":
                case "TAULEAPING":
                    solver = new TauLeaping(model, duration, repeats, samples);
                    break;

                case "MID":
                case "MP":
                case "MIDPOINT":
                    solver = new MidPoint(model, duration, repeats, samples);
                    break;

                case "R":
                case "RLEAPING":
                    solver = new RLeaping(model, duration, repeats, samples);
                    break;

                case "RF":
                case "RFAST":
                case "RLEAPINGFAST":
                    solver = new RLeapingFast(model, duration, repeats, samples);
                    break;

                case "BLEAP":
                case "BLEAPING":
                case "B":
                    solver = new BLeaping(model, duration, repeats, samples);
                    break;

                /*
                 * Diffusion/migration solvers
                 */
                case "TSSA":
                case "TRANSPORTSSA":
                case "DIFFUSIONSSA":
                case "ISSA":
                    solver = new TransportSSA(model, duration, repeats, samples);
                    break;

                case "DFSP":
                case "DIFFUSIONFSP":
                case "TRANSPORTFSP":
                    solver = new DFSP(model, duration, repeats, samples);
                    break;

                case "DFSPPRIME":
                case "OTSSA":
                case "OPTIMALTRANSPORTSSA":
                    solver = new OptimalTransportSSA(model, duration, repeats, samples);
                    break;

                case "LEVY":
                case "LEVYFLIGHT":
                case "FRACTIONAL":
                case "FRACTIONALDIFFUSION":
                case "FD":
                    solver = new FractionalDiffusion(model, duration, repeats, samples);
                    break;

                /*
                 * Specialty solvers
                 */
                case "ET":
                case "EXITTIME":
                case "EXITTIMES":
                case "TS":
                case "TIMESTRETCHING":
                    solver = new ExitTimes(model, duration, repeats, samples);
                    break;

                case "DWSSA":
                    solver = new dwSSA(model, duration, repeats, samples);
                    break;

                case "SDWSSA":
                    solver = new sdwSSA(model, duration, repeats, samples);
                    break;

                default:
                    Console.Error.WriteLine("Unknown solver selection '{0}'.", solverName);
                    throw new ArgumentException(string.Format("Unknown solver '{0}'", solverName), "solverName");
            }

            return solver;
        }
    }
}
