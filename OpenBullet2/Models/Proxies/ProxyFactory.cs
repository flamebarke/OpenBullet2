﻿using OpenBullet2.Entities;
using RuriLib.Models.Proxies;

namespace OpenBullet2.Models.Proxies
{
    public class ProxyFactory
    {
        public Proxy FromEntity(ProxyEntity entity)
        {
            return new Proxy(entity.Host, entity.Port, entity.Type, entity.Username, entity.Password);
        }
    }
}
