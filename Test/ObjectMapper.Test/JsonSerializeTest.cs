using System.Security.Authentication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ObjectMapper;
using System.Text.Json;

namespace ObjectMapper.Test;


[TestClass]
public class JsonSerializeTest
{
    public TestContext? TestContext { get; set; }

    [TestMethod]
    public void TestJsonSerializer()
    {
        var device = new Device
        {
            Id = 1,
            Name = "TempSensor",
            Settings = "{}",
            Created = new DateTime(2023, 4, 19, 9, 0, 0),
            Updated = null
        };

        var updateDevice = new DeviceDto
        {
            Name = "Kitchen temp sensor",
            Settings = new Dictionary<string, object>
            {
                ["Address"] = 1,
                ["Registry"] = 6,
                ["Scale"] = 0.1f
            },
            Created = null,
            Updated = DateTime.Now
        };

        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        TestContext?.WriteLine("Before");
        TestContext?.WriteLine(JsonSerializer.Serialize(device));

        MapObject<DeviceDto, Device>.GetMapObject()
            .Ignore(dest => dest.Created)
            .CustomMap(dest => dest.Settings,
                src => JsonSerializer.Serialize(src.Settings, options))
            .Copy(updateDevice, device);

        TestContext?.WriteLine("After");
        TestContext?.WriteLine(JsonSerializer.Serialize(device));

        Assert.IsTrue(true);
    }

    [TestMethod]
    public void TestGetMethod()
    {
        var newDevice = new DeviceDto
        {
            Name = "Kitchen temp sensor",
            Settings = new Dictionary<string, object>
            {
                ["Address"] = 1,
                ["Registry"] = 6,
                ["Scale"] = 0.1f
            },
            Created = DateTime.Now,
        };

        Device device = MapObject<DeviceDto, Device>.GetMapObject()
            .CustomMap(dest => dest.Settings,
                src => JsonSerializer.Serialize(src.Settings, new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                }))
            .Get(newDevice);
        TestContext?.WriteLine(JsonSerializer.Serialize(device));
    }

    [TestMethod]
    public void TestCopyTo()
    {
        var device = new Device
        {
            Id = 1,
            Name = "TempSensor",
            Settings = "{}",
            Created = new DateTime(2023, 4, 19, 9, 0, 0),
            Updated = null
        };

        var updateDevice = new DeviceDto
        {
            Name = "Kitchen temp sensor",
            Settings = new Dictionary<string, object>
            {
                ["Address"] = 1,
                ["Registry"] = 6,
                ["Scale"] = 0.1f
            },
            Updated = DateTime.Now,
        };

        TestContext?.WriteLine("Before");
        TestContext?.WriteLine(JsonSerializer.Serialize(device));

        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        var mapper = MapObject<DeviceDto, Device>.GetMapObject()
            .Ignore(dest => dest.Created)
            .CustomMap(dest => dest.Settings,
                src => JsonSerializer.Serialize(src.Settings, options));
        
        updateDevice.CopyTo(device, mapper);

        TestContext?.WriteLine("After");
        TestContext?.WriteLine(JsonSerializer.Serialize(device));
    }
}

public class Device
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public string? Settings { get; set; }
    public DateTime? Created { get; set; }
    public DateTime? Updated { get; set; }
}

public class DeviceDto
{
    public string? Name { get; set; }
    public Dictionary<string, object>? Settings { get; set; }
    public DateTime? Created { get; set; }
    public DateTime? Updated { get; set; }
}