using System;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using TIKSN.Data;
using Xunit;

namespace TIKSN.IntegrationTests.Data.Mongo;

[Collection("ServiceProviderCollection")]
public class MongoFileRepositoryTests
{
    private readonly ServiceProviderFixture serviceProviderFixture;

    public MongoFileRepositoryTests(ServiceProviderFixture serviceProviderFixture) =>
        this.serviceProviderFixture = serviceProviderFixture ?? throw new ArgumentNullException(nameof(serviceProviderFixture));

    [Fact]
    public async Task TestFileDuplicateUpload()
    {
        var testFileRepository = this.serviceProviderFixture.GetServiceProvider().GetRequiredService<ITestMongoFileRepository>();

        var buffer1 = new byte[1024 * 1024 * 4];
        Random.Shared.NextBytes(buffer1);

        var buffer2 = new byte[1024 * 1024 * 2];
        Random.Shared.NextBytes(buffer2);

        var fileName = $"file{Random.Shared.Next()}.bin";

        await testFileRepository.UploadAsync(fileName, buffer1, default);
        Func<Task> upload = () => testFileRepository.UploadAsync(fileName, buffer2, default);

        _ = await upload.Should().ThrowAsync<EntityExistsException>();
    }

    [Fact]
    public async Task TestFileExists()
    {
        var testFileRepository = this.serviceProviderFixture.GetServiceProvider().GetRequiredService<ITestMongoFileRepository>();

        var buffer = new byte[1024 * 1024];
        Random.Shared.NextBytes(buffer);

        var fileName = $"file{Random.Shared.Next()}.bin";

        var stage1Exists = await testFileRepository.ExistsAsync(fileName, default);
        await testFileRepository.UploadAsync(fileName, buffer, default);
        var stage2Exists = await testFileRepository.ExistsAsync(fileName, default);

        _ = stage1Exists.Should().BeFalse();
        _ = stage2Exists.Should().BeTrue();
    }
}
