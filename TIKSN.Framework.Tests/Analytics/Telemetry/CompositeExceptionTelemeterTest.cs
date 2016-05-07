// <copyright file="CompositeExceptionTelemeterTest.cs">Copyright ©  2015</copyright>
using Microsoft.Pex.Framework;
using Microsoft.Pex.Framework.Validation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading.Tasks;

namespace TIKSN.Analytics.Telemetry.Tests
{
	/// <summary>This class contains parameterized unit tests for CompositeExceptionTelemeter</summary>
	[PexClass(typeof(CompositeExceptionTelemeter))]
	[PexAllowedExceptionFromTypeUnderTest(typeof(InvalidOperationException))]
	[PexAllowedExceptionFromTypeUnderTest(typeof(ArgumentException), AcceptExceptionSubtypes = true)]
	[TestClass]
	public partial class CompositeExceptionTelemeterTest
	{
		/// <summary>Test stub for TrackException(Exception)</summary>
		[PexMethod]
		public Task TrackExceptionTest(
			[PexAssumeUnderTest]CompositeExceptionTelemeter target,
			Exception exception
		)
		{
			Task result = target.TrackException(exception);
			return result;
			// TODO: add assertions to method CompositeExceptionTelemeterTest.TrackExceptionTest(CompositeExceptionTelemeter, Exception)
		}

		/// <summary>Test stub for TrackException(Exception, TelemetrySeverityLevel)</summary>
		[PexMethod]
		public Task TrackExceptionTest(
			[PexAssumeUnderTest]CompositeExceptionTelemeter target,
			Exception exception,
			TelemetrySeverityLevel severityLevel
		)
		{
			Task result = target.TrackException(exception, severityLevel);
			return result;
			// TODO: add assertions to method CompositeExceptionTelemeterTest.TrackExceptionTest(CompositeExceptionTelemeter, Exception, TelemetrySeverityLevel)
		}
	}
}
