using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SimplCommerce.Module.Core.Models;
using SimplCommerce.Module.Core.Services;
using SimplCommerce.Module.Core.ViewModels;


namespace SimplCommerce.Module.Core.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger _logger;
        private readonly IWidgetInstanceService _widgetInstanceService;
        private readonly UserManager<User> _userManager;

        public HomeController(
            ILoggerFactory factory,
            IWidgetInstanceService widgetInstanceService,
            UserManager<User> userManager
            )
        {
            _logger = factory.CreateLogger("Unhandled Error");
            _widgetInstanceService = widgetInstanceService;
            _userManager = userManager;
        }

        public IActionResult TestError()
        {
            throw new Exception("Test behavior in case of error");
        }

        public async Task<IActionResult> Index()
        {
            var model = new HomeViewModel();

            model.WidgetInstances = _widgetInstanceService.GetPublished().Select(x => new WidgetInstanceViewModel
            {
                Id = x.Id,
                Name = x.Name,
                ViewComponentName = x.Widget.ViewComponentName,
                WidgetId = x.WidgetId,
                WidgetZoneId = x.WidgetZoneId,
                Data = x.Data,
                HtmlData = x.HtmlData
            }).ToList();

            model.Roles = await GetCurrentUserRolesAsync();
            
            return View(model);
        }

        [HttpGet("/Home/ErrorWithCode/{statusCode}")]
        public IActionResult ErrorWithCode(int statusCode)
        {
            if (statusCode == 404)
            {
                return View("404");
            }

            return View("Error");
        }

        public IActionResult Error()
        {
            var feature = HttpContext.Features.Get<IExceptionHandlerFeature>();
            var error = feature?.Error;

            if (error != null)
            {
                _logger.LogError(error.Message, error);
            }

            return View("Error");
        }

        private async Task<IList<string>> GetCurrentUserRolesAsync()
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);
            return (user == null ? new List<string>() : await _userManager.GetRolesAsync(user));
        }
        
    }
}