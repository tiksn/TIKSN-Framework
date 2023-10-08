using Autofac;
using TIKSN.Data.Mongo;
using TIKSN.Licensing;
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

            _ = builder.RegisterGeneric(typeof(LicenseFactory<,>))
                .As(typeof(ILicenseFactory<,>))
                .SingleInstance();

            _ = builder.RegisterType<RSACertificateSignatureService>()
                .Named<ICertificateSignatureService>("1.2.840.113549.1.1.1")
                .SingleInstance();

            _ = builder.RegisterType<DSACertificateSignatureService>()
                .Named<ICertificateSignatureService>("1.2.840.10040.4.1")
                .SingleInstance();

            _ = builder
                .RegisterType<Finance.ForeignExchange.EntityFrameworkCore.DataEntityMapper>()
                .AsImplementedInterfaces()
                .SingleInstance();

            _ = builder
                .RegisterType<Finance.ForeignExchange.LiteDB.DataEntityMapper>()
                .AsImplementedInterfaces()
                .SingleInstance();

            _ = builder
                .RegisterType<Finance.ForeignExchange.Mongo.DataEntityMapper>()
                .AsImplementedInterfaces()
                .SingleInstance();
        }
    }
}
