﻿using OpenBullet2.Web.Attributes;
using OpenBullet2.Web.Dtos.Config.Blocks.Settings;
using RuriLib.Models.Blocks.Custom.HttpRequest.Multipart;

namespace OpenBullet2.Web.Dtos.Config.Blocks.HttpRequest;

/// <summary>
/// DTO that represents a string multipart setting.
/// </summary>
[PolyType("multipartString")]
[MapsFrom(typeof(StringHttpContentSettingsGroup))]
public class StringHttpContentSettingsGroupDto : HttpContentSettingsGroupDto
{
    /// <summary></summary>
    public StringHttpContentSettingsGroupDto()
    {
        Type = HttpContentSettingsGroupType.String;
    }

    /// <summary>
    /// The string data.
    /// </summary>
    public BlockSettingDto? Data { get; set; }
}
