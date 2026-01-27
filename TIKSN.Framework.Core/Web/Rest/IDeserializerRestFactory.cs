using TIKSN.Serialization;

namespace TIKSN.Web.Rest;

public interface IDeserializerRestFactory
{
    public IDeserializer<string> Create(string mediaType);
}
