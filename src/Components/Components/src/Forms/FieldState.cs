// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;

namespace Microsoft.AspNetCore.Components.Forms
{
    public class FieldState
    {
        public List<string> ValidationErrorMessages { get; } = new List<string>();

        public bool IsModified;

        // Consider also additional a general property bag here, with the ability for EditContext
        // to be able to query across all field states for a given object key. This could be used
        // to track arbitrary per-field information, with library authors adding extension methods
        // on EditContext to query for that information.
    }
}
