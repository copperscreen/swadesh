using Microsoft.AspNetCore.Mvc;
using System.Collections;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using System.Collections.Immutable;
using System.Web;
using Core;

namespace swadesh3.Controllers;
[ApiController]
[Route("[controller]")]
public class ListController : ControllerBase
{
    private readonly ILogger<ListController> _logger;

    public ListController(ILogger<ListController> logger)
    {
        _logger = logger;
    }

    [HttpGet]
    public string[][] Get()
    {
        return Scraper.Get();
	}

    [HttpGet("Compiled")]
    public CompiledTable Compiled()
    {
        return Scraper.Compiled(this.Request.Query["urls"].ToString().Split(';'));
    }
}
