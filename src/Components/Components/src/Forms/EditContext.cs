// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace Microsoft.AspNetCore.Components.Forms
{
    /// <summary>
    /// Holds state related to a data editing process.
    /// </summary>
    public class EditContext
    {
        private Dictionary<FieldIdentifier, FieldState> _fieldStates = new Dictionary<FieldIdentifier, FieldState>();

        public event EventHandler<FieldIdentifier> FieldChanged;

        public event Action ValidationRequested;

        public event Action ValidationPerformed;

        public object Model { get; }

        public EditContext(object model)
        {
            Model = model;

            // Recognize INotifyPropertyChanged
            // This won't work recursively, so if you want to react to PropertyChanged on descendant objects, you'd need to use some
            // kind of <PropertyChangeListener Model=@mything.ChildObject /> that does this
            if (Model is INotifyPropertyChanged notifyPropertySource)
            {
                notifyPropertySource.PropertyChanged += (sender, eventInfo) =>
                {
                    NotifyFieldChanged(FieldIdentifier.Create(sender, eventInfo.PropertyName));
                };
            }
        }

        public ValidationMessagesDictionary CreateValidationMessagesDictionary()
            => new ValidationMessagesDictionary(this);

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

        public void NotifyValidationPerformed()
        {
            ValidationPerformed?.Invoke();
        }

        public bool Validate()
        {
            ValidationRequested?.Invoke();
            return IsValid();
        }

        public string ClassName(FieldIdentifier fieldIdentifier)
        {
            if (IsValid(fieldIdentifier))
            {
                return IsModified(fieldIdentifier) ? "valid modified" : "valid";
            }
            else
            {
                return IsModified(fieldIdentifier) ? "invalid modified" : "invalid";
            }
        }

        internal FieldState GetState(FieldIdentifier identifier, bool ensureExists)
        {
            if (_fieldStates.TryGetValue(identifier, out var result))
            {
                return result;
            }
            else if (ensureExists)
            {
                result = new FieldState(identifier);
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
