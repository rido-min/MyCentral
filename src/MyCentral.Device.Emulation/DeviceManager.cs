﻿using Microsoft.Extensions.Options;
using MyCentral.Client;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MyCentral.Device.Emulation
{
    public class DeviceManager : IEmulatedDeviceManager
    {
        private readonly IEmulatedDeviceFactory _factory;
        private Dictionary<string, Task<IEmulatedDevice>>? _devices;

        public DeviceManager(
            IOptions<DeviceCollection> collection,
            IEmulatedDeviceFactory factory)
        {
            Collection = collection.Value;
            _factory = factory;
        }

        public DeviceCollection Collection { get; }

        public Task<IEmulatedDevice?> GetDeviceAsync(string id, CancellationToken token)
        {
            if (_devices is null)
            {
                lock (this)
                {
                    if (_devices is null)
                    {
                        _devices = Collection.Devices.ToDictionary(d => d.Name, d => _factory.CreateDeviceAsync(d, token));
                    }
                }
            }

            if (_devices.TryGetValue(id, out var task))
            {
                return task!;
            }

            return Task.FromResult<IEmulatedDevice?>(null);
        }
    }
}
