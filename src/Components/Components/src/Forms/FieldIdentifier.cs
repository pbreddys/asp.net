// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;

namespace Microsoft.AspNetCore.Components.Forms
{
    /// <summary>
    /// Uniquely identifies a single field that can be edited. This may correspond to a property on a .NET model object,
    /// or may be any other named value.
    /// </summary>
    public readonly struct FieldIdentifier
    {
        /// <summary>
        /// Returns a <see cref="FieldIdentifier"/> matching the specified named model value.
        /// </summary>
        /// <param name="model">The reference-typed object that owns the field.</param>
        /// <param name="fieldName">The name of the editable field.</param>
        /// <returns>The corresponding <see cref="FieldIdentifier"/>.</returns>
        public static FieldIdentifier Create<TModel>(TModel model, string fieldName) where TModel : class
            => new FieldIdentifier(model, fieldName);

        // The constructor is private to enforce usage of FieldIdentifier.Create<TModel>, which in turn enforces that the model
        // is reference-typed (otherwise we may have unintentional clashes of FieldIdentifier)
        private FieldIdentifier(object model, string fieldName)
        {
            if (string.IsNullOrEmpty(fieldName))
            {
                throw new ArgumentException("The value cannot be null or empty.", nameof(fieldName));
            }

            Model = model ?? throw new ArgumentNullException(nameof(model));
            FieldName = fieldName;
        }

        /// <summary>
        /// Gets the object that owns the editable field.
        /// </summary>
        public object Model { get; }

        /// <summary>
        /// Gets the name of the editable field.
        /// </summary>
        public string FieldName { get; }

        /// <inheritdoc />
        public override int GetHashCode()
            => (Model, FieldName).GetHashCode();

        /// <inheritdoc />
        public override bool Equals(object obj)
            => obj is FieldIdentifier otherIdentifier
            && otherIdentifier.Model == Model
            && string.Equals(otherIdentifier.FieldName, FieldName, StringComparison.Ordinal);
    }
}
