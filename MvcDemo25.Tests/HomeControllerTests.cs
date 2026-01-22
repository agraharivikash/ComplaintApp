using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MvcDemo25.web.Controllers;
using Xunit;
using Moq;

namespace MvcDemo25.Tests;

public class HomeControllerTests
{
    [Fact]
    public void Index_Returns_ViewResult()
    {
        var logger = Mock.Of<ILogger<HomeController>>();
        var controller = new HomeController();

        var result = controller.Index();

        Assert.IsType<ViewResult>(result);
    }

    [Fact]
    public void About_Returns_ViewResult()
    {
        var controller = new HomeController();

        var result = controller.About();

        Assert.IsType<ViewResult>(result);
    }
}
