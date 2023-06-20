using System;

namespace CodraftPlugin_Exceptions
{
    public class FittingDoesNotExistException : Exception
    {
        public FittingDoesNotExistException()
        {
        }

        public FittingDoesNotExistException(string message) : base(message)
        {
        }
    }
}
