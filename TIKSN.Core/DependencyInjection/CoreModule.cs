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
            builder.RegisterType<DotNetXmlDeserializer>().AsSelf().SingleInstance();
            builder.RegisterType<DotNetXmlSerializer>().AsSelf().SingleInstance();
            builder.RegisterType<JsonDeserializer>().AsSelf().SingleInstance();
            builder.RegisterType<JsonSerializer>().AsSelf().SingleInstance();
            builder.RegisterType<RestRequester>().As<IRestRequester>();
            builder.RegisterType<SerializationRestFactory>().As<ISerializerRestFactory>().As<IDeserializerRestFactory>()
                .SingleInstance();

            builder.RegisterType<MongoClientSessionContext>()
                .As<IMongoClientSessionStore>()
                .As<IMongoClientSessionProvider>()
                .InstancePerLifetimeScope();
        }
    }
}
