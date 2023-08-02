namespace MapGen.Command
{
    public interface ICommand
    {
        void Execute();

        void Undo();
    }
}