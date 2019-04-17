namespace CSharpToolkit.Utilities {
    /// <summary>
    /// Gets the text "LEFT".
    /// </summary>
    public class LeftJoin : Abstractions.IJoinType {
        /// <summary>
        /// The method that provides the join text. 
        /// </summary>
        /// <returns>"LEFT"</returns>
        public string GetJoinText() => "LEFT";
    }
    /// <summary>
    /// Gets the text "RIGHT".
    /// </summary>
    public class RightJoin : Abstractions.IJoinType {
        /// <summary>
        /// The method that provides the join text. 
        /// </summary>
        /// <returns>"RIGHT"</returns>
        public string GetJoinText() => "RIGHT";
    }
    /// <summary>
    /// Gets the text "FULL OUTER".
    /// </summary>
    public class FullOuterJoin : Abstractions.IJoinType {
        /// <summary>
        /// The method that provides the join text. 
        /// </summary>
        /// <returns>"FULL OUTER"</returns>
        public string GetJoinText() => "FULL OUTER";
    }
    /// <summary>
    /// Gets the text "INNER".
    /// </summary>
    public class InnerJoin : Abstractions.IJoinType {
        /// <summary>
        /// The method that provides the join text. 
        /// </summary>
        /// <returns>"INNER"</returns>
        public string GetJoinText() => "INNER";
    }
    /// <summary>
    /// A place holder the IJoinType interface. Implements Singleton pattern.
    /// </summary>
    public class NullJoin : Abstractions.IJoinType {
        private static NullJoin _instance = new NullJoin();
        /// <summary>
        /// An instance of the NullJoin class.
        /// </summary>
        public static NullJoin Instance => _instance;
        private NullJoin() { }
        /// <summary>
        /// The method that provides the join text. 
        /// </summary>
        /// <returns>Returns empty string.</returns>
        public string GetJoinText() => "";
    }
}