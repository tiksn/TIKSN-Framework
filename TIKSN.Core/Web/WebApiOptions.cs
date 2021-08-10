using System;

namespace TIKSN.Web
{
    public class WebApiOptions
    {
        public Guid ApiKey { get; set; }

        public Uri BaseAddress { get; set; }
    }

    public class WebApiOptions<T> : WebApiOptions
    {
    }
}
