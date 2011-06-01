using System;

namespace SharpVectors.Converters
{
    public interface IObservable
    {
        void Cancel();
        void Subscribe(IObserver observer);
    }
}
