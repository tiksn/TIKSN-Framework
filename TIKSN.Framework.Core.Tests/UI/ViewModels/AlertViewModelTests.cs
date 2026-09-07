using Shouldly;
using TIKSN.UI.ViewModels;
using Xunit;

namespace TIKSN.Tests.UI.ViewModels;

public class AlertViewModelTests
{
    [Fact]
    public void Constructor_SetsProperties()
    {
        // Arrange
        var title = "Test Title";
        var description = "Test Description";
        var buttonText = "OK";

        // Act
        var alert = new AlertViewModel(title, description, buttonText);

        // Assert
        alert.Title.ShouldBe(title);
        alert.Description.ShouldBe(description);
        alert.ButtonText.ShouldBe(buttonText);
    }
}
