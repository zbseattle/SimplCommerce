using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SimplCommerce.Module.Cms.Services;

namespace SimplCommerce.Module.Cms.Controllers
{
    [Authorize(Roles = "admin")]
    [Route("api/menu")]
    public class MenuController : Controller
    {
        private IMenuService _menuSerivce;
        public MenuController(IMenuService menuService)
        {
            this._menuSerivce = menuService;
        }
    }
}
