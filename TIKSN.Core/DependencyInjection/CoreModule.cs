using Autofac;
using TIKSN.Data.Mongo;
using TIKSN.Serialization;
using TIKSN.Web.Rest;

namespace TIKSN.DependencyInjection
{
    public class CoreModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            _ = builder.RegisterType<DotNetXmlDeserializer>().AsSelf().SingleInstance();
            _ = builder.RegisterType<DotNetXmlSerializer>().AsSelf().SingleInstance();
            _ = builder.RegisterType<JsonDeserializer>().AsSelf().SingleInstance();
            _ = builder.RegisterType<JsonSerializer>().AsSelf().SingleInstance();
            _ = builder.RegisterType<RestRequester>().As<IRestRequester>();
            _ = builder.RegisterType<SerializationRestFactory>().As<ISerializerRestFactory>().As<IDeserializerRestFactory>()
                .SingleInstance();

            _ = builder.RegisterType<MongoClientSessionContext>()
                .As<IMongoClientSessionStore>()
                .As<IMongoClientSessionProvider>()
                .InstancePerLifetimeScope();
        }
    }
}
