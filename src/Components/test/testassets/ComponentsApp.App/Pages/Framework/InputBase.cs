using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using System;
using System.Linq.Expressions;

namespace ComponentsApp.App.Pages
{
    public class InputBase : ComponentBase
    {
        protected FieldIdentifier FieldIdentifier { get; private set; }

        [CascadingParameter] protected EditContext EditContext { get; private set; }
        [Parameter] string Value { get; set; }
        [Parameter] Action<string> ValueChanged { get; set; }
        [Parameter] Expression<Func<string>> ValueExpression { get; set; }

        protected override void OnParametersSet()
        {
            FieldIdentifier = ValueExpression == null ? default : FieldIdentifier.Create(ValueExpression);
        }

        protected string CurrentValue
        {
            get => Value;
            set
            {
                if (!string.Equals(Value, value, StringComparison.Ordinal))
                {
                    Value = value;
                    ValueChanged?.Invoke(value);
                    EditContext?.NotifyFieldChanged(FieldIdentifier);
                }
            }
        }

        protected string CssClass
            => EditContext?.ClassName(FieldIdentifier);
    }
}
