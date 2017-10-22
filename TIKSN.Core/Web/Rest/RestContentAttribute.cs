using System;

namespace TIKSN.Web.Rest
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class RestContentAttribute : Attribute
    {
        public RestContentAttribute(string mediaType = "application/json")
        {
            MediaType = mediaType;
        }

        public string MediaType { get; private set; }
    }
}