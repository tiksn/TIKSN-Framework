using TIKSN.Serialization;

namespace TIKSN.Web.Rest
{
    public interface ISerializerRestFactory
    {
        ISerializer Create(string mediaType);
    }
}
