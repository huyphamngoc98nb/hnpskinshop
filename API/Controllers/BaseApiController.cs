using API.RequestHelpers;
using Core.Entities;
using Core.Interfaces;
using Microsoft.AspNetCore.Mvc;
using SkiShop.Core.Interfaces;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BaseApiController : ControllerBase
{
    // This is a base controller that will be used to inherit from other controllers
    protected async Task<ActionResult> CreatePageResult<T>(IGenericsRepository<T> repo, ISpecification<T> spec, int pageIndex, int pageSize) where T : BaseEntity
    {
        var count = await repo.CountAsync(spec);
        var data = await repo.ListAsync(spec);

        return Ok(new Pagination<T>(pageIndex, pageSize, count, data));
    }
}

