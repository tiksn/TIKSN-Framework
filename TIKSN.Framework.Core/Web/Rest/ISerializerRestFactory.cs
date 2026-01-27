using TIKSN.Serialization;

namespace TIKSN.Web.Rest;

public interface ISerializerRestFactory
{
    public ISerializer<string> Create(string mediaType);
}
