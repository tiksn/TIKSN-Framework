using System;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace TIKSN.IntegrationTests.Data.Mongo;

[Collection("ServiceProviderCollection")]
public class MongoFileRepositoryTests
{
    private readonly ServiceProviderFixture serviceProviderFixture;

    public MongoFileRepositoryTests(ServiceProviderFixture serviceProviderFixture) =>
        this.serviceProviderFixture = serviceProviderFixture ?? throw new ArgumentNullException(nameof(serviceProviderFixture));

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
