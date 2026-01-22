using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MvcDemo25.web.Controllers;
using MvcDemo25.web.Models;
using Moq;
using System.Threading.Tasks;
using Xunit;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;

namespace MvcDemo25.Tests;

public class PersonControllerTests
{
    private AppDbContext CreateInMemoryContext(string dbName)
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(dbName)
            .Options;
        return new AppDbContext(options);
    }

    private PersonController CreateController(AppDbContext ctx, ITempDataDictionary? tempData = null)
    {
        var logger = Mock.Of<ILogger<PersonController>>();
        var controller = new PersonController(logger, ctx);
        if (tempData != null)
            controller.TempData = tempData;
        else
            controller.TempData = new TempDataDictionary(new DefaultHttpContext(), Mock.Of<ITempDataProvider>());
        return controller;
    }

    [Fact]
    public async Task Index_Returns_View_With_People()
    {
        var ctx = CreateInMemoryContext("Index_Returns_View_With_People");
        ctx.People.Add(new Person { FirstName = "A", LastName = "Z" });
        ctx.People.Add(new Person { FirstName = "B", LastName = "Y" });
        await ctx.SaveChangesAsync();

        var controller = CreateController(ctx);

        var result = await controller.Index();

        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsAssignableFrom<IEnumerable<Person>>(viewResult.Model);
    }

    [Fact]
    public void Add_Get_Returns_View()
    {
        var ctx = CreateInMemoryContext("Add_Get_Returns_View");
        var controller = CreateController(ctx);

        var result = controller.Add();

        Assert.IsType<ViewResult>(result);
    }

    [Fact]
    public async Task Add_Post_InvalidModel_Returns_View()
    {
        var ctx = CreateInMemoryContext("Add_Post_InvalidModel_Returns_View");
        var controller = CreateController(ctx);
        controller.ModelState.AddModelError("FirstName", "Required");

        var result = await controller.Add(new Person());

        Assert.IsType<ViewResult>(result);
    }

    [Fact]
    public async Task Add_Post_ValidModel_Redirects()
    {
        var ctx = CreateInMemoryContext("Add_Post_ValidModel_Redirects");
        var controller = CreateController(ctx);

        var person = new Person { FirstName = "John", LastName = "Doe" };
        var result = await controller.Add(person);

        var redirect = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal(nameof(PersonController.Add), redirect.ActionName);
    }

    [Fact]
    public async Task Edit_Get_Returns_View_With_Person()
    {
        var ctx = CreateInMemoryContext("Edit_Get_Returns_View_With_Person");
        var p = new Person { FirstName = "X", LastName = "Y" };
        ctx.People.Add(p);
        await ctx.SaveChangesAsync();

        var controller = CreateController(ctx);
        var result = await controller.Edit(p.Id);

        var view = Assert.IsType<ViewResult>(result);
        var model = Assert.IsType<Person>(view.Model);
    }

    [Fact]
    public async Task Edit_Post_InvalidModel_Returns_View()
    {
        var ctx = CreateInMemoryContext("Edit_Post_InvalidModel_Returns_View");
        var controller = CreateController(ctx);
        controller.ModelState.AddModelError("FirstName", "Required");

        var result = await controller.Edit(new Person());

        Assert.IsType<ViewResult>(result);
    }

    [Fact]
    public async Task Edit_Post_NoRecordFound_Returns_View_With_Error()
    {
        var ctx = CreateInMemoryContext("Edit_Post_NoRecordFound_Returns_View_With_Error");
        var controller = CreateController(ctx);

        var person = new Person { Id = 999, FirstName = "A", LastName = "B" };
        var result = await controller.Edit(person);

        var view = Assert.IsType<ViewResult>(result);
        Assert.Equal("No record found", controller.TempData["error"]);
    }

    [Fact]
    public async Task Edit_Post_Valid_Updates_And_Redirects()
    {
        var ctx = CreateInMemoryContext("Edit_Post_Valid_Updates_And_Redirects");
        var p = new Person { FirstName = "Old", LastName = "Name" };
        ctx.People.Add(p);
        await ctx.SaveChangesAsync();

        var controller = CreateController(ctx);
        p.FirstName = "New";

        var result = await controller.Edit(p);

        var redirect = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal(nameof(PersonController.Index), redirect.ActionName);
    }

    [Fact]
    public async Task Delete_Removes_Record_And_Redirects()
    {
        var ctx = CreateInMemoryContext("Delete_Removes_Record_And_Redirects");
        var p = new Person { FirstName = "ToBe", LastName = "Deleted" };
        ctx.People.Add(p);
        await ctx.SaveChangesAsync();

        var controller = CreateController(ctx);

        var result = await controller.Delete(p.Id);

        var redirect = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal(nameof(PersonController.Index), redirect.ActionName);
        Assert.Empty(await ctx.People.ToListAsync());
    }
}
