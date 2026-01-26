using System;
using System.Collections.Generic;
using TIKSN.Identity;
using Xunit;
using static LanguageExt.Prelude;

namespace TIKSN.Tests.Identity;

public class ServiceIdentityTests
{
    [Fact]
    public void Constructor_ValidArguments_CreatesInstance()
    {
        // Arrange
        var applicationName = "MyApp";
        var componentNames = toSeq(["ServiceA", "ComponentB"]);
        var instanceId = ServiceInstanceId.Parse(Guid.NewGuid().ToString());

        // Act
        var serviceIdentity = new ServiceIdentity(applicationName, componentNames, instanceId);

        // Assert
        Assert.Equal(applicationName, serviceIdentity.ApplicationName);
        Assert.Equal(componentNames, serviceIdentity.ComponentNames);
        Assert.Equal(instanceId, serviceIdentity.InstanceId);
        Assert.True(serviceIdentity.ComponentPath.IsSome);
        _ = serviceIdentity.ComponentPath.IfSome(path => Assert.Equal("ServiceA.ComponentB", path));
    }

    [Fact]
    public void Constructor_EmptyComponentNames_ComponentPathIsNone()
    {
        // Arrange
        var applicationName = "MyApp";
        var componentNames = Seq<string>();
        var instanceId = ServiceInstanceId.Parse(Guid.NewGuid().ToString());

        // Act
        var serviceIdentity = new ServiceIdentity(applicationName, componentNames, instanceId);

        // Assert
        Assert.True(serviceIdentity.ComponentPath.IsNone);
    }

    [Fact]
    public void Constructor_NullComponentNames_ComponentPathIsNone()
    {
        // Arrange
        var applicationName = "MyApp";
        IEnumerable<string> componentNames = null;
        var instanceId = ServiceInstanceId.Parse(Guid.NewGuid().ToString());

        // Act
        var serviceIdentity = new ServiceIdentity(applicationName, componentNames, instanceId);

        // Assert
        Assert.True(serviceIdentity.ComponentPath.IsNone);
    }

    [Fact]
    public void Constructor_ApplicationNameIsNullOrWhiteSpace_ThrowsArgumentException()
    {
        // Arrange
        string applicationName1 = null;
        var applicationName2 = "";
        var applicationName3 = " ";
        var componentNames = toSeq(["ServiceA"]);
        var instanceId = ServiceInstanceId.Parse(Guid.NewGuid().ToString());

        // Act & Assert
        _ = Assert.Throws<ArgumentException>(() => new ServiceIdentity(applicationName1, componentNames, instanceId));
        _ = Assert.Throws<ArgumentException>(() => new ServiceIdentity(applicationName2, componentNames, instanceId));
        _ = Assert.Throws<ArgumentException>(() => new ServiceIdentity(applicationName3, componentNames, instanceId));
    }

    [Fact]
    public void CreateAnotherInstance_CreatesNewInstanceWithDifferentId()
    {
        // Arrange
        var applicationName = "MyApp";
        var componentNames = toSeq(["ServiceA"]);
        var instanceId1 = ServiceInstanceId.Parse(Guid.NewGuid().ToString());
        var serviceIdentity1 = new ServiceIdentity(applicationName, componentNames, instanceId1);
        var instanceId2 = ServiceInstanceId.Parse(Guid.NewGuid().ToString());

        // Act
        var serviceIdentity2 = serviceIdentity1.CreateAnotherInstance(instanceId2);

        // Assert
        Assert.Equal(serviceIdentity1.ApplicationName, serviceIdentity2.ApplicationName);
        Assert.Equal(serviceIdentity1.ComponentNames, serviceIdentity2.ComponentNames);
        Assert.NotEqual(serviceIdentity1.InstanceId, serviceIdentity2.InstanceId);
        Assert.Equal(instanceId2, serviceIdentity2.InstanceId);
        Assert.Equal(serviceIdentity1.ComponentPath, serviceIdentity2.ComponentPath);
    }

