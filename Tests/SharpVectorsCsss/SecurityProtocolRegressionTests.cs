#if DOTNET48
using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using SharpVectors.Dom.Css;
using SharpVectors.Net;

namespace SharpVectors.Csss.Tests
{
    [TestFixture]
    public class SecurityProtocolRegressionTests
    {
        [Test]
        public void HttpCreator_ReadsLoopbackResponse()
        {
            var listener = new TcpListener(IPAddress.Loopback, 0);
            listener.Start();
            try
            {
                int port = ((IPEndPoint)listener.LocalEndpoint).Port;
                var server = Task.Run(() =>
                {
                    using (var client = listener.AcceptTcpClient())
                    using (var stream = client.GetStream())
                    {
                        client.ReceiveTimeout = 5000;
                        using (var reader = new StreamReader(stream, Encoding.ASCII, false, 1024, true))
                        {
                            string line;
                            do { line = reader.ReadLine(); } while (!string.IsNullOrEmpty(line));
                            if (line == null)
                            {
                                throw new IOException("HTTP request ended before its headers were complete.");
                            }
                        }

                        byte[] response = Encoding.ASCII.GetBytes(
                            "HTTP/1.1 200 OK\r\nContent-Length: 5\r\nConnection: close\r\n\r\nHello");
                        stream.Write(response, 0, response.Length);
                    }
                });
                var clientRequest = Task.Run(() =>
                {
                    var creator = new ExtendedHttpWebRequestCreator();
                    var request = creator.Create(new Uri("http://127.0.0.1:" + port + "/image.svg"));
                    using (var response = request.GetResponse())
                    using (var reader = new StreamReader(response.GetResponseStream()))
                    {
                        return reader.ReadToEnd();
                    }
                });

                Assert.That(clientRequest.Wait(TimeSpan.FromSeconds(10)), Is.True, "HTTP request timed out.");
                Assert.That(clientRequest.Result, Is.EqualTo("Hello"));
                Assert.That(server.Wait(TimeSpan.FromSeconds(10)), Is.True, "HTTP server timed out.");
            }
            finally
            {
                listener.Stop();
            }
        }

        [TestCase(0, "document")]
        [TestCase(3072, "document")]
        [TestCase(0, "svg")]
        [TestCase(3072, "svg")]
        [TestCase(0, "legacyOptIn")]
        [TestCase(0, "nameTableDocument")]
        [TestCase(3072, "nameTableDocument")]
        [TestCase(0, "data")]
        [TestCase(3072, "data")]
        [TestCase(0, "http")]
        [TestCase(3072, "http")]
        [TestCase(0, "httpRegistration")]
        [TestCase(3072, "httpRegistration")]
        public void RequestSetup_PreservesHostProtocolInFreshProcess(int initialProtocol, string operation)
        {
            string corePath = typeof(DataWebRequest).Assembly.Location.Replace("'", "''");
            string cssPath = typeof(CssXmlDocument).Assembly.Location.Replace("'", "''");
            string testPath = typeof(TestSvgWindow).Assembly.Location.Replace("'", "''");
            string script = @"
$ErrorActionPreference = 'Stop'
try {
    [void][Reflection.Assembly]::LoadFrom('" + corePath + @"')
    [void][Reflection.Assembly]::LoadFrom('" + cssPath + @"')
    [void][Reflection.Assembly]::LoadFrom('" + testPath + @"')
    $expected = [Net.SecurityProtocolType]" + initialProtocol + @"
    [Net.ServicePointManager]::SecurityProtocol = $expected
    switch ('" + operation + @"') {
        'document' {
            $document = New-Object SharpVectors.Dom.Css.CssXmlDocument
            $bytes = [Text.Encoding]::UTF8.GetBytes('<svg xmlns=""http://www.w3.org/2000/svg""/>')
            $stream = New-Object IO.MemoryStream(,$bytes)
            try { $document.Load($stream) } finally { $stream.Dispose() }
            if ($document.DocumentElement.LocalName -ne 'svg') { throw 'SVG document did not load' }
        }
        'svg' {
            $window = [SharpVectors.Csss.Tests.TestSvgWindow]::Create()
            $document = New-Object SharpVectors.Dom.Svg.SvgDocument($window)
            $bytes = [Text.Encoding]::UTF8.GetBytes('<svg xmlns=""http://www.w3.org/2000/svg""/>')
            $stream = New-Object IO.MemoryStream(,$bytes)
            try { $document.Load($stream) } finally { $stream.Dispose() }
            if ($document.DocumentElement.LocalName -ne 'svg') { throw 'SVG document did not load' }
        }
        'legacyOptIn' {
            [SharpVectors.Net.DataSecurityProtocols]::Initialize()
            $expected = [Net.SecurityProtocolType]([int][Net.SecurityProtocolType]::Tls -bor [int][Net.SecurityProtocolType]::Tls11 -bor [int][Net.SecurityProtocolType]::Tls12)
        }
        'nameTableDocument' {
            $document = New-Object SharpVectors.Dom.Css.CssXmlDocument (New-Object Xml.NameTable)
            if ($null -eq $document) { throw 'Document was not created' }
        }
        'data' {
            [void][SharpVectors.Net.DataWebRequest]::Register()
            $request = [Net.WebRequest]::Create('data:text/plain;base64,SGVsbG8=')
            $response = $request.GetResponse()
            try {
                $reader = New-Object IO.StreamReader($response.GetResponseStream())
                try { $text = $reader.ReadToEnd() } finally { $reader.Dispose() }
                if ($text -ne 'Hello') { throw 'Data URI response was incorrect' }
            } finally { $response.Dispose() }
        }
        'http' {
            $creator = New-Object SharpVectors.Net.ExtendedHttpWebRequestCreator
            $request = $creator.Create([Uri]'https://example.com/image.svg')
            if ($request.RequestUri.AbsoluteUri -ne 'https://example.com/image.svg') { throw 'HTTP request was incorrect' }
        }
        'httpRegistration' { [void][SharpVectors.Net.ExtendedHttpWebRequest]::Register() }
        default { throw 'Unknown test operation' }
    }
    $actual = [Net.ServicePointManager]::SecurityProtocol
    if ($actual -ne $expected) { throw ""SecurityProtocol changed from $expected to $actual"" }
} catch {
    [Console]::Error.WriteLine($_.ToString())
    exit 1
}
";
            string encodedScript = Convert.ToBase64String(Encoding.Unicode.GetBytes(script));
            string powerShell = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.System),
                @"WindowsPowerShell\v1.0\powershell.exe");
            var startInfo = new ProcessStartInfo(powerShell, "-NoProfile -NonInteractive -EncodedCommand " + encodedScript)
            {
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true
            };

            using (var process = Process.Start(startInfo))
            {
                if (!process.WaitForExit(30000))
                {
                    process.Kill();
                    Assert.Fail("The isolated TLS regression process timed out.");
                }

                string output = process.StandardOutput.ReadToEnd();
                string error = process.StandardError.ReadToEnd();
                Assert.That(process.ExitCode, Is.EqualTo(0), output + Environment.NewLine + error);
            }
        }
    }
}
#endif
