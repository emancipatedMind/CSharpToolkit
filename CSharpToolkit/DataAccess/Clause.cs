namespace CSharpToolkit.DataAccess {
    using System;
    using System.Collections.Generic;
    using System.Linq;
    /// <summary>
    /// Immutable clause builder. Each clause returned is unique, so each may be reused for building, or for adding to.
    /// </summary>
    public struct Clause {
        ClauseType _type;
        List<KeyValuePair<string, object>> _parameters;
        List<string> _clauses;
        object _token;

        private Clause(List<string> clauses, List<KeyValuePair<string, object>> parameters, ClauseType type) {
            _clauses = clauses;
            _parameters = parameters;
            _type = type;
            _token = new object();
        }

        /// <summary>
        /// Create new clause.
        /// </summary>
        /// <param name="type">Specify clause type.</param>
        /// <returns>New clause.</returns>
        public static Clause New(ClauseType type) =>
            new Clause(new List<string>(), new List<KeyValuePair<string, object>>(), type);

        /// <summary>
        /// Build simple data order from clauses, and parameters.
        /// </summary>
        /// <returns></returns>
        public SimpleDataOrder Build() {
            string clauseLink = " ";
            Func<string, string> clauseWrapper = s => s;
            switch (_type) {
                case ClauseType.AND:
                    clauseLink = " AND";
                    break;
                case ClauseType.OR:
                    clauseLink = " OR";
                    clauseWrapper = s => $"({s})";
                    break;
            }

            string clause = string.Join(clauseLink, _clauses).Trim();
            if (string.IsNullOrEmpty(clause) == false) {
                if (_clauses.Count > 1)
                    clause = clauseWrapper(clause);
                clause = Format(clause);
            }
            return new SimpleDataOrder(clause, new List<KeyValuePair<string, object>>(_parameters));
        }

        /// <summary>
        /// Conditionally add clause. If condition returns true, clause is added. Otherwise, it is not.
        /// </summary>
        /// <param name="parameterName">Parameter name.</param>
        /// <param name="value">Parameter value.</param>
        /// <param name="clause">The clause for which the parameter name will be inserted.</param>
        /// <param name="condition">Clause condition. Returning true adds clause, false skips it.</param>
        /// <returns>Modified clause if condition was true. Otherwise, returns input clause.</returns>
        public Clause AddClause(string parameterName, object value, Func<string, string> clause, Func<object, bool> condition) {
            bool conditionSatisfied = condition(value);
            if (conditionSatisfied) {
                AddClause(parameterName, value, clause);
                return Clone();
            }
            return this;
        }

        /// <summary>
        /// Add clause that contains parameter.
        /// </summary>
        /// <param name="parameterName">Parameter name.</param>
        /// <param name="value">Parameter value.</param>
        /// <param name="clause">The clause for which the parameter name will be inserted.</param>
        /// <returns>Modified clause.</returns>
        public Clause AddClause(string parameterName, object value, Func<string, string> clause) {
            AddParameter(parameterName, value);
            AddClause(clause(parameterName.Trim()));
            return Clone();
        }

        /// <summary>
        /// Add previous built clause to this clause.
        /// </summary>
        /// <param name="clause">Previously built clause.</param>
        /// <returns>Modified clause.</returns>
        public Clause AddClause(Clause clause) {
            SimpleDataOrder dataOrder = clause.Build();
            AddClause(dataOrder.Query);
            AddParameter(dataOrder.Parameters);
            return Clone();
        }

        /// <summary>
        /// Add unparameterized clause.
        /// </summary>
        /// <param name="clause">Simple clause.</param>
        /// <returns>Modified clause.</returns>
        public Clause AddClause(string clause) {
            lock (_token) {
                _clauses.Add(Format(clause));
                return Clone();
            }
        }

        private void AddParameter(IEnumerable<KeyValuePair<string, object>> parameters) {
            foreach (var parameter in parameters)
                AddParameter(parameter.Key, parameter.Value);
        }

        private void AddParameter(KeyValuePair<string, object> parameter ) =>
            AddParameter(parameter.Key, parameter.Value);

        private void AddParameter(string key, object value) {
            lock (_token) {
                _parameters.Add(new KeyValuePair<string, object>(key.Trim(), value));
            }
        }

        private Clause Clone() =>
            new Clause(new List<string>(_clauses), new List<KeyValuePair<string, object>>(_parameters), _type);

        /// <summary>
        /// Trims trailing whitespaces, and adds single white space as header.
        /// </summary>
        /// <param name="clause"></param>
        /// <returns></returns>
        public static string Format(string clause) =>
            " " + clause.Trim();

    }
}