// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Linq;

namespace Microsoft.AspNetCore.Components.Forms
{
    internal class FieldState
    {
        private FieldIdentifier _fieldIdentifier;
        private HashSet<ValidationMessagesDictionary> _validationSources { get; } = new HashSet<ValidationMessagesDictionary>(); // TODO: Instantiate lazily

        public FieldState(FieldIdentifier fieldIdentifier)
        {
            _fieldIdentifier = fieldIdentifier;
        }

        public bool IsModified { get; set; }

        public IEnumerable<string> ValidationErrorMessages
            => _validationSources.SelectMany(source => source[_fieldIdentifier]);

        public void AddValidationMessagesDictionary(ValidationMessagesDictionary source)
        {
            _validationSources.Add(source);
        }

        public void RemoveValidationMessagesDictionary(ValidationMessagesDictionary source)
        {
            _validationSources.Remove(source);
        }

        // Consider also additional a general property bag here, with the ability for EditContext
        // to be able to query across all field states for a given object key. This could be used
        // to track arbitrary per-field information, with library authors adding extension methods
        // on EditContext to query for that information.
    }
}
