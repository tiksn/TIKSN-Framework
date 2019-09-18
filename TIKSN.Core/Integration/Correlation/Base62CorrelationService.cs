using Microsoft.Extensions.Options;
using System;

namespace TIKSN.Integration.Correlation
{
    public class Base62CorrelationService
    {
        private readonly Random _random;
        private readonly IOptions<Base62CorrelationServiceOptions> _base62CorrelationServiceOptions;

        public Base62CorrelationService(Random random, IOptions<Base62CorrelationServiceOptions> base62CorrelationServiceOptions)
        {
            _random = random ?? throw new ArgumentNullException(nameof(random));
            _base62CorrelationServiceOptions = base62CorrelationServiceOptions ?? throw new ArgumentNullException(nameof(base62CorrelationServiceOptions));
        }
    }
}