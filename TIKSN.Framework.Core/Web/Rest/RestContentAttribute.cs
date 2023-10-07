using System;

namespace TIKSN.Web.Rest
{
    [AttributeUsage(AttributeTargets.Property)]
    public class RestContentAttribute : Attribute
    {
        public RestContentAttribute(string mediaType = "application/json") => this.MediaType = mediaType;

        public string MediaType { get; }
    }
}
