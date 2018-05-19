﻿using Realms.Sync;
using System;
using System.Threading.Tasks;
using TIKSN.Configuration;
using TIKSN.FileSystem;

namespace TIKSN.Data.Realm
{
    public class RealmUnitOfWorkFactory : IRealmUnitOfWorkFactory
    {
        private readonly IKnownFolders _knownFolders;
        private readonly IPartialConfiguration<SyncRealmOptions> _syncRealmOptions;
        private User _user;

        public RealmUnitOfWorkFactory(IPartialConfiguration<SyncRealmOptions> syncRealmOptions, IKnownFolders knownFolders)
        {
            _syncRealmOptions = syncRealmOptions ?? throw new ArgumentNullException(nameof(syncRealmOptions));
            _knownFolders = knownFolders ?? throw new ArgumentNullException(nameof(knownFolders));
        }

        public async Task<IRealmUnitOfWork> CreateAsync()
        {
            var syncRealmOptions = _syncRealmOptions.GetConfiguration();

            if (_user == null)
                throw new InvalidOperationException("User is not logged int yet.");

            var syncConfig = new SyncConfiguration(_user, new Uri(syncRealmOptions.ServerURL), _knownFolders.LocalAppData.GetFileInfo(syncRealmOptions.Path).PhysicalPath);
            var realm = await Realms.Realm.GetInstanceAsync(syncConfig);
            return new RealmUnitOfWork(realm);
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
    }
}