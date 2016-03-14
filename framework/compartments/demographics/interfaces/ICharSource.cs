namespace compartments.demographics.interfaces
{
    public interface ICharSource
    {
        bool EndOfFile { get; }
        char Current { get; }
        char Next();
        void Seek(uint offset);
    }
}
