// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Linq.Expressions;

namespace Microsoft.AspNetCore.Components.Forms
{
    public static class EditContextFieldIdentifierExtensions
    {
        public static string ClassName<T>(this EditContext editContext, Expression<Func<T>> forField)
            => editContext.ClassName(FieldIdentifier.Create(forField));
    }
}
