// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;

namespace Microsoft.AspNetCore.Components.Forms
{
    /// <summary>
    /// Provides the ability to set state information for fields within an <see cref="EditContext"/>.
    /// </summary>
    public class EditScope
    {
        private readonly EditContext _editContext;

        // Internal because these should only be created via editContext.CreateScope()
        internal EditScope(EditContext editContext)
        {
            _editContext = editContext ?? throw new ArgumentNullException(nameof(editContext));
        }

        /// <summary>
        /// Gets the <see cref="TFieldState"/> for each field within the <see cref="EditScope"/>.
        /// </summary>
        /// <param name="fieldIdentifier">An identifier for the field.</param>
        /// <returns>The <see cref="TFieldState"/> for the field within this <see cref="EditScope"/>.</returns>
        public FieldState this[FieldIdentifier fieldIdentifier]
        {
            get
            {
                // We create the FieldState instances lazily so that we don't need to track
                // unnecessary (scope, field) pairs. But once they are created they will remain
                // until the scope asks for it to be cleared.
                var existingState = _editContext.GetState(this, fieldIdentifier);
                if (existingState == null)
                {
                    existingState = new FieldState();
                    _editContext.SetState(this, fieldIdentifier, existingState);
                }

                return existingState;
            }
        }

        /// <summary>
        /// Clears all data stored within this <see cref="EditScope"/>.
        /// </summary>
        public void Clear()
            => _editContext.ClearState(this);

        /// <summary>
        /// Removes any <see cref="TFieldState"/> stored for the specified field within this <see cref="EditScope"/>.
        /// </summary>
        public void Clear(FieldIdentifier fieldIdentifier)
            => _editContext.ClearState(this, fieldIdentifier);
    }
}
