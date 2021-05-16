namespace CSharpToolkit.ViewModels {
    using System;
    using Abstractions;

    public class NoInfoDialogViewModel : DialogViewModel<EventArgs> {
        protected override EventArgs GetSuccessObject() => EventArgs.Empty;
    }
}