    [Fact]
    public void Equals_SameValues_ReturnsTrue()
    {
        // Arrange
        var applicationName = "MyApp";
        var componentNames = toSeq(["ServiceA", "ComponentB"]);
        var instanceId = ServiceInstanceId.Parse(Guid.NewGuid().ToString());

        var serviceIdentity1 = new ServiceIdentity(applicationName, componentNames, instanceId);
        var serviceIdentity2 = new ServiceIdentity(applicationName, componentNames, instanceId);

        // Act & Assert
        Assert.True(serviceIdentity1.Equals(serviceIdentity2));
        Assert.True(serviceIdentity1 == serviceIdentity2);
        Assert.Equal(serviceIdentity1.GetHashCode(), serviceIdentity2.GetHashCode());
    }

    [Fact]
    public void Equals_DifferentApplicationName_ReturnsFalse()
    {
        // Arrange
        var applicationName1 = "MyApp1";
        var applicationName2 = "MyApp2";
        var componentNames = toSeq(["ServiceA"]);
        var instanceId = ServiceInstanceId.Parse(Guid.NewGuid().ToString());

        var serviceIdentity1 = new ServiceIdentity(applicationName1, componentNames, instanceId);
        var serviceIdentity2 = new ServiceIdentity(applicationName2, componentNames, instanceId);

        // Act & Assert
        Assert.False(serviceIdentity1.Equals(serviceIdentity2));
        Assert.True(serviceIdentity1 != serviceIdentity2);
    }

    [Fact]
    public void Equals_DifferentComponentNames_ReturnsFalse()
    {
        // Arrange
        var applicationName = "MyApp";
        var componentNames1 = toSeq(["ServiceA"]);
        var componentNames2 = toSeq(["ServiceB"]);
        var instanceId = ServiceInstanceId.Parse(Guid.NewGuid().ToString());

        var serviceIdentity1 = new ServiceIdentity(applicationName, componentNames1, instanceId);
        var serviceIdentity2 = new ServiceIdentity(applicationName, componentNames2, instanceId);

        // Act & Assert
        Assert.False(serviceIdentity1.Equals(serviceIdentity2));
        Assert.True(serviceIdentity1 != serviceIdentity2);
    }

    [Fact]
    public void Equals_DifferentInstanceId_ReturnsFalse()
    {
        // Arrange
        var applicationName = "MyApp";
        var componentNames = toSeq(["ServiceA"]);
        var instanceId1 = ServiceInstanceId.Parse(Guid.NewGuid().ToString());
        var instanceId2 = ServiceInstanceId.Parse(Guid.NewGuid().ToString());

        var serviceIdentity1 = new ServiceIdentity(applicationName, componentNames, instanceId1);
        var serviceIdentity2 = new ServiceIdentity(applicationName, componentNames, instanceId2);

        // Act & Assert
        Assert.False(serviceIdentity1.Equals(serviceIdentity2));
        Assert.True(serviceIdentity1 != serviceIdentity2);
    }

    [Fact]
    public void ToString_ReturnsExpectedFormat()
    {
        // Arrange
        var applicationName = "MyApp";
        var componentNames = toSeq(["ServiceA", "ComponentB"]);
        var instanceId = ServiceInstanceId.Parse("00000000-0000-0000-0000-000000000001");

        var serviceIdentity = new ServiceIdentity(applicationName, componentNames, instanceId);

        // Act
        var result = serviceIdentity.ToString();

        // Assert
        Assert.Equal($"MyApp.ServiceA.ComponentB:{instanceId}", result);
    }

    [Fact]
    public void ToString_NoComponents_ReturnsApplicationName()
    {
        // Arrange
        var applicationName = "MyApp";
        var componentNames = Seq<string>();
        var instanceId = ServiceInstanceId.Parse("00000000-0000-0000-0000-000000000002");

        var serviceIdentity = new ServiceIdentity(applicationName, componentNames, instanceId);

        // Act
        var result = serviceIdentity.ToString();

        // Assert
        Assert.Equal($"MyApp:{instanceId}", result);
    }
}

