namespace TinyStorage.Application.Shared.UnitTests.Common;

public sealed class UserAccessorTests
{
    [Fact]
    public void Isu_ReturnsClaimValue_WhenExists()
    {
        // Arrange
        var claims = new[] { new Claim("isu", "1337") };
        var principal = new ClaimsPrincipal(new ClaimsIdentity(claims));
        var context = new DefaultHttpContext { User = principal };
        var accessor = new Mock<IHttpContextAccessor>();
        accessor.SetupGet(x => x.HttpContext).Returns(context);

        var user = new UserAccessor(accessor.Object);

        // Act
        var isu = user.Isu;

        // Assert
        isu.Should().Be(1337);
    }

    [Fact]
    public void Isu_ReturnsZero_WhenClaimNotExists()
    {
        // Arrange
        var principal = new ClaimsPrincipal(new ClaimsIdentity());
        var context = new DefaultHttpContext { User = principal };
        var accessor = new Mock<IHttpContextAccessor>();
        accessor.SetupGet(x => x.HttpContext).Returns(context);

        var user = new UserAccessor(accessor.Object);

        // Act
        var isu = user.Isu;

        // Assert
        isu.Should().Be(0);
    }

    [Fact]
    public void IsLaboratoryAssistant_ReturnsTrue_WhenRoleExists()
    {
        // Arrange
        var claims = new[] { new Claim(ClaimTypes.Role, "Лаборант") };
        var principal = new ClaimsPrincipal(new ClaimsIdentity(claims));
        var context = new DefaultHttpContext { User = principal };
        var accessor = new Mock<IHttpContextAccessor>();
        accessor.SetupGet(x => x.HttpContext).Returns(context);

        var user = new UserAccessor(accessor.Object);

        // Act & Assert
        user.IsLaboratoryAssistant.Should().BeTrue();
    }

    [Fact]
    public void IsLaboratoryAssistant_ReturnsFalse_WhenRoleNotExists()
    {
        // Arrange
        var claims = new[] { new Claim(ClaimTypes.Role, "Администратор") };
        var principal = new ClaimsPrincipal(new ClaimsIdentity(claims));
        var context = new DefaultHttpContext { User = principal };
        var accessor = new Mock<IHttpContextAccessor>();
        accessor.SetupGet(x => x.HttpContext).Returns(context);

        var user = new UserAccessor(accessor.Object);

        // Act & Assert
        user.IsLaboratoryAssistant.Should().BeFalse();
    }

    [Fact]
    public void IsAdministrator_ReturnsTrue_WhenRoleExists()
    {
        // Arrange
        var claims = new[] { new Claim(ClaimTypes.Role, "Администратор") };
        var principal = new ClaimsPrincipal(new ClaimsIdentity(claims));
        var context = new DefaultHttpContext { User = principal };
        var accessor = new Mock<IHttpContextAccessor>();
        accessor.SetupGet(x => x.HttpContext).Returns(context);

        var user = new UserAccessor(accessor.Object);

        // Act & Assert
        user.IsAdministrator.Should().BeTrue();
    }

    [Fact]
    public void IsAdministrator_ReturnsFalse_WhenRoleNotExists()
    {
        // Arrange
        var claims = new[] { new Claim(ClaimTypes.Role, "Лаборант") };
        var principal = new ClaimsPrincipal(new ClaimsIdentity(claims));
        var context = new DefaultHttpContext { User = principal };
        var accessor = new Mock<IHttpContextAccessor>();
        accessor.SetupGet(x => x.HttpContext).Returns(context);

        var user = new UserAccessor(accessor.Object);

        // Act & Assert
        user.IsAdministrator.Should().BeFalse();
    }
}
