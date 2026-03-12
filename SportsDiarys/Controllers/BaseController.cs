using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace SportDiary.Controllers;

[Authorize]
public abstract class BaseController : Controller
{
  
}
