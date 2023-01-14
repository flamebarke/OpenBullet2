﻿using Microsoft.AspNetCore.Mvc;
using OpenBullet2.Web.Attributes;
using OpenBullet2.Web.Dtos.Plugin;
using OpenBullet2.Web.Exceptions;
using RuriLib.Services;

namespace OpenBullet2.Web.Controllers;

[Admin]
[ApiVersion("1.0")]
public class PluginController : ApiController
{
    private readonly PluginRepository _pluginRepository;

    public PluginController(PluginRepository pluginRepository)
    {
        _pluginRepository = pluginRepository;
    }

    [HttpGet("all")]
    [MapToApiVersion("1.0")]
    public ActionResult<IEnumerable<PluginDto>> GetAll()
    {
        var pluginNames = _pluginRepository.GetPluginNames();
        var plugins = pluginNames.Select(p => new PluginDto { Name = p });
        return Ok(plugins);
    }

    [HttpDelete]
    [MapToApiVersion("1.0")]
    public ActionResult Delete(string name)
    {
        var pluginNames = _pluginRepository.GetPluginNames();

        if (!pluginNames.Contains(name))
        {
            throw new EntryNotFoundException(
                ErrorCode.PLUGIN_NOT_FOUND, name, nameof(PluginRepository));
        }

        _pluginRepository.DeletePlugin(name);
        return Ok();
    }

    [HttpPost]
    [MapToApiVersion("1.0")]
    public ActionResult Add(IFormFile file)
    {
        _pluginRepository.AddPlugin(file.OpenReadStream());
        return Ok();
    }
}
