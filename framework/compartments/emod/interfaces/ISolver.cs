namespace compartments.emod.interfaces
{
    public interface ISolver
    {
        void Solve();
        void OutputData(string prefix);
        string[] GetTrajectoryLabels();
        float[][] GetTrajectoryData();
    }
}
