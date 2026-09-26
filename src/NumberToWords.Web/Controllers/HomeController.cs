using Microsoft.AspNetCore.Mvc;
using NumberToWords.Core.Services;
using NumberToWords.Web.Models;

namespace NumberToWords.Web.Controllers;

public class HomeController(NumberToWordsConverter converter) : Controller
{
    [HttpGet]
    public IActionResult Index() => View(new ConvertViewModel());

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Index(ConvertViewModel model)
    {
        var result = converter.Convert(model.Number ?? "");

        if (result.IsSuccess)
            model.Words = result.Words;
        else
            model.Error = result.Error;

        return View(model);
    }
}