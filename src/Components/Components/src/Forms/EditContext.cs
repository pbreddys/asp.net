// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.AspNetCore.Components.Forms
{
    /// <summary>
    /// Holds state related to a data editing process.
    /// </summary>
    public class EditContext
    {
        private Dictionary<FieldIdentifier, FieldState> _fieldStates = new Dictionary<FieldIdentifier, FieldState>();

        public EditContext(object model)
        {
            Model = model ?? throw new ArgumentNullException(nameof(model));
        }

        public object Model { get; } // TODO: Is this really needed?

        public event EventHandler<FieldIdentifier> FieldChanged;

        public event EventHandler ValidationRequested;

        public ValidationSource CreateValidationSource()
            => new ValidationSource(this);

        public bool IsModified()
            => _fieldStates.Values.Any(s => s.IsModified);

        public bool IsModified(FieldIdentifier fieldIdentifier)
            => _fieldStates.TryGetValue(fieldIdentifier, out var statesByFieldEntry)
            ? statesByFieldEntry.IsModified
            : false;

        public void ClearModifications()
        {
            foreach (var fieldState in _fieldStates.Values)
            {
                fieldState.IsModified = false;
            }
        }

        public bool IsValid()
            => !ValidationErrorMessages().Any();

        public bool IsValid(FieldIdentifier fieldIdentifier)
            => !ValidationErrorMessages(fieldIdentifier).Any();

        public IEnumerable<string> ValidationErrorMessages()
            => _fieldStates.Values.SelectMany(state => state.ValidationErrorMessages);

        public IEnumerable<string> ValidationErrorMessages(FieldIdentifier fieldIdentifier)
            => _fieldStates.TryGetValue(fieldIdentifier, out var statesByFieldEntry)
            ? statesByFieldEntry.ValidationErrorMessages
            : Enumerable.Empty<string>();
        
        public void NotifyFieldChanged(FieldIdentifier fieldIdentifier)
        {
            GetState(fieldIdentifier, true).IsModified = true;
            FieldChanged?.Invoke(this, fieldIdentifier);
        }

        public bool Validate()
        {
            ValidationRequested?.Invoke(this, null);
            return IsValid();
        }

        internal FieldState GetState(FieldIdentifier identifier, bool ensureExists)
        {
            if (_fieldStates.TryGetValue(identifier, out var result))
            {
                return result;
            }
            else if (ensureExists)
            {
                result = new FieldState();
                _fieldStates.Add(identifier, result);
                return result;
            }
            else
            {
                return null;
            }
        }
    }
}
