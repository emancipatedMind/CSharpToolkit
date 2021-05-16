namespace CSharpToolkit.DataAccess {
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Utilities;
    using Abstractions;
    /// <summary>
    /// Immutable clause builder. Each clause returned is unique, so each may be reused for building, or for adding to.
    /// </summary>
    public class Clause {
        ClauseType _type;
        KeyValuePair<string, object>[] _parameters;
        string[] _clauses;

        private Clause(IEnumerable<string> clauses, IEnumerable<KeyValuePair<string, object>> parameters, ClauseType type) {
            _clauses = clauses.ToArray();
            _parameters = parameters.ToArray();
            _type = type;
        }

        /// <summary>
        /// Create new clause.
        /// </summary>
        /// <param name="type">Specify clause type.</param>
        /// <returns>New clause.</returns>
        public static Clause New(ClauseType type) =>
            new Clause(new string[0], new KeyValuePair<string, object>[0], type);

        /// <summary>
        /// Create new AND clause.
        /// </summary>
        /// <returns>New clause.</returns>
        public static Clause New() =>
            New(ClauseType.AND);

        /// <summary>
        /// Build simple data order from clauses, and parameters.
        /// </summary>
        /// <returns>A simple data order of the clause.</returns>
        public SimpleDataOrder Build() {
            string clauseLink = " ";
            Func<string, string> clauseWrapper = s => s;
            switch (_type) {
                case ClauseType.AND:
                    clauseLink = $"{Environment.NewLine}AND";
                    break;
                case ClauseType.OR:
                    clauseLink = $"{Environment.NewLine}    OR";
                    clauseWrapper = s => $"({Environment.NewLine}    {s}{Environment.NewLine})";
                    break;
            }

            string clause = string.Join(clauseLink, _clauses).Trim();
            if (string.IsNullOrWhiteSpace(clause) == false) {
                if (_clauses.Length > 1)
                    clause = clauseWrapper(clause);
                clause = Format(clause);
            }
            return new SimpleDataOrder(clause, _parameters.ToArray());
        }

        /// <summary>
        /// Add clause with auto-generated parameter name.
        /// </summary>
        /// <param name="value">Parameter value.</param>
        /// <param name="clause">The clause for which the parameter name will be inserted.</param>
        /// <returns>Modified clause.</returns>
        public Clause AddClause(object value, Func<string, string> clause) =>
            AddClause(GenerateParameterName(), value, clause);

        /// <summary>
        /// Add clause that contains parameter.
        /// </summary>
        /// <param name="parameterName">Parameter name.</param>
        /// <param name="value">Parameter value.</param>
        /// <param name="clause">The clause for which the parameter name will be inserted.</param>
        /// <returns>Modified clause.</returns>
        public Clause AddClause(string parameterName, object value, Func<string, string> clause) =>
            AddClause(parameterName, value, clause, obj => true);

        /// <summary>
        /// Add previous built clause to this clause.
        /// </summary>
        /// <param name="clause">Previously built clause.</param>
        /// <returns>Modified clause.</returns>
        public Clause AddClause(Clause clause) =>
            AddClause(clause, () => true);

        /// <summary>
        /// Add unparameterized clause.
        /// </summary>
        /// <param name="clause">Simple clause.</param>
        /// <returns>Modified clause.</returns>
        public Clause AddClause(string clause) =>
            AddClause(clause, () => true);

        /// <summary>
        /// Add an implementation of the <see cref="ISimpleDataOrder"/> implementation. If condition returns true, clause is added. Otherwise, it is not.
        /// </summary>
        /// <param name="order">Simple data order to add to clause.</param>
        /// <returns>Modified clause if condition was true. Otherwise, returns input clause.</returns>
        public Clause AddClause(ISimpleDataOrder order) =>
            AddClause(order, () => true);

        /// <summary>
        /// Conditionally add clause with auto-generated parameter name. If condition returns true, clause is added. Otherwise, it is not.
        /// </summary>
        /// <param name="value">Parameter value.</param>
        /// <param name="clause">The clause for which the parameter name will be inserted.</param>
        /// <param name="condition">Clause condition which determines if clause will be inserted. The parameter passed into this is the value parameter of this method. Returning true adds clause, false skips it.</param>
        /// <returns>Modified clause if condition was true. Otherwise, returns input clause.</returns>
        public Clause AddClause(object value, Func<string, string> clause, Func<object, bool> condition) =>
            AddClause(GenerateParameterName(), value, clause, condition);

        /// <summary>
        /// Conditionally add clause that contains parameter. If condition returns true, clause is added. Otherwise, it is not.
        /// </summary>
        /// <param name="parameterName">Parameter name.</param>
        /// <param name="value">Parameter value.</param>
        /// <param name="clause">The clause for which the parameter name will be inserted.</param>
        /// <param name="condition">Clause condition which determines if clause will be inserted. The parameter passed into this is the value parameter of this method. Returning true adds clause, false skips it.</param>
        /// <returns>Modified clause if condition was true. Otherwise, returns input clause.</returns>
        public Clause AddClause(string parameterName, object value, Func<string, string> clause, Func<object, bool> condition) {
            bool conditionFailed = condition(value) == false;
            if (conditionFailed)
                return this;

            if (string.IsNullOrWhiteSpace(parameterName))
                parameterName = GenerateParameterName();

            string clauseText = clause(parameterName.Trim());
            if (string.IsNullOrWhiteSpace(clauseText))
                return this;

            return GetClone(clauseText, new[] { new KeyValuePair<string, object>(parameterName, value) });
        }

        /// <summary>
        /// Conditionally add previous built clause to this clause. If condition returns true, clause is added. Otherwise, it is not.
        /// </summary>
        /// <param name="clause">Previously built clause.</param>
        /// <param name="condition">Clause condition which determines if clause will be inserted. The parameter passed into this is the value parameter of this method. Returning true adds clause, false skips it.</param>
        /// <returns>Modified clause if condition was true. Otherwise, returns input clause.</returns>
        public Clause AddClause(Clause clause, Func<bool> condition) {
            bool conditionFailed = condition() == false;
            if (conditionFailed)
                return this;

            SimpleDataOrder dataOrder = clause.Build();
            if (string.IsNullOrWhiteSpace(dataOrder.Query))
                return this;
            return GetClone(string.Join($"{Environment.NewLine}    ", dataOrder.Query.Split(new[] { Environment.NewLine, "\r\n" , "\r", "\n" }, StringSplitOptions.None)), dataOrder.Parameters);
        }

        /// <summary>
        /// Conditionally add unparameterized clause. If condition returns true, clause is added. Otherwise, it is not.
        /// </summary>
        /// <param name="clause">Simple clause.</param>
        /// <param name="condition">Clause condition which determines if clause will be inserted. The parameter passed into this is the value parameter of this method. Returning true adds clause, false skips it.</param>
        /// <returns>Modified clause if condition was true. Otherwise, returns input clause.</returns>
        public Clause AddClause(string clause, Func<bool> condition) {
            bool conditionFailed = condition() == false || string.IsNullOrWhiteSpace(clause);
            if (conditionFailed)
                return this;

            return GetClone(clause, new KeyValuePair<string, object>[0]);
        }

        /// <summary>
        /// Conditionally add an implementation of the <see cref="ISimpleDataOrder"/> implementation. If condition returns true, clause is added. Otherwise, it is not.
        /// </summary>
        /// <param name="order">Simple data order to add to clause.</param>
        /// <param name="condition">Clause condition which determines if clause will be inserted. The parameter passed into this is the value parameter of this method. Returning true adds clause, false skips it.</param>
        /// <returns>Modified clause if condition was true. Otherwise, returns input clause.</returns>
        public Clause AddClause(ISimpleDataOrder order, Func<bool> condition) {
            bool conditionFailed = condition() == false || string.IsNullOrWhiteSpace(order.Query);
            if (conditionFailed)
                return this;

            return GetClone(order.Query, order.Parameters);
        }

        private Clause GetClone(string clause, IEnumerable<KeyValuePair<string, object>> parameters) =>
            new Clause(_clauses.Concat(new[] { Format(clause) }), _parameters.Concat(parameters), _type);

        /// <summary>
        /// Generates random unique parameter name.
        /// </summary>
        /// <returns>Random unique parameter name.</returns>
        public static string GenerateParameterName() =>
            "@" + Get.SafeGuid();

        /// <summary>
        /// Trims trailing whitespaces, and adds single white space as header.
        /// </summary>
        /// <param name="clause"></param>
        /// <returns>Formatted string.</returns>
        public static string Format(string clause) =>
            " " + clause.Trim();

        /// <summary>
        /// Produces an equal statement of the form "tableAlias.columnName = {x}".
        /// </summary>
        /// <param name="columnName">Column Name</param>
        /// <param name="tableAlias">Table Alias</param>
        /// <returns>Function producing statement.</returns>
        public static Func<string, string> EqualStatementCallback(string columnName, string tableAlias = "") =>
            x => $"{(string.IsNullOrWhiteSpace(tableAlias) ? "" : $"{tableAlias}.") }{columnName} = {x}";

        /// <summary>
        /// Produces an equal statement of the form "CONVERT(DATE, tableAlias.columnName) = CONVERT(DATE, {x})".
        /// </summary>
        /// <param name="columnName">Column Name</param>
        /// <param name="tableAlias">Table Alias</param>
        /// <returns>Function producing statement.</returns>
        public static Func<string, string> DateEqualStatementCallback(string columnName, string tableAlias = "") =>
            x => $"CONVERT(DATE, {(string.IsNullOrWhiteSpace(tableAlias) ? "" : $"{tableAlias}.") }{columnName}) = CONVERT(DATE, {x})";

        /// <summary>
        /// Produces a not equal statement of the form "tableAlias.columnName &lt;&gt; {x}".
        /// </summary>
        /// <param name="columnName">Column Name</param>
        /// <param name="tableAlias">Table Alias</param>
        /// <returns>Function producing statement.</returns>
        public static Func<string, string> NotEqualStatementCallback(string columnName, string tableAlias = "") =>
            x => $"{(string.IsNullOrWhiteSpace(tableAlias) ? "" : $"{tableAlias}.") }{columnName} <> {x}";

        /// <summary>
        /// Produces a not equal statement of the form "CONVERT(DATE, tableAlias.columnName)  &lt;&gt; CONVERT(DATE, {x})".
        /// </summary>
        /// <param name="columnName">Column Name</param>
        /// <param name="tableAlias">Table Alias</param>
        /// <returns>Function producing statement.</returns>
        public static Func<string, string> NotDateEqualStatementCallback(string columnName, string tableAlias = "") =>
            x => $"CONVERT(DATE, {(string.IsNullOrWhiteSpace(tableAlias) ? "" : $"{tableAlias}.") }{columnName}) <> CONVERT(DATE, {x})";

        /// <summary>
        /// Produces an LIKE statement of the form "tableAlias.columnName LIKE '%' + {x}".
        /// </summary>
        /// <param name="columnName">Column Name</param>
        /// <param name="tableAlias">Table Alias</param>
        /// <returns>Function producing statement.</returns>
        public static Func<string, string> LeadingLikeStatementCallback(string columnName, string tableAlias = "") =>
            x => $"{(string.IsNullOrWhiteSpace(tableAlias) ? "" : $"{tableAlias}.") }{columnName} LIKE '%' + {x}";

        /// <summary>
        /// Produces an LIKE statement of the form "tableAlias.columnName LIKE {x} + '%'".
        /// </summary>
        /// <param name="columnName">Column Name</param>
        /// <param name="tableAlias">Table Alias</param>
        /// <returns>Function producing statement.</returns>
        public static Func<string, string> TrailingLikeStatementCallback(string columnName, string tableAlias = "") =>
            x => $"{(string.IsNullOrWhiteSpace(tableAlias) ? "" : $"{tableAlias}.") }{columnName} LIKE {x} + '%'";

        /// <summary>
        /// Produces an LIKE statement of the form "tableAlias.columnName LIKE '%' + {x} + '%'".
        /// </summary>
        /// <param name="columnName">Column Name</param>
        /// <param name="tableAlias">Table Alias</param>
        /// <returns>Function producing statement.</returns>
        public static Func<string, string> LikeStatementCallback(string columnName, string tableAlias = "") =>
            x => $"{(string.IsNullOrWhiteSpace(tableAlias) ? "" : $"{tableAlias}.") }{columnName} LIKE '%' + {x} + '%'";

        /// <summary>
        /// Produces an equal statement of the form "primaryAlias.primaryColumn = primaryAlias.primaryColumn".
        /// </summary>
        /// <param name="primaryColumn">The primary column.</param>
        /// <param name="secondaryColumn">The column to match the secondary column with.</param>
        /// <param name="primaryAlias">The alias for the primary column's table.</param>
        /// <param name="secondaryAlias">The alias for the secondary column's table.</param>
        /// <returns>Function producing statement.</returns>
        public static Func<string> ColumnsEqualStatementCallback(string primaryColumn, string secondaryColumn, string primaryAlias = "", string secondaryAlias = "") =>
            () => ColumnsEqualStatement(primaryColumn, secondaryColumn, primaryAlias, secondaryAlias);

        /// <summary>
        /// Produces an equal statement of the form "primaryAlias.primaryColumn = primaryAlias.primaryColumn".
        /// </summary>
        /// <param name="primaryColumn">The primary column.</param>
        /// <param name="secondaryColumn">The column to match the secondary column with.</param>
        /// <param name="primaryAlias">The alias for the primary column's table.</param>
        /// <param name="secondaryAlias">The alias for the secondary column's table.</param>
        /// <returns>Produces an equal statement.</returns>
        public static string ColumnsEqualStatement(string primaryColumn, string secondaryColumn, string primaryAlias = "", string secondaryAlias = "") =>
            $"{(string.IsNullOrWhiteSpace(primaryAlias) ? "" : $"{primaryAlias}.") }{primaryColumn} = {(string.IsNullOrWhiteSpace(secondaryAlias) ? "" : $"{secondaryAlias}.")}{secondaryColumn}";

        /// <summary>
        /// Produces a not equal statement of the form "primaryAlias.primaryColumn = primaryAlias.primaryColumn".
        /// </summary>
        /// <param name="primaryColumn">The primary column.</param>
        /// <param name="secondaryColumn">The column to match the secondary column with.</param>
        /// <param name="primaryAlias">The alias for the primary column's table.</param>
        /// <param name="secondaryAlias">The alias for the secondary column's table.</param>
        /// <returns>Function producing statement.</returns>
        public static Func<string> ColumnsNotEqualStatementCallback(string primaryColumn, string secondaryColumn, string primaryAlias = "", string secondaryAlias = "") =>
            () => ColumnsNotEqualStatement(primaryColumn, secondaryColumn, primaryAlias, secondaryAlias);

        /// <summary>
        /// Produces a not equal statement of the form "primaryAlias.primaryColumn = primaryAlias.primaryColumn".
        /// </summary>
        /// <param name="primaryColumn">The primary column.</param>
        /// <param name="secondaryColumn">The column to match the secondary column with.</param>
        /// <param name="primaryAlias">The alias for the primary column's table.</param>
        /// <param name="secondaryAlias">The alias for the secondary column's table.</param>
        /// <returns>Produces an equal statement.</returns>
        public static string ColumnsNotEqualStatement(string primaryColumn, string secondaryColumn, string primaryAlias = "", string secondaryAlias = "") =>
            $"{(string.IsNullOrWhiteSpace(primaryAlias) ? "" : $"{primaryAlias}.") }{primaryColumn} <> {(string.IsNullOrWhiteSpace(secondaryAlias) ? "" : $"{secondaryAlias}.")}{secondaryColumn}";

        /// <summary>
        /// Produces NOT IN statement of the form "tableAlias.columnName NOT IN (values[0], values[1])".
        /// </summary>
        /// <param name="columnName">The name of the column.</param>
        /// <param name="values">The values to check for.</param>
        /// <param name="tableAlias">The alias of the table. If empty, will not be included.</param>
        /// <returns><see cref="ISimpleDataOrder"/> implementation containing query produced, and parameters.</returns>
        public static ISimpleDataOrder NotInStatement(string columnName, IEnumerable<object> values, string tableAlias = "") {
            string nullKey = $"@null{Get.SafeGuid()}";
            var parameters = values.GroupBy(value => value).Where(group => group.Key != null).Select(group => new Tuple<string, object>("@" + Get.SafeGuid(), group.Key)).ToList();
            var list = values.Select(value => parameters.FirstOrDefault(param => param.Item2.Equals(value))?.Item1 ?? nullKey).ToArray();

            string statement = $"{(string.IsNullOrWhiteSpace(tableAlias) ? "": $"{tableAlias}.")}{columnName} NOT IN ({string.Join(", ", list)})";

            if (statement.Contains(nullKey))
                parameters.Insert(0, Tuple.Create<string, object>(nullKey, null));
            return new SimpleDataOrder(statement, parameters.Select(param => new KeyValuePair<string, object>(param.Item1, param.Item2)));
        }

        /// <summary>
        /// Produces IN statement of the form "tableAlias.columnName IN (values[0], values[1])".
        /// </summary>
        /// <param name="columnName">The name of the column.</param>
        /// <param name="values">The values to check for.</param>
        /// <param name="tableAlias">The alias of the table. If empty, will not be included.</param>
        /// <returns><see cref="ISimpleDataOrder"/> implementation containing query produced, and parameters.</returns>
        public static ISimpleDataOrder InStatement(string columnName, IEnumerable<object> values, string tableAlias = "") {
            if ((values?.Any() ?? false) == false)
                return new SimpleDataOrder();
            string nullKey = $"@null{Get.SafeGuid()}";
            var parameters = values.GroupBy(value => value).Where(group => group.Key != null).Select(group => new Tuple<string, object>(Clause.GenerateParameterName(), group.Key)).ToList();
            var list = values.Select(value => parameters.FirstOrDefault(param => param.Item2.Equals(value))?.Item1 ?? nullKey).ToArray();

            string statement = $"{(string.IsNullOrWhiteSpace(tableAlias) ? "": $"{tableAlias}.")}{columnName} IN ({string.Join(", ", list)})";

            if (statement.Contains(nullKey))
                parameters.Insert(0, Tuple.Create<string, object>(nullKey, null));
            return new SimpleDataOrder(statement, parameters.Select(param => new KeyValuePair<string, object>(param.Item1, param.Item2)));
        }

    }
}