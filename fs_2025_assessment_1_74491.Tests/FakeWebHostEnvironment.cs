using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.FileProviders;
using System;

public class FakeWebHostEnvironment : IWebHostEnvironment
{
    public string ApplicationName { get; set; } = "Tests";
    public string EnvironmentName { get; set; } = "Development";

    public string WebRootPath { get; set; } = AppContext.BaseDirectory;
    public IFileProvider WebRootFileProvider { get; set; }
        = new NullFileProvider();

    public string ContentRootPath { get; set; } = AppContext.BaseDirectory;
    public IFileProvider ContentRootFileProvider { get; set; }
        = new NullFileProvider();
}
