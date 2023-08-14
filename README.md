# ObjectMapper

A simple object mapper to use copy properties to another object.

## How to use?

```csharp
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
```

```csharp
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
```

## How to get new object?

```csharp
var newDevice = new DeviceDto
{
    Name = "Kitchen temp sensor",
    Settings = new Dictionary<string, object>
    {
        ["Address"] = 1,
        ["Registry"] = 6,
        ["Scale"] = 0.1f
    },
    Created = DateTime.Now
};

Device device = MapObject<DeviceDto, Device>.GetMapObject()
    .CustomMap(dest => dest.Settings, 
        src => JsonSerializer.Serialize(src.Settings, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            }))
    .Get(newDevice);
```

## Using CopyTo extension method.

```csharp
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

var options = new JsonSerializerOptions
{
    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
};

var mapper = MapObject<DeviceDto, Device>.GetMapObject()
    .Ignore(dest => dest.Created)
    .CustomMap(dest => dest.Settings,
        src => JsonSerializer.Serialize(src.Settings, options));

updateDevice.CopyTo(device, mapper);
```

## Use in collections

```csharp
var deviceDtos = new List<DeviceDto>
{
    new DeviceDto { Name = "device01", Created = DateTime.Now },
    new DeviceDto { Name = "device02", Created = DateTime.Now },
    new DeviceDto { Name = "device03", Created = DateTime.Now },
    new DeviceDto { Name = "device04", Created = DateTime.Now },
    new DeviceDto { Name = "device05", Created = DateTime.Now },
    new DeviceDto { Name = "device06", Created = DateTime.Now },
    new DeviceDto { Name = "device07", Created = DateTime.Now },
    new DeviceDto { Name = "device08", Created = DateTime.Now },
};

var mapper = MapObject<DeviceDto, Device>.GetMapObject()
    .CustomMap(dest => dest.Settings, src => "some settings")
    .Ignore(dest => dest.Updated);

var devices = deviceDtos.Select(dto => mapper.Get(dto));
```

## Changes
### 1.0.3
- New `CopyTo` and `ConvertTo` extension methods. If you want to use without custom map/ignore properties, use these methods. Example: `objectA.ConvertTo<TypeB>()`.