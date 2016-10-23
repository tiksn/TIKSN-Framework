using System;

namespace TIKSN.Web.Rest
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class RestContentAttribute : Attribute
    {
    }
}