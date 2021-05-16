using CSharpToolkit.Extensions;
using CSharpToolkit.Utilities;

namespace CSharpToolkit.ViewModels {
    public class LengthLimitedTextViewModel : SingleFieldViewModel<string> {

        int _lengthLimit = 1000;

        public int LengthLimit {
            get { return _lengthLimit ; }
            set {
                if (Perform.ReplaceIfDifferent(ref _lengthLimit, value).WasSuccessful) {
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(RemainingCharacters));
                }
            }
        }

        public override string Field {
            get { return base.Field; }
            set {
                base.Field = value?.SafeSubstring(0, LengthLimit);
                OnPropertyChanged(nameof(RemainingCharacters));
            }
        }

        public int RemainingCharacters => LengthLimit - (Field?.Length ?? 0);

    }
}
