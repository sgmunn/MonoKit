namespace MonoKit
{
    using System;
    
    public interface ICommand
    {
        void Execute();
        bool GetCanExecute();
    }
}
