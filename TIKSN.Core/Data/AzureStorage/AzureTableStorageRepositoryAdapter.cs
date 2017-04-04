using Microsoft.Extensions.Options;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace TIKSN.Data.AzureStorage
{
	public class AzureTableStorageRepositoryAdapter<T> : IRepository<T> where T : class, ITableEntity
	{
		private readonly IAzureTableStorageRepository<T> _azureTableStorageRepository;
		private readonly IOptions<AzureTableStorageRepositoryAdapterOptions> _options;

		public AzureTableStorageRepositoryAdapter(IAzureTableStorageRepository<T> azureTableStorageRepository, IOptions<AzureTableStorageRepositoryAdapterOptions> options)
		{
			_azureTableStorageRepository = azureTableStorageRepository;
			_options = options;
		}

		public Task AddAsync(T entity, CancellationToken cancellationToken)
		{
			switch (_options.Value.AddOption)
			{
				case AzureTableStorageRepositoryAdapterOptions.AddOptions.Add:
					return _azureTableStorageRepository.AddAsync(entity, cancellationToken);

				case AzureTableStorageRepositoryAdapterOptions.AddOptions.AddOrMerge:
					return _azureTableStorageRepository.AddOrMergeAsync(entity, cancellationToken);

				case AzureTableStorageRepositoryAdapterOptions.AddOptions.AddOrReplace:
					return _azureTableStorageRepository.AddOrReplaceAsync(entity, cancellationToken);

				default:
					throw new NotSupportedException($"Add option '{_options.Value.AddOption}' is not supported.");
			}
		}

		public Task AddRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken)
		{
			return BatchOperationHelper.BatchOperationAsync(entities, cancellationToken, AddAsync);
		}

		public Task RemoveAsync(T entity, CancellationToken cancellationToken)
		{
			return _azureTableStorageRepository.DeleteAsync(entity, cancellationToken);
		}

		public Task RemoveRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken)
		{
			return BatchOperationHelper.BatchOperationAsync(entities, cancellationToken, RemoveAsync);
		}

		public Task UpdateAsync(T entity, CancellationToken cancellationToken)
		{
			switch (_options.Value.UpdateOption)
			{
				case AzureTableStorageRepositoryAdapterOptions.UpdateOptions.Merge:
					return _azureTableStorageRepository.MergeAsync(entity, cancellationToken);

				case AzureTableStorageRepositoryAdapterOptions.UpdateOptions.Replace:
					return _azureTableStorageRepository.ReplaceAsync(entity, cancellationToken);

				default:
					throw new NotSupportedException($"Update option '{_options.Value.AddOption}' is not supported.");
			}
		}

		public Task UpdateRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken)
		{
			return BatchOperationHelper.BatchOperationAsync(entities, cancellationToken, UpdateAsync);
		}
	}

	public class AzureTableStorageRepositoryAdapter<TBusinessEntity, TDataEntity>
		: AzureTableStorageRepositoryAdapter<TDataEntity>, IRepository<TBusinessEntity> where TDataEntity : class, ITableEntity where TBusinessEntity : class
	{
		private readonly IMapper<TBusinessEntity, TDataEntity> _mapper;

		public AzureTableStorageRepositoryAdapter(
			IAzureTableStorageRepository<TDataEntity> azureTableStorageRepository,
			IOptions<AzureTableStorageRepositoryAdapterOptions> options,
			IMapper<TBusinessEntity, TDataEntity> mapper) : base(azureTableStorageRepository, options)
		{
			_mapper = mapper;
		}

		public Task AddAsync(TBusinessEntity entity, CancellationToken cancellationToken)
		{
			var dataEntity = _mapper.Map(entity);

			return AddAsync(dataEntity, cancellationToken);
		}

		public Task AddRangeAsync(IEnumerable<TBusinessEntity> entities, CancellationToken cancellationToken)
		{
			return BatchOperationHelper.BatchOperationAsync(entities, cancellationToken, AddAsync);
		}

		public Task RemoveAsync(TBusinessEntity entity, CancellationToken cancellationToken)
		{
			var dataEntity = _mapper.Map(entity);

			return RemoveAsync(dataEntity, cancellationToken);
		}

		public Task RemoveRangeAsync(IEnumerable<TBusinessEntity> entities, CancellationToken cancellationToken)
		{
			return BatchOperationHelper.BatchOperationAsync(entities, cancellationToken, RemoveAsync);
		}

		public Task UpdateAsync(TBusinessEntity entity, CancellationToken cancellationToken)
		{
			var dataEntity = _mapper.Map(entity);

			return UpdateAsync(dataEntity, cancellationToken);
		}

		public Task UpdateRangeAsync(IEnumerable<TBusinessEntity> entities, CancellationToken cancellationToken)
		{
			return BatchOperationHelper.BatchOperationAsync(entities, cancellationToken, UpdateAsync);
		}
	}
}