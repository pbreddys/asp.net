// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Linq;

namespace Microsoft.AspNetCore.Components.Forms
{
    internal class FieldState
    {
        private Dictionary<ValidationSource, List<string>> _validationErrorMessages { get; } = new Dictionary<ValidationSource, List<string>>(); // TODO: Instantiate lazily

        public bool IsModified { get; set; }

        public IEnumerable<string> ValidationErrorMessages
            => _validationErrorMessages.Values.SelectMany(x => x);

        public List<string> GetValidationErrorMessagesForSource(ValidationSource source, bool ensureExists)
        {
            if (_validationErrorMessages.TryGetValue(source, out var result))
            {
                return result;
            }
            else if (ensureExists)
            {
                result = new List<string>();
                _validationErrorMessages.Add(source, result);
                return result;
            }
            else
            {
                return null;
            }
        }

        public void ClearValidationErrorMessagesForSource(ValidationSource source)
        {
            _validationErrorMessages.Remove(source);
        }

        // Consider also additional a general property bag here, with the ability for EditContext
        // to be able to query across all field states for a given object key. This could be used
        // to track arbitrary per-field information, with library authors adding extension methods
        // on EditContext to query for that information.
    }
}
