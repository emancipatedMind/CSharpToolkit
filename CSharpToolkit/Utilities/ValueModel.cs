namespace CSharpToolkit.Utilities {
    public class ValueModel<TValue> {

        public ValueModel(string user, int id, TValue value) : this(user, System.DateTime.Now, id, value) { }
        public ValueModel(string user, System.DateTime updateTime, int id, TValue value) {
            UserCode = user;
            UpdateTime = updateTime;
            Id = id;
            Value = value;
        }

        public int Id { get; set; }
        public TValue Value { get; set; }
        public System.DateTime UpdateTime { get; set; }
        public string UserCode { get; set; }

    }
}
