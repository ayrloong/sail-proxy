﻿// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Kubernetes.Testing.Models;

namespace Microsoft.Kubernetes.Testing;

public interface ITestCluster
{
    Task UnhandledRequest(HttpContext context);

    Task<ListResult> ListResourcesAsync(string group, string version, string plural, ListParameters parameters);
}
