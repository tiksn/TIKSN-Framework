using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Realms.Sync;
using TIKSN.Settings;

namespace TIKSN.Data.Realm
{
    public class RoamingRealmCredentialsProvider : IRealmCredentialsProvider
    {
        public const string ProviderName = "Roaming-Realm-Credentials-Provider";
        public const string IdentitySettingName = "Roaming-Realm-Credentials-Identity";

        private readonly ISettingsService _settingsService;

        public RoamingRealmCredentialsProvider(ISettingsService settingsService)
        {
            _settingsService = settingsService ?? throw new ArgumentNullException(nameof(settingsService));
        }

        public Task<Credentials> GetCredentialsAsync(CancellationToken cancellationToken)
        {
            var identity = _settingsService.GetRoamingSetting(IdentitySettingName, string.Empty);

            if (identity.Length == 0)
            {
                identity = Guid.NewGuid().ToString();

                _settingsService.SetRoamingSetting(IdentitySettingName, identity);
            }

            var userInfo = new Dictionary<string, object>();

            return Task.FromResult(Credentials.Custom(ProviderName, identity, userInfo));
        }
    }
}
