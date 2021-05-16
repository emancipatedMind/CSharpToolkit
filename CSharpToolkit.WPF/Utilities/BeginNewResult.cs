namespace CSharpToolkit.Utilities {
    using Utilities;
    public class BeginNewResult<TResult> {

        public BeginNewResult()  { }
        public BeginNewResult(int id) {
            Id = id;
            Type = BeginNewType.Refresh;
        }
        public BeginNewResult(TResult model) {
            Model = model;
            Type = BeginNewType.Load;
        }

        public int Id { get; }
        public TResult Model { get; }
        public BeginNewType Type { get; } = BeginNewType.None;

    }
}
