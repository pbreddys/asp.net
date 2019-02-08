// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;

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

        public List<string> this[FieldIdentifier fieldIdentifier]
        {
            get
            {
                if (!_messagesByField.TryGetValue(fieldIdentifier, out var result))
                {
                    result = new List<string>();
                    _messagesByField.Add(fieldIdentifier, result);

                    // Also attach ourself to the FieldState, so we can find these messages when querying across all dictionaries
                    _editContext.GetState(fieldIdentifier, true).AddValidationMessagesDictionary(this);
                }

                return result;
            }
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
