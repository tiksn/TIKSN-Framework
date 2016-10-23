using TIKSN.Serialization;

namespace TIKSN.Web.Rest
{
    public interface IDeserializerRestFactory
    {
        IDeserializer Create(string mediaType);
    }
}
