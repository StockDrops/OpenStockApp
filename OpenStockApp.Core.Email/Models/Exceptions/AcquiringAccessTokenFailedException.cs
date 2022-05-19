using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace StockDrops.Core.Models.Backend.Integrations
{
    public class AcquiringAccessTokenFailedException : Exception
    {
        public AcquiringAccessTokenFailedException()
        {
        }

        public AcquiringAccessTokenFailedException(string message) : base(message)
        {
        }

        public AcquiringAccessTokenFailedException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected AcquiringAccessTokenFailedException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
