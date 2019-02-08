// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Linq;

namespace Microsoft.AspNetCore.Components.Forms
{
    public class ValidationMessagesDictionary
    {
        private EditContext _editContext;

        public Dictionary<FieldIdentifier, List<string>> _messagesByField = new Dictionary<FieldIdentifier, List<string>>();

        internal ValidationMessagesDictionary(EditContext editContext)
        {
            _editContext = editContext;
        }

        public IEnumerable<string> this[FieldIdentifier fieldIdentifier]
            => _messagesByField.TryGetValue(fieldIdentifier, out var result)
            ? result
            : Enumerable.Empty<string>();

        public void Add(FieldIdentifier fieldIdentifier, string message)
            => GetOrCreateMessagesListForField(fieldIdentifier).Add(message);

        public void AddRange(FieldIdentifier fieldIdentifier, IEnumerable<string> messages)
            => GetOrCreateMessagesListForField(fieldIdentifier).AddRange(messages);

        private List<string> GetOrCreateMessagesListForField(FieldIdentifier fieldIdentifier)
        {
            if (!_messagesByField.TryGetValue(fieldIdentifier, out var messagesForField))
            {
                messagesForField = new List<string>();
                _messagesByField.Add(fieldIdentifier, messagesForField);

                // Also attach ourself to the FieldState, so we can find these messages when querying across all dictionaries
                _editContext.GetState(fieldIdentifier, true).AddValidationMessagesDictionary(this);
            }

            return messagesForField;
        }

        public void Clear(FieldIdentifier fieldIdentifier)
        {
            _editContext.GetState(fieldIdentifier, false)?.RemoveValidationMessagesDictionary(this);
            _messagesByField.Remove(fieldIdentifier);
        }

        public void Clear()
        {
            foreach (var fieldIdentifier in _messagesByField.Keys)
            {
                _editContext.GetState(fieldIdentifier, false)?.RemoveValidationMessagesDictionary(this);
            }

            _messagesByField.Clear();
        }
    }
}
