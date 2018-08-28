using System;

namespace SharpVectors.Converters
{
    public abstract class ConsoleConverter : IObservable
    {
        #region Private Fields

        private string _outputDir;
        private ConverterOptions _options;

        #endregion

        #region Constructors and Destructor

        protected ConsoleConverter()
        {
        }

        #endregion

        #region Public Propeties

        public string OutputDir
        {
            get
            {
                return _outputDir;
            }
            set
            {
                _outputDir = value;
            }
        }

        public ConverterOptions Options
        {
            get
            {
                return _options;
            }
            set
            {
                _options = value;
            }
        }

        #endregion

        #region Public Methods

        public abstract bool Convert(ConsoleWriter writer);

        #endregion

        #region IObservable Members

        public abstract void Cancel();

        public abstract void Subscribe(IObserver observer);

        #endregion
    }
}
