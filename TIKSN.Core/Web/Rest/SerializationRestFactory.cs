using System;
using System.Collections.Generic;
using TIKSN.Serialization;

namespace TIKSN.Web.Rest
{
    public class SerializationRestFactory : ISerializerRestFactory, IDeserializerRestFactory
    {
        private readonly IDictionary<string, Tuple<ISerializer, IDeserializer>> map;

        public SerializationRestFactory(JsonSerializer jsonSerializer, JsonDeserializer jsonDeserializer, DotNetXmlSerializer dotNetXmlSerializer, DotNetXmlDeserializer dotNetXmlDeserializer)
        {
            map = new Dictionary<string, Tuple<ISerializer, IDeserializer>>();
            map.Add("application/json", new Tuple<ISerializer, IDeserializer>(jsonSerializer, jsonDeserializer));
            map.Add("application/xml", new Tuple<ISerializer, IDeserializer>(dotNetXmlSerializer, dotNetXmlDeserializer));
        }

        IDeserializer IDeserializerRestFactory.Create(string mediaType)
        {
            return map[mediaType].Item2;
        }

        ISerializer ISerializerRestFactory.Create(string mediaType)
        {
            return map[mediaType].Item1;
        }
    }
}