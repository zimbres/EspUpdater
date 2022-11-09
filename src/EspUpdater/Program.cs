using System.Security.Cryptography;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
var store = builder.Configuration.GetValue<string>("Store");

var app = builder.Build();

var macsFile = new ConfigurationBuilder().AddJsonFile("macs.json", false, true).Build();
var macs = macsFile.GetSection("MACS").Get<List<string>>();

app.MapGet("/", () =>
{
    return Results.Text("Ok");
});

app.MapGet("/update", (HttpRequest request) =>
{
    if (request.Headers.UserAgent != "ESP8266-http-Update")
    {
        return Results.BadRequest("Only for ESP8266 updater!");
    }

    var mac = request.Headers["x-ESP8266-STA-MAC"].ToString();
    app.Logger.LogInformation("Connection from MAC: {mac}", mac);
    var fileName = mac.Replace(":", "");

    if (!macs.Contains(mac))
    {
        return Results.Unauthorized();
    }

    var md5 = GetMD5Checksum($"{store}{fileName}.bin");
    var md5Sketch = request.Headers["x-ESP8266-sketch-md5"];

    if (md5 == md5Sketch)
    {
        return Results.StatusCode(304);
    }

    return Results.File($"{store}{fileName}.bin");
});

static string GetMD5Checksum(string filename)
{
    using var md5 = MD5.Create();
    using var stream = File.OpenRead(filename);
    var hash = md5.ComputeHash(stream);
    return BitConverter.ToString(hash).Replace("-", "").ToLower();
}

app.Run();
