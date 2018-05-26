using Realms.Sync;
using System;
using System.Threading.Tasks;
using TIKSN.Configuration;
using TIKSN.FileSystem;

namespace TIKSN.Data.Realm
{
    public abstract class RealmUnitOfWorkFactoryBase<TRealmUnitOfWork> : IRealmUnitOfWorkFactory<TRealmUnitOfWork>
        where TRealmUnitOfWork : IUnitOfWork
    {
        private readonly IKnownFolders _knownFolders;
        private readonly IPartialConfiguration<SyncRealmOptions> _syncRealmOptions;
        private User _user;

        protected RealmUnitOfWorkFactoryBase(IPartialConfiguration<SyncRealmOptions> syncRealmOptions, IKnownFolders knownFolders)
        {
            _syncRealmOptions = syncRealmOptions ?? throw new ArgumentNullException(nameof(syncRealmOptions));
            _knownFolders = knownFolders ?? throw new ArgumentNullException(nameof(knownFolders));
        }

        public async Task<TRealmUnitOfWork> CreateAsync()
        {
            var syncRealmOptions = _syncRealmOptions.GetConfiguration();

            var syncConfig = new SyncConfiguration(_user, new Uri(syncRealmOptions.RealmURI), _knownFolders.LocalAppData.GetFileInfo(syncRealmOptions.Path).PhysicalPath);
            if (syncRealmOptions.EnableSSLValidation.HasValue)
                syncConfig.EnableSSLValidation = syncRealmOptions.EnableSSLValidation.Value;
            var realm = await Realms.Realm.GetInstanceAsync(syncConfig);
            return await CreateAsync(realm);
        }

        public async Task LoginAsync(Credentials credentials)
        {
            var syncRealmOptions = _syncRealmOptions.GetConfiguration();

            if (_user != null)
                throw new InvalidOperationException("Please log out first, before another login.");

            _user = await User.LoginAsync(credentials, new Uri(syncRealmOptions.ServerURL)).ConfigureAwait(false);
        }

        public async Task LogoutAsync()
        {
            await _user.LogOutAsync();

            _user = null;
        }

        protected abstract Task<TRealmUnitOfWork> CreateAsync(Realms.Realm realm);
    }
}