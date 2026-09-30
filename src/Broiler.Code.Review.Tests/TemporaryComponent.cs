using System.Text;
using Broiler.Code.Review.Cli.Assurance;

namespace Broiler.Code.Review.Tests;

/// <summary>
/// A component root on disk for one test, deleted afterwards. Files are written
/// and read as bytes, so a test controls byte-order marks and line endings
/// exactly and sees exactly what the tool wrote.
/// </summary>
internal sealed class TemporaryComponent : IDisposable
{
    public TemporaryComponent()
    {
        Root = Path.Combine(Path.GetTempPath(), "broiler-assurance-" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(Root);
    }

    public string Root { get; }

    public string PathOf(string relative) =>
        Path.Combine(Root, relative.Replace('/', Path.DirectorySeparatorChar));

    public void Write(string relative, string text, bool byteOrderMark = false) =>
        WriteBytes(relative, AssuranceSourceText.Encode(text, byteOrderMark));

    public void WriteBytes(string relative, byte[] bytes)
    {
        string path = PathOf(relative);
        Directory.CreateDirectory(Path.GetDirectoryName(path)!);
        File.WriteAllBytes(path, bytes);
    }

    public void Folder(string relative) => Directory.CreateDirectory(PathOf(relative));

    public byte[] ReadBytes(string relative) => File.ReadAllBytes(PathOf(relative));

    public string Read(string relative) => Encoding.UTF8.GetString(ReadBytes(relative));

    /// <summary>A minimal project file, optionally naming its assembly or marking itself a test.</summary>
    public void Project(string relative, string? assemblyName = null, bool isTest = false)
    {
        var properties = new StringBuilder();
        if (assemblyName is not null)
            properties.Append("<AssemblyName>").Append(assemblyName).Append("</AssemblyName>");

        if (isTest)
            properties.Append("<IsTestProject>true</IsTestProject>");

        Write(relative,
            $"<Project Sdk=\"Microsoft.NET.Sdk\"><PropertyGroup>{properties}</PropertyGroup></Project>\n");
    }

    /// <summary>Runs <c>broiler-review assurance</c> with the given arguments.</summary>
    public (int Exit, string Output, string Error) Run(params string[] args)
    {
        var output = new StringWriter();
        var error = new StringWriter();
        int exit = AssuranceCommand.Run(args, output, error);
        return (exit, output.ToString(), error.ToString());
    }

    public void Dispose()
    {
        try
        {
            Directory.Delete(Root, recursive: true);
        }
        catch (IOException)
        {
        }
        catch (UnauthorizedAccessException)
        {
        }
    }
}
