using System;
namespace ApiExceptions
{
    public class ApiDecodeException : System.ApplicationException
    {

        public ApiDecodeException()
            : base("Failed reading from API")
        {
        }

        public ApiDecodeException(Exception innerException)
            : base("Failed reading from API", innerException)
        {
        }

    }
}
