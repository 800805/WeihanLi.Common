﻿using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using WeihanLi.Common.Helpers;
using Xunit;

namespace WeihanLi.Common.Test.HelpersTest
{
    public class ProcessExecutorTest
    {
        [Fact]
        public void DotnetInfoTest()
        {
            using var executor = new ProcessExecutor("dotnet", "--info");
            var list = new List<string>();
            executor.OnOutputDataReceived += (sender, str) =>
            {
                list.Add(str);
            };
            var exitCode = -1;
            executor.OnExited += (sender, code) =>
            {
                exitCode = code;
            };
            executor.Execute();

            Assert.NotEmpty(list);
            Assert.Equal(0, exitCode);
        }

        [Fact]
        public async Task DotnetInfoAsyncTest()
        {
            using var executor = new ProcessExecutor("dotnet", "--info");
            var list = new List<string>();
            executor.OnOutputDataReceived += (sender, str) =>
            {
                list.Add(str);
            };
            var exitCode = -1;
            executor.OnExited += (sender, code) =>
            {
                exitCode = code;
            };
            await executor.ExecuteAsync();

            Assert.NotEmpty(list);
            Assert.Equal(0, exitCode);
        }

        [Fact]
        public async Task HostNameTest()
        {
            using var executor = new ProcessExecutor("hostName");
            var list = new List<string>();
            executor.OnOutputDataReceived += (sender, str) =>
            {
                list.Add(str);
            };
            var exitCode = -1;
            executor.OnExited += (sender, code) =>
            {
                exitCode = code;
            };
            await executor.ExecuteAsync();
            Assert.NotEmpty(list);

            var hostName = Dns.GetHostName();
            Assert.Contains(list, x => hostName.Equals(x));
            Assert.Equal(0, exitCode);
        }

        [Fact]
        public async Task EnvironmentVariablesTest()
        {
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                return;
            }
            using var executor = new ProcessExecutor(new ProcessStartInfo("powershell", "-Command \"Write-Host $env:TestUser\"")
            {
                Environment =
                {
                    { "TestUser", "Alice" }
                }
            });
            var list = new List<string>();
            executor.OnOutputDataReceived += (sender, str) =>
            {
                if (str != null)
                {
                    list.Add(str);
                }
            };
            var exitCode = -1;
            executor.OnExited += (sender, code) =>
            {
                exitCode = code;
            };
            await executor.ExecuteAsync();
            Assert.NotEmpty(list);

            Assert.Contains(list, x => "Alice".Equals(x));
            Assert.Equal(0, exitCode);
        }
    }
}
