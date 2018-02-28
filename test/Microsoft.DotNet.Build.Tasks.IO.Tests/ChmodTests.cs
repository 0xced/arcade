﻿// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.IO;
using System.Runtime.InteropServices;
using Mono.Unix;
using Xunit;
using Xunit.Abstractions;

namespace Microsoft.DotNet.Build.Tasks.IO.Tests
{
    public class ChmodTests
    {
        private readonly ITestOutputHelper _output;

        public ChmodTests(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public void ItExecutesChmod()
        {
            var file = Path.Combine(AppContext.BaseDirectory, Path.GetRandomFileName());
            File.WriteAllText(file, "");

            AssertDoesNotHavePermission(file, FileAccessPermissions.UserExecute);
            var task = new Chmod
            {
                File = file,
                Mode = "+x",
                BuildEngine = new MockEngine(_output),
            };
            Assert.True(task.Execute(), "Task should pass");

            AssertPermission(file, FileAccessPermissions.UserExecute);
        }

        private void AssertPermission(string filepath, FileAccessPermissions permission)
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                return;
            }

            var info = new UnixFileInfo(filepath);
            Assert.True(info.FileAccessPermissions.HasFlag(permission), $"File {filepath} should have permission {permission}");
        }

        private void AssertDoesNotHavePermission(string filepath, FileAccessPermissions permission)
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                return;
            }

            var info = new UnixFileInfo(filepath);
            Assert.False(info.FileAccessPermissions.HasFlag(permission), $"File {filepath} should not have permission {permission}");
        }
    }
}
