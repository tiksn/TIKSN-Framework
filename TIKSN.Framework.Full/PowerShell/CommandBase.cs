using Nito.AsyncEx;
using System;
using System.Diagnostics;
using System.Management.Automation;
using System.Threading.Tasks;

namespace TIKSN.PowerShell
{
    public abstract class CommandBase : PSCmdlet
    {
        //private Lazy<SpeechSynthesizer> lazySpeechSynthesizer = new Lazy<SpeechSynthesizer>(() => new SpeechSynthesizer());
        private Stopwatch stopwatch;

        protected IServiceProvider ServiceProvider { get; private set; }

        //public void WriteVerboseAndVocal(string text)
        //{
        //    this.WriteVerbose(text);
        //    this.WriteVocal(text);
        //}

        //public void WriteVocal(string text)
        //{
        //    if (this.Vocal.IsPresent)
        //    {
        //        this.lazySpeechSynthesizer.Value.SpeakAsync(text);
        //    }
        //}

        protected override void BeginProcessing()
        {
            base.BeginProcessing();
            this.WriteVerbose(string.Format("Command started at {0}.", DateTime.Now.ToLongTimeString()));
            this.stopwatch = Stopwatch.StartNew();

            ServiceProvider = CreateServiceProvider(this);

            //if (!string.IsNullOrEmpty(this.Language))
            //{
            //    var contentCulture = new CultureInfo(this.Language);

            //    Thread.CurrentThread.CurrentCulture = contentCulture;
            //    Thread.CurrentThread.CurrentUICulture = contentCulture;
            //}
        }

        protected abstract IServiceProvider CreateServiceProvider(CommandBase command);

        protected override void EndProcessing()
        {
            this.stopwatch.Stop();
            this.WriteVerbose(string.Format("Command finished at {0}. It took {1} to complete.", DateTime.Now.ToLongTimeString(), this.stopwatch.Elapsed));
            base.EndProcessing();
        }

        protected sealed override void ProcessRecord()
        {
            AsyncContext.Run(async () => await ProcessRecordAsync());
        }

        protected virtual Task ProcessRecordAsync()
        {
            return Task.Delay(0);
        }
    }
}
