using TIKSN.Serialization;

namespace TIKSN.Web.Rest
{
    public interface ISerializerRestFactory
    {
        ISerializer<string> Create(string mediaType);
    }
}
