using System;

namespace Exceptions
{
    public class SwapIsNotLegalException : Exception
    {
        public override string Message => "Tiles swap is not legal";
    }
}