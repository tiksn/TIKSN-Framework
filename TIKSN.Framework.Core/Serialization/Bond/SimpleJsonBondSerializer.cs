using Bond.Protocols;

namespace TIKSN.Serialization.Bond;

public class SimpleJsonBondSerializer : SerializerBase<string>
{
    protected override string SerializeInternal<T>(T obj)
    {
        using var output = new StringWriter();
        var writer = new SimpleJsonWriter(output);

        global::Bond.Serialize.To(writer, obj);

        writer.Flush();

        return output.GetStringBuilder().ToString();
    }
}
