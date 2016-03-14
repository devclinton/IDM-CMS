using compartments.emod;

namespace compartments.solvers.solverbase
{
    public interface IModelBuilder
    {
        IModel BuildModel(ModelInfo modelInfo);
    }
}
