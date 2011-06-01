using System;

namespace SharpVectors.Converters
{
    public interface IObserver
    {
        void OnStarted(IObservable sender);
        void OnCompleted(IObservable sender, bool isSuccessful);
    }
}
