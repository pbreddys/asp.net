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
        private Dictionary<FieldIdentifier, Dictionary<EditScope, FieldState>> _statesByField = new Dictionary<FieldIdentifier, Dictionary<EditScope, FieldState>>();
        private Dictionary<EditScope, Dictionary<FieldIdentifier, FieldState>> _statesByScope = new Dictionary<EditScope, Dictionary<FieldIdentifier, FieldState>>();
        private EditScope _modificationTracker;

        public EditContext(object model)
        {
            Model = model ?? throw new ArgumentNullException(nameof(model));
            _modificationTracker = CreateScope();
        }

        public object Model { get; }

        public event EventHandler<FieldIdentifier> FieldChanged;

        public event EventHandler ValidationRequested;

        /// <summary>
        /// Creates an <see cref="EditScope"/> that can be used to get or set state information for fields within this <see cref="EditContext"/>.
        /// </summary>
        /// <returns>The newly-created <see cref="EditScope"/>.</returns>
        public EditScope CreateScope()
            => new EditScope(this);

        public bool IsModified()
            => _statesByField.Any(statesByFieldEntry => statesByFieldEntry.Value.Any(state => state.Value.IsModified));

        public bool IsModified(FieldIdentifier fieldIdentifier)
            => _statesByField.TryGetValue(fieldIdentifier, out var statesByFieldEntry)
            ? statesByFieldEntry.Values.Any(state => state.IsModified)
            : false;

        public void ClearModifications()
        {
            // This is a bit odd, because if any other EditScope has set flags, then this "clear"
            // method won't do anything. Maybe it shouldn't be native to FieldState, but rather is
            // either some external propertybag-based mechanism, or is just native to the EditContext
            // itself independently of the edit scopes.
            _modificationTracker.Clear();
        }

        public bool IsValid()
            => !ValidationErrorMessages().Any();

        public bool IsValid(FieldIdentifier fieldIdentifier)
            => !ValidationErrorMessages(fieldIdentifier).Any();

        public IEnumerable<string> ValidationErrorMessages()
            => _statesByField.Values.SelectMany(pair => pair.Values).SelectMany(state => state.ValidationErrorMessages);

        public IEnumerable<string> ValidationErrorMessages(FieldIdentifier fieldIdentifier)
            => _statesByField.TryGetValue(fieldIdentifier, out var statesByFieldEntry)
            ? statesByFieldEntry.Values.SelectMany(state => state.ValidationErrorMessages)
            : Enumerable.Empty<string>();

        public void NotifyFieldChanged(FieldIdentifier fieldIdentifier)
        {
            _modificationTracker[fieldIdentifier].IsModified = true;
            FieldChanged?.Invoke(this, fieldIdentifier);
        }

        public bool Validate()
        {
            ValidationRequested?.Invoke(this, null);
            return IsValid();
        }

        internal FieldState GetState(EditScope scope, FieldIdentifier identifier)
            => _statesByScope.TryGetValue(scope, out var statesByScopeEntry)
            && statesByScopeEntry.TryGetValue(identifier, out var result)
            ? result
            : null;

        internal void SetState(EditScope scope, FieldIdentifier fieldIdentifier, FieldState state)
        {
            if (scope == null)
            {
                throw new System.ArgumentNullException(nameof(scope));
            }

            if (state == null)
            {
                throw new System.ArgumentNullException(nameof(state));
            }

            if (!_statesByField.TryGetValue(fieldIdentifier, out var statesByFieldEntry))
            {
                statesByFieldEntry = new Dictionary<EditScope, FieldState>();
                _statesByField.Add(fieldIdentifier, statesByFieldEntry);
            }

            if (!_statesByScope.TryGetValue(scope, out var statesByScopeEntry))
            {
                statesByScopeEntry = new Dictionary<FieldIdentifier, FieldState>();
                _statesByScope.Add(scope, statesByScopeEntry);
            }

            statesByFieldEntry[scope] = state;
            statesByScopeEntry[fieldIdentifier] = state;
        }

        internal void ClearState(EditScope scope, FieldIdentifier fieldIdentifier)
        {
            if (scope == null)
            {
                throw new System.ArgumentNullException(nameof(scope));
            }

            ClearStatesByFieldEntry(scope, fieldIdentifier);
            ClearStatesByScopeEntry(scope, fieldIdentifier);
        }

        internal void ClearState(EditScope scope)
        {
            if (scope == null)
            {
                throw new System.ArgumentNullException(nameof(scope));
            }

            if (_statesByScope.TryGetValue(scope, out var statesByScopeEntry))
            {
                _statesByScope.Remove(scope);

                // The entry we just removed tells us which fields there are corresponding states for,
                // so we don't have to iterate through the complete list of fields
                foreach (var fieldIdentifier in statesByScopeEntry.Keys)
                {
                    ClearStatesByFieldEntry(scope, fieldIdentifier);
                }
            }
        }

        private void ClearStatesByFieldEntry(EditScope scope, FieldIdentifier fieldIdentifier)
        {
            if (_statesByField.TryGetValue(fieldIdentifier, out var statesByFieldIdentifier))
            {
                statesByFieldIdentifier.Remove(scope);

                if (statesByFieldIdentifier.Count == 0)
                {
                    _statesByField.Remove(fieldIdentifier);
                }
            }
        }

        private void ClearStatesByScopeEntry(EditScope scope, FieldIdentifier fieldIdentifier)
        {
            if (_statesByScope.TryGetValue(scope, out var statesByScopeEntry))
            {
                statesByScopeEntry.Remove(fieldIdentifier);

                if (statesByScopeEntry.Count == 0)
                {
                    _statesByScope.Remove(scope);
                }
            }
        }
    }
}
