// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;

namespace Microsoft.AspNetCore.Components.Forms
{
    public class ValidationSource
    {
        private readonly EditContext _editContext;
        private List<FieldState> _fieldStatesWithErrorMessage = new List<FieldState>(); // TODO: Instantiate lazily

        // Internal because these should only be created via editContext.AddValidationSource()
        internal ValidationSource(EditContext editContext)
        {
            _editContext = editContext ?? throw new ArgumentNullException(nameof(editContext));
        }

        public List<string> this[FieldIdentifier fieldIdentifier]
        {
            get
            {
                var fieldState = _editContext.GetState(fieldIdentifier, true);
                var messages = fieldState.GetValidationErrorMessagesForSource(this, false);
                if (messages == null)
                {
                    _fieldStatesWithErrorMessage.Add(fieldState);
                    messages = fieldState.GetValidationErrorMessagesForSource(this, true);
                }

                return messages;
            }
        }

        public void Clear()
        {
            foreach (var fieldState in _fieldStatesWithErrorMessage)
            {
                fieldState.ClearValidationErrorMessagesForSource(this);
            }

            _fieldStatesWithErrorMessage.Clear();
        }
    }
}
