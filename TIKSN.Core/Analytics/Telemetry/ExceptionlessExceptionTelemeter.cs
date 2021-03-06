﻿using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Exceptionless;

namespace TIKSN.Analytics.Telemetry
{
    public class ExceptionlessExceptionTelemeter : ExceptionlessTelemeterBase, IExceptionTelemeter
    {
        public async Task TrackException(Exception exception)
        {
            try
            {
                exception.ToExceptionless().Submit();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }

        public async Task TrackException(Exception exception, TelemetrySeverityLevel severityLevel)
        {
            try
            {
                exception.ToExceptionless().SetType(severityLevel.ToString()).Submit();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }
    }
}
