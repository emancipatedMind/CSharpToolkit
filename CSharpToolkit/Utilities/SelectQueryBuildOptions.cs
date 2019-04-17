namespace CSharpToolkit.Utilities {
    using System;
    using System.Collections.Generic; 
    /// <summary>
    /// The build options to be used by SelectQuery.
    /// </summary>
    public class SelectQueryBuildOptions {

        /// <summary>
        /// Instantiates <see cref="SelectQueryBuildOptions"/>.
        /// </summary>
        public SelectQueryBuildOptions() { }
        /// <summary>
        /// Instantiates <see cref="SelectQueryBuildOptions"/>.
        /// </summary>
        /// <param name="collection">Fills order by list.</param>
        public SelectQueryBuildOptions(IEnumerable<Tuple<string, bool>> collection) {
            OrderBy.AddRange(collection ?? new Tuple<string, bool>[0]);
        }

        /// <summary>
        /// Whether only distinct records should be returned.
        /// </summary>
        public bool DistinctOnly { get; set; }
        /// <summary>
        /// The number of records to limit the query to.
        /// </summary>
        public int? Limit { get; set; }
        /// <summary>
        /// How to order the returned rows. Item1 is the column, and Item2 is ASC(false) or DESC(true). 
        /// </summary>
        public List<Tuple<string, bool>> OrderBy { get; } = new List<Tuple<string, bool>>();
    }
}