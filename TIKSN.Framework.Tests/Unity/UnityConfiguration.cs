using Microsoft.Practices.Unity;
using System;
using TIKSN.Analytics.Telemetry;

namespace TIKSN.Framework.Tests.Unity
{
	public static class UnityConfiguration
	{
		private static readonly Lazy<IUnityContainer> lazyUnityContainer = new Lazy<IUnityContainer>(CreateContainer);

		public static T Resolve<T>()
		{
			return lazyUnityContainer.Value.Resolve<T>();
		}

		private static IUnityContainer CreateContainer()
		{
			var container = new UnityContainer();

			RegisterContainerControlledType<IExceptionTelemeter, PushalotExceptionTelemeter>("Pushalot");
			RegisterContainerControlledType<IExceptionTelemeter, CompositeExceptionTelemeter>();

			return container;
		}

		private static void RegisterContainerControlledType<TFrom, TTo>(string name = null) where TTo : TFrom
		{
			if (string.IsNullOrEmpty(name))
			{
				lazyUnityContainer.Value.RegisterType<TFrom, TTo>(new ContainerControlledLifetimeManager());
			}
			else
			{
				lazyUnityContainer.Value.RegisterType<TFrom, TTo>(name, new ContainerControlledLifetimeManager());
			}
		}

		private static void RegisterPerResolveType<TFrom, TTo>() where TTo : TFrom
		{
			lazyUnityContainer.Value.RegisterType<TFrom, TTo>(new PerResolveLifetimeManager());
		}
	}
}