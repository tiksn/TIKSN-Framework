using TIKSN.Serialization;

namespace TIKSN.Web.Rest
{
    public interface IDeserializerRestFactory
    {
        IDeserializer<string> Create(string mediaType);
    }
}
