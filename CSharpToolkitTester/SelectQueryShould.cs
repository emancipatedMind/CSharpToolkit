namespace CSharpToolkitTester {
    using CSharpToolkit.DataAccess;
    using CSharpToolkit.Utilities;
    using NUnit.Framework;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    [TestFixture]
    public class SelectQueryShould {

        [Test]
        public void ProduceCorrectSQLFromSingleSelectQueryOrderWithNoTableAlias() {
            var columns = new string[] { "ID", "Name" };

            var baseQuery = SelectQuery.New(new SelectQueryOrder("Table", "", columns));

            var dataOrder = baseQuery.Build();

            string expectedSQL = "SELECT\r\n    Table.ID AS TableID,\r\n    Table.Name AS TableName\r\nFROM Table";

            Assert.AreEqual(expectedSQL, dataOrder.Query);
            Assert.IsFalse(dataOrder.Parameters.Any());
            Assert.AreEqual(2, dataOrder.Aliases.Count());
            Assert.AreEqual("Table", dataOrder.Aliases.ElementAt(0).Name);
            Assert.AreEqual("ID", dataOrder.Aliases.ElementAt(0).Data.Item1);
            Assert.AreEqual("TableID", dataOrder.Aliases.ElementAt(0).Data.Item2);
            Assert.AreEqual("Table", dataOrder.Aliases.ElementAt(1).Name);
            Assert.AreEqual("Name", dataOrder.Aliases.ElementAt(1).Data.Item1);
            Assert.AreEqual("TableName", dataOrder.Aliases.ElementAt(1).Data.Item2);
        }

        [Test]
        public void ProduceCorrectSQLFromSingleSelectQueryOrderWithTableAlias() {
            var columns = new string[] { "ID", "Name" };

            var baseQuery = SelectQuery.New(new SelectQueryOrder("Table", "TableAlias", columns));

            var dataOrder = baseQuery.Build();

            string expectedSQL = "SELECT\r\n    TableAlias.ID AS TableAliasID,\r\n    TableAlias.Name AS TableAliasName\r\nFROM Table TableAlias";

            Assert.AreEqual(expectedSQL, dataOrder.Query);
            Assert.IsFalse(dataOrder.Parameters.Any());
            Assert.AreEqual(2, dataOrder.Aliases.Count());
            Assert.AreEqual("TableAlias", dataOrder.Aliases.ElementAt(0).Name);
            Assert.AreEqual("ID", dataOrder.Aliases.ElementAt(0).Data.Item1);
            Assert.AreEqual("TableAliasID", dataOrder.Aliases.ElementAt(0).Data.Item2);
            Assert.AreEqual("TableAlias", dataOrder.Aliases.ElementAt(1).Name);
            Assert.AreEqual("Name", dataOrder.Aliases.ElementAt(1).Data.Item1);
            Assert.AreEqual("TableAliasName", dataOrder.Aliases.ElementAt(1).Data.Item2);
        }

        [Test]
        public void ProduceCorrectSQLFromSingleSelectQueryOrderWithUnparameterizedClause() {

            var columns = new string[] { "ID", "Name" };
            var baseQuery = SelectQuery.New(new SelectQueryOrder("Table", "", columns));
            var dataOrder = baseQuery.Build(Clause.New().AddClause("ID = 2").AddClause("Name = 'Test'"));

            string expectedSQL = "SELECT\r\n    Table.ID AS TableID,\r\n    Table.Name AS TableName\r\nFROM Table\r\nWHERE ID = 2\r\nAND Name = 'Test'";

            Assert.AreEqual(expectedSQL, dataOrder.Query);
            Assert.IsFalse(dataOrder.Parameters.Any());
            Assert.AreEqual(2, dataOrder.Aliases.Count());
            Assert.AreEqual("Table", dataOrder.Aliases.ElementAt(0).Name);
            Assert.AreEqual("ID", dataOrder.Aliases.ElementAt(0).Data.Item1);
            Assert.AreEqual("TableID", dataOrder.Aliases.ElementAt(0).Data.Item2);
            Assert.AreEqual("Table", dataOrder.Aliases.ElementAt(1).Name);
            Assert.AreEqual("Name", dataOrder.Aliases.ElementAt(1).Data.Item1);
            Assert.AreEqual("TableName", dataOrder.Aliases.ElementAt(1).Data.Item2);
        }

        [Test]
        public void ProduceCorrectSQLFromSingleSelectQueryOrderWithTableAliasAndUnparameterizedClause() {

            var columns = new string[] { "ID", "Name" };
            var baseQuery = SelectQuery.New(new SelectQueryOrder("Table", "TableAlias", columns));
            var dataOrder = baseQuery.Build(Clause.New().AddClause("TableAlias.ID = 2").AddClause("TableAlias.Name = 'Test'"));

            string expectedSQL = "SELECT\r\n    TableAlias.ID AS TableAliasID,\r\n    TableAlias.Name AS TableAliasName\r\nFROM Table TableAlias\r\nWHERE TableAlias.ID = 2\r\nAND TableAlias.Name = 'Test'";

            Assert.AreEqual(expectedSQL, dataOrder.Query);
            Assert.IsFalse(dataOrder.Parameters.Any());
            Assert.AreEqual(2, dataOrder.Aliases.Count());
            Assert.AreEqual("TableAlias", dataOrder.Aliases.ElementAt(0).Name);
            Assert.AreEqual("ID", dataOrder.Aliases.ElementAt(0).Data.Item1);
            Assert.AreEqual("TableAliasID", dataOrder.Aliases.ElementAt(0).Data.Item2);
            Assert.AreEqual("TableAlias", dataOrder.Aliases.ElementAt(1).Name);
            Assert.AreEqual("Name", dataOrder.Aliases.ElementAt(1).Data.Item1);
            Assert.AreEqual("TableAliasName", dataOrder.Aliases.ElementAt(1).Data.Item2);
        }

        [Test]
        public void ProduceCorrectSQLFromSingleSelectQueryOrderWithParameterizedClause() {

            var clause =
                Clause.New()
                    .AddClause("@id", 4, x => $"ID = {x}")
                    .AddClause("@name", "Test", x => $"Name = {x}")
                    ;

            var columns = new string[] { "ID", "Name" };
            var baseQuery = SelectQuery.New(new SelectQueryOrder("Table", "", columns));
            var dataOrder = baseQuery.Build(clause);

            string expectedSQL = "SELECT\r\n    Table.ID AS TableID,\r\n    Table.Name AS TableName\r\nFROM Table\r\nWHERE ID = @id\r\nAND Name = @name";

            Assert.AreEqual(expectedSQL, dataOrder.Query);

            Assert.AreEqual(2, dataOrder.Parameters.Count());
            Assert.AreEqual("@id" , dataOrder.Parameters.ElementAt(0).Key);
            Assert.AreEqual(4, dataOrder.Parameters.ElementAt(0).Value);
            Assert.AreEqual("@name" , dataOrder.Parameters.ElementAt(1).Key);
            Assert.AreEqual("Test" , dataOrder.Parameters.ElementAt(1).Value);

            Assert.AreEqual(2, dataOrder.Aliases.Count());
            Assert.AreEqual("Table", dataOrder.Aliases.ElementAt(0).Name);
            Assert.AreEqual("ID", dataOrder.Aliases.ElementAt(0).Data.Item1);
            Assert.AreEqual("TableID", dataOrder.Aliases.ElementAt(0).Data.Item2);
            Assert.AreEqual("Table", dataOrder.Aliases.ElementAt(1).Name);
            Assert.AreEqual("Name", dataOrder.Aliases.ElementAt(1).Data.Item1);
            Assert.AreEqual("TableName", dataOrder.Aliases.ElementAt(1).Data.Item2);
        }

        [Test]
        public void ProduceCorrectSQLFromSingleSelectQueryOrderWithTableAliasAndParameterizedClause() {

            var clause =
                Clause.New()
                    .AddClause("@id", 4, x => $"ID = {x}")
                    .AddClause("@name", "Test", x => $"Name = {x}")
                    ;

            var columns = new string[] { "ID", "Name" };
            var baseQuery = SelectQuery.New(new SelectQueryOrder("Table", "TableAlias", columns));
            var dataOrder = baseQuery.Build(clause);

            string expectedSQL = "SELECT\r\n    TableAlias.ID AS TableAliasID,\r\n    TableAlias.Name AS TableAliasName\r\nFROM Table TableAlias\r\nWHERE ID = @id\r\nAND Name = @name";

            Assert.AreEqual(expectedSQL, dataOrder.Query);

            Assert.AreEqual(2, dataOrder.Parameters.Count());
            Assert.AreEqual("@id" , dataOrder.Parameters.ElementAt(0).Key);
            Assert.AreEqual(4, dataOrder.Parameters.ElementAt(0).Value);
            Assert.AreEqual("@name" , dataOrder.Parameters.ElementAt(1).Key);
            Assert.AreEqual("Test" , dataOrder.Parameters.ElementAt(1).Value);

            Assert.AreEqual(2, dataOrder.Aliases.Count());
            Assert.AreEqual("TableAlias", dataOrder.Aliases.ElementAt(0).Name);
            Assert.AreEqual("ID", dataOrder.Aliases.ElementAt(0).Data.Item1);
            Assert.AreEqual("TableAliasID", dataOrder.Aliases.ElementAt(0).Data.Item2);
            Assert.AreEqual("TableAlias", dataOrder.Aliases.ElementAt(1).Name);
            Assert.AreEqual("Name", dataOrder.Aliases.ElementAt(1).Data.Item1);
            Assert.AreEqual("TableAliasName", dataOrder.Aliases.ElementAt(1).Data.Item2);
        }

        [Test]
        public void ProduceCorrectSQLFromInnerJoinedSelectQueryOrderWithNoTableAlias() {
            var orderColumns = new string[] { "Product" };
            var customerColumns = new string[] { "Name" };

            var primary = new SelectQueryOrder("Order", "", orderColumns);
            var secondary = new SelectQueryOrder("Customer", "", customerColumns, "Id", new KeyValuePair<string, string>("Order", "CustomerId"));

            var dataOrder = SelectQuery.New(primary).AddInnerJoin(secondary).Build();

            string expectedSQL = "SELECT\r\n    Order.Product AS OrderProduct,\r\n    Customer.Name AS CustomerName\r\nFROM Order\r\nINNER JOIN Customer ON Customer.Id = Order.CustomerId";

            Assert.AreEqual(expectedSQL, dataOrder.Query);
            Assert.IsFalse(dataOrder.Parameters.Any());
            Assert.AreEqual(2, dataOrder.Aliases.Count());
            Assert.AreEqual("Order", dataOrder.Aliases.ElementAt(0).Name);
            Assert.AreEqual("Product", dataOrder.Aliases.ElementAt(0).Data.Item1);
            Assert.AreEqual("OrderProduct", dataOrder.Aliases.ElementAt(0).Data.Item2);
            Assert.AreEqual("Customer", dataOrder.Aliases.ElementAt(1).Name);
            Assert.AreEqual("Name", dataOrder.Aliases.ElementAt(1).Data.Item1);
            Assert.AreEqual("CustomerName", dataOrder.Aliases.ElementAt(1).Data.Item2);
        }

        [Test]
        public void ProduceCorrectSQLFromInnerJoinedSelectQueryOrderWithTableAlias() {
            var orderColumns = new string[] { "Product" };
            var customerColumns = new string[] { "Name" };

            var primary = new SelectQueryOrder("Order", "O", orderColumns);
            var secondary = new SelectQueryOrder("Customer", "C", customerColumns, "Id", new KeyValuePair<string, string>("O", "CustomerId"));

            var dataOrder = SelectQuery.New(primary).AddInnerJoin(secondary).Build();

            string expectedSQL = "SELECT\r\n    O.Product AS OProduct,\r\n    C.Name AS CName\r\nFROM Order O\r\nINNER JOIN Customer C ON C.Id = O.CustomerId";

            Assert.AreEqual(expectedSQL, dataOrder.Query);
            Assert.IsFalse(dataOrder.Parameters.Any());
            Assert.AreEqual(2, dataOrder.Aliases.Count());
            Assert.AreEqual("O", dataOrder.Aliases.ElementAt(0).Name);
            Assert.AreEqual("Product", dataOrder.Aliases.ElementAt(0).Data.Item1);
            Assert.AreEqual("OProduct", dataOrder.Aliases.ElementAt(0).Data.Item2);
            Assert.AreEqual("C", dataOrder.Aliases.ElementAt(1).Name);
            Assert.AreEqual("Name", dataOrder.Aliases.ElementAt(1).Data.Item1);
            Assert.AreEqual("CName", dataOrder.Aliases.ElementAt(1).Data.Item2);
        }

        [Test]
        public void ProduceCorrectSQLFromInnerJoinedSelectQueryOrderWithUnparameterizedClause() {
            var orderColumns = new string[] { "Product" };
            var customerColumns = new string[] { "Name" };

            var primary = new SelectQueryOrder("Order", "", orderColumns);
            var secondary = new SelectQueryOrder("Customer", "", customerColumns, "Id", new KeyValuePair<string, string>("Order", "CustomerId"));

            var dataOrder = SelectQuery.New(primary).AddInnerJoin(secondary).Build(Clause.New().AddClause("Customer.Id = 2").AddClause("Order.CategoryId = 5"));

            string expectedSQL = "SELECT\r\n    Order.Product AS OrderProduct,\r\n    Customer.Name AS CustomerName\r\nFROM Order\r\nINNER JOIN Customer ON Customer.Id = Order.CustomerId\r\nWHERE Customer.Id = 2\r\nAND Order.CategoryId = 5";

            Assert.AreEqual(expectedSQL, dataOrder.Query);
            Assert.IsFalse(dataOrder.Parameters.Any());
            Assert.AreEqual(2, dataOrder.Aliases.Count());
            Assert.AreEqual("Order", dataOrder.Aliases.ElementAt(0).Name);
            Assert.AreEqual("Product", dataOrder.Aliases.ElementAt(0).Data.Item1);
            Assert.AreEqual("OrderProduct", dataOrder.Aliases.ElementAt(0).Data.Item2);
            Assert.AreEqual("Customer", dataOrder.Aliases.ElementAt(1).Name);
            Assert.AreEqual("Name", dataOrder.Aliases.ElementAt(1).Data.Item1);
            Assert.AreEqual("CustomerName", dataOrder.Aliases.ElementAt(1).Data.Item2);
        }

        [Test]
        public void ProduceCorrectSQLFromInnerJoinedSelectQueryOrderWithTableAliasAndUnparameterizedClause() {
            var orderColumns = new string[] { "Product" };
            var customerColumns = new string[] { "Name" };

            var primary = new SelectQueryOrder("Order", "O", orderColumns);
            var secondary = new SelectQueryOrder("Customer", "C", customerColumns, "Id", new KeyValuePair<string, string>("O", "CustomerId"));

            var dataOrder = SelectQuery.New(primary).AddInnerJoin(secondary).Build(Clause.New().AddClause("C.Id = 2").AddClause("O.CategoryId = 5"));

            string expectedSQL = "SELECT\r\n    O.Product AS OProduct,\r\n    C.Name AS CName\r\nFROM Order O\r\nINNER JOIN Customer C ON C.Id = O.CustomerId\r\nWHERE C.Id = 2\r\nAND O.CategoryId = 5";

            Assert.AreEqual(expectedSQL, dataOrder.Query);
            Assert.IsFalse(dataOrder.Parameters.Any());
            Assert.AreEqual(2, dataOrder.Aliases.Count());
            Assert.AreEqual("O", dataOrder.Aliases.ElementAt(0).Name);
            Assert.AreEqual("Product", dataOrder.Aliases.ElementAt(0).Data.Item1);
            Assert.AreEqual("OProduct", dataOrder.Aliases.ElementAt(0).Data.Item2);
            Assert.AreEqual("C", dataOrder.Aliases.ElementAt(1).Name);
            Assert.AreEqual("Name", dataOrder.Aliases.ElementAt(1).Data.Item1);
            Assert.AreEqual("CName", dataOrder.Aliases.ElementAt(1).Data.Item2);
        }

        [Test]
        public void ProduceCorrectSQLFromInnerJoinedSelectQueryOrderWithParameterizedClause() {

            var clause =
                Clause.New()
                    .AddClause("@customer", 2, x => $"Customer.Id = {x}")
                    .AddClause("@category", 5, x => $"Order.CategoryId = {x}")
                    ;

            var orderColumns = new string[] { "Product" };
            var customerColumns = new string[] { "Name" };

            var primary = new SelectQueryOrder("Order", "", orderColumns);
            var secondary = new SelectQueryOrder("Customer", "", customerColumns, "Id", new KeyValuePair<string, string>("Order", "CustomerId"));

            var dataOrder = SelectQuery.New(primary).AddInnerJoin(secondary).Build(clause);

            string expectedSQL = "SELECT\r\n    Order.Product AS OrderProduct,\r\n    Customer.Name AS CustomerName\r\nFROM Order\r\nINNER JOIN Customer ON Customer.Id = Order.CustomerId\r\nWHERE Customer.Id = @customer\r\nAND Order.CategoryId = @category";

            Assert.AreEqual(expectedSQL, dataOrder.Query);

            Assert.AreEqual(2, dataOrder.Parameters.Count());
            Assert.AreEqual("@customer" , dataOrder.Parameters.ElementAt(0).Key);
            Assert.AreEqual(2, dataOrder.Parameters.ElementAt(0).Value);
            Assert.AreEqual("@category" , dataOrder.Parameters.ElementAt(1).Key);
            Assert.AreEqual(5, dataOrder.Parameters.ElementAt(1).Value);

            Assert.AreEqual(2, dataOrder.Aliases.Count());
            Assert.AreEqual("Order", dataOrder.Aliases.ElementAt(0).Name);
            Assert.AreEqual("Product", dataOrder.Aliases.ElementAt(0).Data.Item1);
            Assert.AreEqual("OrderProduct", dataOrder.Aliases.ElementAt(0).Data.Item2);
            Assert.AreEqual("Customer", dataOrder.Aliases.ElementAt(1).Name);
            Assert.AreEqual("Name", dataOrder.Aliases.ElementAt(1).Data.Item1);
            Assert.AreEqual("CustomerName", dataOrder.Aliases.ElementAt(1).Data.Item2);
        }

        [Test]
        public void ProduceCorrectSQLFromInnerJoinedSelectQueryOrderWithTableAliasAndParameterizedClause() {
            var clause =
                Clause.New()
                    .AddClause("@customer", 2, x => $"C.Id = {x}")
                    .AddClause("@category", 5, x => $"O.CategoryId = {x}")
                    ;

            var orderColumns = new string[] { "Product" };
            var customerColumns = new string[] { "Name" };

            var primary = new SelectQueryOrder("Order", "O", orderColumns);
            var secondary = new SelectQueryOrder("Customer", "C", customerColumns, "Id", new KeyValuePair<string, string>("O", "CustomerId"));

            var dataOrder = SelectQuery.New(primary).AddInnerJoin(secondary).Build(clause);

            string expectedSQL = "SELECT\r\n    O.Product AS OProduct,\r\n    C.Name AS CName\r\nFROM Order O\r\nINNER JOIN Customer C ON C.Id = O.CustomerId\r\nWHERE C.Id = @customer\r\nAND O.CategoryId = @category";

            Assert.AreEqual(expectedSQL, dataOrder.Query);

            Assert.AreEqual(2, dataOrder.Parameters.Count());
            Assert.AreEqual("@customer" , dataOrder.Parameters.ElementAt(0).Key);
            Assert.AreEqual(2, dataOrder.Parameters.ElementAt(0).Value);
            Assert.AreEqual("@category" , dataOrder.Parameters.ElementAt(1).Key);
            Assert.AreEqual(5, dataOrder.Parameters.ElementAt(1).Value);

            Assert.AreEqual(2, dataOrder.Aliases.Count());
            Assert.AreEqual("O", dataOrder.Aliases.ElementAt(0).Name);
            Assert.AreEqual("Product", dataOrder.Aliases.ElementAt(0).Data.Item1);
            Assert.AreEqual("OProduct", dataOrder.Aliases.ElementAt(0).Data.Item2);
            Assert.AreEqual("C", dataOrder.Aliases.ElementAt(1).Name);
            Assert.AreEqual("Name", dataOrder.Aliases.ElementAt(1).Data.Item1);
            Assert.AreEqual("CName", dataOrder.Aliases.ElementAt(1).Data.Item2);
        }

        [Test]
        public void ProduceCorrectSQLFromLeftJoinedSelectQueryOrderWithNoTableAlias() {
            var orderColumns = new string[] { "Product" };
            var customerColumns = new string[] { "Name" };

            var primary = new SelectQueryOrder("Order", "", orderColumns);
            var secondary = new SelectQueryOrder("Customer", "", customerColumns, "Id", new KeyValuePair<string, string>("Order", "CustomerId"));

            var dataOrder = SelectQuery.New(primary).AddLeftJoin(secondary).Build();

            string expectedSQL = "SELECT\r\n    Order.Product AS OrderProduct,\r\n    Customer.Name AS CustomerName\r\nFROM Order\r\nLEFT JOIN Customer ON Customer.Id = Order.CustomerId";

            Assert.AreEqual(expectedSQL, dataOrder.Query);
            Assert.IsFalse(dataOrder.Parameters.Any());
            Assert.AreEqual(2, dataOrder.Aliases.Count());
            Assert.AreEqual("Order", dataOrder.Aliases.ElementAt(0).Name);
            Assert.AreEqual("Product", dataOrder.Aliases.ElementAt(0).Data.Item1);
            Assert.AreEqual("OrderProduct", dataOrder.Aliases.ElementAt(0).Data.Item2);
            Assert.AreEqual("Customer", dataOrder.Aliases.ElementAt(1).Name);
            Assert.AreEqual("Name", dataOrder.Aliases.ElementAt(1).Data.Item1);
            Assert.AreEqual("CustomerName", dataOrder.Aliases.ElementAt(1).Data.Item2);
        }

        [Test]
        public void ProduceCorrectSQLFromLeftJoinedSelectQueryOrderWithTableAlias() {
            var orderColumns = new string[] { "Product" };
            var customerColumns = new string[] { "Name" };

            var primary = new SelectQueryOrder("Order", "O", orderColumns);
            var secondary = new SelectQueryOrder("Customer", "C", customerColumns, "Id", new KeyValuePair<string, string>("O", "CustomerId"));

            var dataOrder = SelectQuery.New(primary).AddLeftJoin(secondary).Build();

            string expectedSQL = "SELECT\r\n    O.Product AS OProduct,\r\n    C.Name AS CName\r\nFROM Order O\r\nLEFT JOIN Customer C ON C.Id = O.CustomerId";

            Assert.AreEqual(expectedSQL, dataOrder.Query);
            Assert.IsFalse(dataOrder.Parameters.Any());
            Assert.AreEqual(2, dataOrder.Aliases.Count());
            Assert.AreEqual("O", dataOrder.Aliases.ElementAt(0).Name);
            Assert.AreEqual("Product", dataOrder.Aliases.ElementAt(0).Data.Item1);
            Assert.AreEqual("OProduct", dataOrder.Aliases.ElementAt(0).Data.Item2);
            Assert.AreEqual("C", dataOrder.Aliases.ElementAt(1).Name);
            Assert.AreEqual("Name", dataOrder.Aliases.ElementAt(1).Data.Item1);
            Assert.AreEqual("CName", dataOrder.Aliases.ElementAt(1).Data.Item2);
        }

        [Test]
        public void ProduceCorrectSQLFromLeftJoinedSelectQueryOrderWithUnparameterizedClause() {
            var orderColumns = new string[] { "Product" };
            var customerColumns = new string[] { "Name" };

            var primary = new SelectQueryOrder("Order", "", orderColumns);
            var secondary = new SelectQueryOrder("Customer", "", customerColumns, "Id", new KeyValuePair<string, string>("Order", "CustomerId"));

            var dataOrder = SelectQuery.New(primary).AddLeftJoin(secondary).Build(Clause.New().AddClause("Customer.Id = 2").AddClause("Order.CategoryId = 5"));

            string expectedSQL = "SELECT\r\n    Order.Product AS OrderProduct,\r\n    Customer.Name AS CustomerName\r\nFROM Order\r\nLEFT JOIN Customer ON Customer.Id = Order.CustomerId\r\nWHERE Customer.Id = 2\r\nAND Order.CategoryId = 5";

            Assert.AreEqual(expectedSQL, dataOrder.Query);
            Assert.IsFalse(dataOrder.Parameters.Any());
            Assert.AreEqual(2, dataOrder.Aliases.Count());
            Assert.AreEqual("Order", dataOrder.Aliases.ElementAt(0).Name);
            Assert.AreEqual("Product", dataOrder.Aliases.ElementAt(0).Data.Item1);
            Assert.AreEqual("OrderProduct", dataOrder.Aliases.ElementAt(0).Data.Item2);
            Assert.AreEqual("Customer", dataOrder.Aliases.ElementAt(1).Name);
            Assert.AreEqual("Name", dataOrder.Aliases.ElementAt(1).Data.Item1);
            Assert.AreEqual("CustomerName", dataOrder.Aliases.ElementAt(1).Data.Item2);
        }

        [Test]
        public void ProduceCorrectSQLFromLeftJoinedSelectQueryOrderWithTableAliasAndUnparameterizedClause() {
            var orderColumns = new string[] { "Product" };
            var customerColumns = new string[] { "Name" };

            var primary = new SelectQueryOrder("Order", "O", orderColumns);
            var secondary = new SelectQueryOrder("Customer", "C", customerColumns, "Id", new KeyValuePair<string, string>("O", "CustomerId"));

            var dataOrder = SelectQuery.New(primary).AddLeftJoin(secondary).Build(Clause.New().AddClause("C.Id = 2").AddClause("O.CategoryId = 5"));

            string expectedSQL = "SELECT\r\n    O.Product AS OProduct,\r\n    C.Name AS CName\r\nFROM Order O\r\nLEFT JOIN Customer C ON C.Id = O.CustomerId\r\nWHERE C.Id = 2\r\nAND O.CategoryId = 5";

            Assert.AreEqual(expectedSQL, dataOrder.Query);
            Assert.IsFalse(dataOrder.Parameters.Any());
            Assert.AreEqual(2, dataOrder.Aliases.Count());
            Assert.AreEqual("O", dataOrder.Aliases.ElementAt(0).Name);
            Assert.AreEqual("Product", dataOrder.Aliases.ElementAt(0).Data.Item1);
            Assert.AreEqual("OProduct", dataOrder.Aliases.ElementAt(0).Data.Item2);
            Assert.AreEqual("C", dataOrder.Aliases.ElementAt(1).Name);
            Assert.AreEqual("Name", dataOrder.Aliases.ElementAt(1).Data.Item1);
            Assert.AreEqual("CName", dataOrder.Aliases.ElementAt(1).Data.Item2);
        }

        [Test]
        public void ProduceCorrectSQLFromLeftJoinedSelectQueryOrderWithParameterizedClause() {

            var clause =
                Clause.New()
                    .AddClause("@customer", 2, x => $"Customer.Id = {x}")
                    .AddClause("@category", 5, x => $"Order.CategoryId = {x}")
                    ;

            var orderColumns = new string[] { "Product" };
            var customerColumns = new string[] { "Name" };

            var primary = new SelectQueryOrder("Order", "", orderColumns);
            var secondary = new SelectQueryOrder("Customer", "", customerColumns, "Id", new KeyValuePair<string, string>("Order", "CustomerId"));

            var dataOrder = SelectQuery.New(primary).AddLeftJoin(secondary).Build(clause);

            string expectedSQL = "SELECT\r\n    Order.Product AS OrderProduct,\r\n    Customer.Name AS CustomerName\r\nFROM Order\r\nLEFT JOIN Customer ON Customer.Id = Order.CustomerId\r\nWHERE Customer.Id = @customer\r\nAND Order.CategoryId = @category";

            Assert.AreEqual(expectedSQL, dataOrder.Query);

            Assert.AreEqual(2, dataOrder.Parameters.Count());
            Assert.AreEqual("@customer" , dataOrder.Parameters.ElementAt(0).Key);
            Assert.AreEqual(2, dataOrder.Parameters.ElementAt(0).Value);
            Assert.AreEqual("@category" , dataOrder.Parameters.ElementAt(1).Key);
            Assert.AreEqual(5, dataOrder.Parameters.ElementAt(1).Value);

            Assert.AreEqual(2, dataOrder.Aliases.Count());
            Assert.AreEqual("Order", dataOrder.Aliases.ElementAt(0).Name);
            Assert.AreEqual("Product", dataOrder.Aliases.ElementAt(0).Data.Item1);
            Assert.AreEqual("OrderProduct", dataOrder.Aliases.ElementAt(0).Data.Item2);
            Assert.AreEqual("Customer", dataOrder.Aliases.ElementAt(1).Name);
            Assert.AreEqual("Name", dataOrder.Aliases.ElementAt(1).Data.Item1);
            Assert.AreEqual("CustomerName", dataOrder.Aliases.ElementAt(1).Data.Item2);
        }

        [Test]
        public void ProduceCorrectSQLFromLeftJoinedSelectQueryOrderWithTableAliasAndParameterizedClause() {
            var clause =
                Clause.New()
                    .AddClause("@customer", 2, x => $"C.Id = {x}")
                    .AddClause("@category", 5, x => $"O.CategoryId = {x}")
                    ;

            var orderColumns = new string[] { "Product" };
            var customerColumns = new string[] { "Name" };

            var primary = new SelectQueryOrder("Order", "O", orderColumns);
            var secondary = new SelectQueryOrder("Customer", "C", customerColumns, "Id", new KeyValuePair<string, string>("O", "CustomerId"));

            var dataOrder = SelectQuery.New(primary).AddLeftJoin(secondary).Build(clause);

            string expectedSQL = "SELECT\r\n    O.Product AS OProduct,\r\n    C.Name AS CName\r\nFROM Order O\r\nLEFT JOIN Customer C ON C.Id = O.CustomerId\r\nWHERE C.Id = @customer\r\nAND O.CategoryId = @category";

            Assert.AreEqual(expectedSQL, dataOrder.Query);

            Assert.AreEqual(2, dataOrder.Parameters.Count());
            Assert.AreEqual("@customer" , dataOrder.Parameters.ElementAt(0).Key);
            Assert.AreEqual(2, dataOrder.Parameters.ElementAt(0).Value);
            Assert.AreEqual("@category" , dataOrder.Parameters.ElementAt(1).Key);
            Assert.AreEqual(5, dataOrder.Parameters.ElementAt(1).Value);

            Assert.AreEqual(2, dataOrder.Aliases.Count());
            Assert.AreEqual("O", dataOrder.Aliases.ElementAt(0).Name);
            Assert.AreEqual("Product", dataOrder.Aliases.ElementAt(0).Data.Item1);
            Assert.AreEqual("OProduct", dataOrder.Aliases.ElementAt(0).Data.Item2);
            Assert.AreEqual("C", dataOrder.Aliases.ElementAt(1).Name);
            Assert.AreEqual("Name", dataOrder.Aliases.ElementAt(1).Data.Item1);
            Assert.AreEqual("CName", dataOrder.Aliases.ElementAt(1).Data.Item2);
        }

        [Test]
        public void ProduceCorrectSQLFromRightJoinedSelectQueryOrderWithNoTableAlias() {
            var orderColumns = new string[] { "Product" };
            var customerColumns = new string[] { "Name" };

            var primary = new SelectQueryOrder("Order", "", orderColumns);
            var secondary = new SelectQueryOrder("Customer", "", customerColumns, "Id", new KeyValuePair<string, string>("Order", "CustomerId"));

            var dataOrder = SelectQuery.New(primary).AddRightJoin(secondary).Build();

            string expectedSQL = "SELECT\r\n    Order.Product AS OrderProduct,\r\n    Customer.Name AS CustomerName\r\nFROM Order\r\nRIGHT JOIN Customer ON Customer.Id = Order.CustomerId";

            Assert.AreEqual(expectedSQL, dataOrder.Query);
            Assert.IsFalse(dataOrder.Parameters.Any());
            Assert.AreEqual(2, dataOrder.Aliases.Count());
            Assert.AreEqual("Order", dataOrder.Aliases.ElementAt(0).Name);
            Assert.AreEqual("Product", dataOrder.Aliases.ElementAt(0).Data.Item1);
            Assert.AreEqual("OrderProduct", dataOrder.Aliases.ElementAt(0).Data.Item2);
            Assert.AreEqual("Customer", dataOrder.Aliases.ElementAt(1).Name);
            Assert.AreEqual("Name", dataOrder.Aliases.ElementAt(1).Data.Item1);
            Assert.AreEqual("CustomerName", dataOrder.Aliases.ElementAt(1).Data.Item2);
        }

        [Test]
        public void ProduceCorrectSQLFromRightJoinedSelectQueryOrderWithTableAlias() {
            var orderColumns = new string[] { "Product" };
            var customerColumns = new string[] { "Name" };

            var primary = new SelectQueryOrder("Order", "O", orderColumns);
            var secondary = new SelectQueryOrder("Customer", "C", customerColumns, "Id", new KeyValuePair<string, string>("O", "CustomerId"));

            var dataOrder = SelectQuery.New(primary).AddRightJoin(secondary).Build();

            string expectedSQL = "SELECT\r\n    O.Product AS OProduct,\r\n    C.Name AS CName\r\nFROM Order O\r\nRIGHT JOIN Customer C ON C.Id = O.CustomerId";

            Assert.AreEqual(expectedSQL, dataOrder.Query);
            Assert.IsFalse(dataOrder.Parameters.Any());
            Assert.AreEqual(2, dataOrder.Aliases.Count());
            Assert.AreEqual("O", dataOrder.Aliases.ElementAt(0).Name);
            Assert.AreEqual("Product", dataOrder.Aliases.ElementAt(0).Data.Item1);
            Assert.AreEqual("OProduct", dataOrder.Aliases.ElementAt(0).Data.Item2);
            Assert.AreEqual("C", dataOrder.Aliases.ElementAt(1).Name);
            Assert.AreEqual("Name", dataOrder.Aliases.ElementAt(1).Data.Item1);
            Assert.AreEqual("CName", dataOrder.Aliases.ElementAt(1).Data.Item2);
        }

        [Test]
        public void ProduceCorrectSQLFromRightJoinedSelectQueryOrderWithUnparameterizedClause() {
            var orderColumns = new string[] { "Product" };
            var customerColumns = new string[] { "Name" };

            var primary = new SelectQueryOrder("Order", "", orderColumns);
            var secondary = new SelectQueryOrder("Customer", "", customerColumns, "Id", new KeyValuePair<string, string>("Order", "CustomerId"));

            var dataOrder = SelectQuery.New(primary).AddRightJoin(secondary).Build(Clause.New().AddClause("Customer.Id = 2").AddClause("Order.CategoryId = 5"));

            string expectedSQL = "SELECT\r\n    Order.Product AS OrderProduct,\r\n    Customer.Name AS CustomerName\r\nFROM Order\r\nRIGHT JOIN Customer ON Customer.Id = Order.CustomerId\r\nWHERE Customer.Id = 2\r\nAND Order.CategoryId = 5";

            Assert.AreEqual(expectedSQL, dataOrder.Query);
            Assert.IsFalse(dataOrder.Parameters.Any());
            Assert.AreEqual(2, dataOrder.Aliases.Count());
            Assert.AreEqual("Order", dataOrder.Aliases.ElementAt(0).Name);
            Assert.AreEqual("Product", dataOrder.Aliases.ElementAt(0).Data.Item1);
            Assert.AreEqual("OrderProduct", dataOrder.Aliases.ElementAt(0).Data.Item2);
            Assert.AreEqual("Customer", dataOrder.Aliases.ElementAt(1).Name);
            Assert.AreEqual("Name", dataOrder.Aliases.ElementAt(1).Data.Item1);
            Assert.AreEqual("CustomerName", dataOrder.Aliases.ElementAt(1).Data.Item2);
        }

        [Test]
        public void ProduceCorrectSQLFromRightJoinedSelectQueryOrderWithTableAliasAndUnparameterizedClause() {
            var orderColumns = new string[] { "Product" };
            var customerColumns = new string[] { "Name" };

            var primary = new SelectQueryOrder("Order", "O", orderColumns);
            var secondary = new SelectQueryOrder("Customer", "C", customerColumns, "Id", new KeyValuePair<string, string>("O", "CustomerId"));

            var dataOrder = SelectQuery.New(primary).AddRightJoin(secondary).Build(Clause.New().AddClause("C.Id = 2").AddClause("O.CategoryId = 5"));

            string expectedSQL = "SELECT\r\n    O.Product AS OProduct,\r\n    C.Name AS CName\r\nFROM Order O\r\nRIGHT JOIN Customer C ON C.Id = O.CustomerId\r\nWHERE C.Id = 2\r\nAND O.CategoryId = 5";

            Assert.AreEqual(expectedSQL, dataOrder.Query);
            Assert.IsFalse(dataOrder.Parameters.Any());
            Assert.AreEqual(2, dataOrder.Aliases.Count());
            Assert.AreEqual("O", dataOrder.Aliases.ElementAt(0).Name);
            Assert.AreEqual("Product", dataOrder.Aliases.ElementAt(0).Data.Item1);
            Assert.AreEqual("OProduct", dataOrder.Aliases.ElementAt(0).Data.Item2);
            Assert.AreEqual("C", dataOrder.Aliases.ElementAt(1).Name);
            Assert.AreEqual("Name", dataOrder.Aliases.ElementAt(1).Data.Item1);
            Assert.AreEqual("CName", dataOrder.Aliases.ElementAt(1).Data.Item2);
        }

        [Test]
        public void ProduceCorrectSQLFromRightJoinedSelectQueryOrderWithParameterizedClause() {

            var clause =
                Clause.New()
                    .AddClause("@customer", 2, x => $"Customer.Id = {x}")
                    .AddClause("@category", 5, x => $"Order.CategoryId = {x}")
                    ;

            var orderColumns = new string[] { "Product" };
            var customerColumns = new string[] { "Name" };

            var primary = new SelectQueryOrder("Order", "", orderColumns);
            var secondary = new SelectQueryOrder("Customer", "", customerColumns, "Id", new KeyValuePair<string, string>("Order", "CustomerId"));

            var dataOrder = SelectQuery.New(primary).AddRightJoin(secondary).Build(clause);

            string expectedSQL = "SELECT\r\n    Order.Product AS OrderProduct,\r\n    Customer.Name AS CustomerName\r\nFROM Order\r\nRIGHT JOIN Customer ON Customer.Id = Order.CustomerId\r\nWHERE Customer.Id = @customer\r\nAND Order.CategoryId = @category";

            Assert.AreEqual(expectedSQL, dataOrder.Query);

            Assert.AreEqual(2, dataOrder.Parameters.Count());
            Assert.AreEqual("@customer" , dataOrder.Parameters.ElementAt(0).Key);
            Assert.AreEqual(2, dataOrder.Parameters.ElementAt(0).Value);
            Assert.AreEqual("@category" , dataOrder.Parameters.ElementAt(1).Key);
            Assert.AreEqual(5, dataOrder.Parameters.ElementAt(1).Value);

            Assert.AreEqual(2, dataOrder.Aliases.Count());
            Assert.AreEqual("Order", dataOrder.Aliases.ElementAt(0).Name);
            Assert.AreEqual("Product", dataOrder.Aliases.ElementAt(0).Data.Item1);
            Assert.AreEqual("OrderProduct", dataOrder.Aliases.ElementAt(0).Data.Item2);
            Assert.AreEqual("Customer", dataOrder.Aliases.ElementAt(1).Name);
            Assert.AreEqual("Name", dataOrder.Aliases.ElementAt(1).Data.Item1);
            Assert.AreEqual("CustomerName", dataOrder.Aliases.ElementAt(1).Data.Item2);
        }

        [Test]
        public void ProduceCorrectSQLFromRightJoinedSelectQueryOrderWithTableAliasAndParameterizedClause() {
            var clause =
                Clause.New()
                    .AddClause("@customer", 2, x => $"C.Id = {x}")
                    .AddClause("@category", 5, x => $"O.CategoryId = {x}")
                    ;

            var orderColumns = new string[] { "Product" };
            var customerColumns = new string[] { "Name" };

            var primary = new SelectQueryOrder("Order", "O", orderColumns);
            var secondary = new SelectQueryOrder("Customer", "C", customerColumns, "Id", new KeyValuePair<string, string>("O", "CustomerId"));

            var dataOrder = SelectQuery.New(primary).AddRightJoin(secondary).Build(clause);

            string expectedSQL = "SELECT\r\n    O.Product AS OProduct,\r\n    C.Name AS CName\r\nFROM Order O\r\nRIGHT JOIN Customer C ON C.Id = O.CustomerId\r\nWHERE C.Id = @customer\r\nAND O.CategoryId = @category";

            Assert.AreEqual(expectedSQL, dataOrder.Query);

            Assert.AreEqual(2, dataOrder.Parameters.Count());
            Assert.AreEqual("@customer" , dataOrder.Parameters.ElementAt(0).Key);
            Assert.AreEqual(2, dataOrder.Parameters.ElementAt(0).Value);
            Assert.AreEqual("@category" , dataOrder.Parameters.ElementAt(1).Key);
            Assert.AreEqual(5, dataOrder.Parameters.ElementAt(1).Value);

            Assert.AreEqual(2, dataOrder.Aliases.Count());
            Assert.AreEqual("O", dataOrder.Aliases.ElementAt(0).Name);
            Assert.AreEqual("Product", dataOrder.Aliases.ElementAt(0).Data.Item1);
            Assert.AreEqual("OProduct", dataOrder.Aliases.ElementAt(0).Data.Item2);
            Assert.AreEqual("C", dataOrder.Aliases.ElementAt(1).Name);
            Assert.AreEqual("Name", dataOrder.Aliases.ElementAt(1).Data.Item1);
            Assert.AreEqual("CName", dataOrder.Aliases.ElementAt(1).Data.Item2);
        }

        [Test]
        public void ProduceCorrectSQLFromOuterJoinedSelectQueryOrderWithNoTableAlias() {
            var orderColumns = new string[] { "Product" };
            var customerColumns = new string[] { "Name" };

            var primary = new SelectQueryOrder("Order", "", orderColumns);
            var secondary = new SelectQueryOrder("Customer", "", customerColumns, "Id", new KeyValuePair<string, string>("Order", "CustomerId"));

            var dataOrder = SelectQuery.New(primary).AddFullOuterJoin(secondary).Build();

            string expectedSQL = "SELECT\r\n    Order.Product AS OrderProduct,\r\n    Customer.Name AS CustomerName\r\nFROM Order\r\nFULL OUTER JOIN Customer ON Customer.Id = Order.CustomerId";

            Assert.AreEqual(expectedSQL, dataOrder.Query);
            Assert.IsFalse(dataOrder.Parameters.Any());
            Assert.AreEqual(2, dataOrder.Aliases.Count());
            Assert.AreEqual("Order", dataOrder.Aliases.ElementAt(0).Name);
            Assert.AreEqual("Product", dataOrder.Aliases.ElementAt(0).Data.Item1);
            Assert.AreEqual("OrderProduct", dataOrder.Aliases.ElementAt(0).Data.Item2);
            Assert.AreEqual("Customer", dataOrder.Aliases.ElementAt(1).Name);
            Assert.AreEqual("Name", dataOrder.Aliases.ElementAt(1).Data.Item1);
            Assert.AreEqual("CustomerName", dataOrder.Aliases.ElementAt(1).Data.Item2);
        }

        [Test]
        public void ProduceCorrectSQLFromOuterJoinedSelectQueryOrderWithTableAlias() {
            var orderColumns = new string[] { "Product" };
            var customerColumns = new string[] { "Name" };

            var primary = new SelectQueryOrder("Order", "O", orderColumns);
            var secondary = new SelectQueryOrder("Customer", "C", customerColumns, "Id", new KeyValuePair<string, string>("O", "CustomerId"));

            var dataOrder = SelectQuery.New(primary).AddFullOuterJoin(secondary).Build();

            string expectedSQL = "SELECT\r\n    O.Product AS OProduct,\r\n    C.Name AS CName\r\nFROM Order O\r\nFULL OUTER JOIN Customer C ON C.Id = O.CustomerId";

            Assert.AreEqual(expectedSQL, dataOrder.Query);
            Assert.IsFalse(dataOrder.Parameters.Any());
            Assert.AreEqual(2, dataOrder.Aliases.Count());
            Assert.AreEqual("O", dataOrder.Aliases.ElementAt(0).Name);
            Assert.AreEqual("Product", dataOrder.Aliases.ElementAt(0).Data.Item1);
            Assert.AreEqual("OProduct", dataOrder.Aliases.ElementAt(0).Data.Item2);
            Assert.AreEqual("C", dataOrder.Aliases.ElementAt(1).Name);
            Assert.AreEqual("Name", dataOrder.Aliases.ElementAt(1).Data.Item1);
            Assert.AreEqual("CName", dataOrder.Aliases.ElementAt(1).Data.Item2);
        }

        [Test]
        public void ProduceCorrectSQLFromOuterJoinedSelectQueryOrderWithUnparameterizedClause() {
            var orderColumns = new string[] { "Product" };
            var customerColumns = new string[] { "Name" };

            var primary = new SelectQueryOrder("Order", "", orderColumns);
            var secondary = new SelectQueryOrder("Customer", "", customerColumns, "Id", new KeyValuePair<string, string>("Order", "CustomerId"));

            var dataOrder = SelectQuery.New(primary).AddFullOuterJoin(secondary).Build(Clause.New().AddClause("Customer.Id = 2").AddClause("Order.CategoryId = 5"));

            string expectedSQL = "SELECT\r\n    Order.Product AS OrderProduct,\r\n    Customer.Name AS CustomerName\r\nFROM Order\r\nFULL OUTER JOIN Customer ON Customer.Id = Order.CustomerId\r\nWHERE Customer.Id = 2\r\nAND Order.CategoryId = 5";

            Assert.AreEqual(expectedSQL, dataOrder.Query);
            Assert.IsFalse(dataOrder.Parameters.Any());
            Assert.AreEqual(2, dataOrder.Aliases.Count());
            Assert.AreEqual("Order", dataOrder.Aliases.ElementAt(0).Name);
            Assert.AreEqual("Product", dataOrder.Aliases.ElementAt(0).Data.Item1);
            Assert.AreEqual("OrderProduct", dataOrder.Aliases.ElementAt(0).Data.Item2);
            Assert.AreEqual("Customer", dataOrder.Aliases.ElementAt(1).Name);
            Assert.AreEqual("Name", dataOrder.Aliases.ElementAt(1).Data.Item1);
            Assert.AreEqual("CustomerName", dataOrder.Aliases.ElementAt(1).Data.Item2);
        }

        [Test]
        public void ProduceCorrectSQLFromOuterJoinedSelectQueryOrderWithTableAliasAndUnparameterizedClause() {
            var orderColumns = new string[] { "Product" };
            var customerColumns = new string[] { "Name" };

            var primary = new SelectQueryOrder("Order", "O", orderColumns);
            var secondary = new SelectQueryOrder("Customer", "C", customerColumns, "Id", new KeyValuePair<string, string>("O", "CustomerId"));

            var dataOrder = SelectQuery.New(primary).AddFullOuterJoin(secondary).Build(Clause.New().AddClause("C.Id = 2").AddClause("O.CategoryId = 5"));

            string expectedSQL = "SELECT\r\n    O.Product AS OProduct,\r\n    C.Name AS CName\r\nFROM Order O\r\nFULL OUTER JOIN Customer C ON C.Id = O.CustomerId\r\nWHERE C.Id = 2\r\nAND O.CategoryId = 5";

            Assert.AreEqual(expectedSQL, dataOrder.Query);
            Assert.IsFalse(dataOrder.Parameters.Any());
            Assert.AreEqual(2, dataOrder.Aliases.Count());
            Assert.AreEqual("O", dataOrder.Aliases.ElementAt(0).Name);
            Assert.AreEqual("Product", dataOrder.Aliases.ElementAt(0).Data.Item1);
            Assert.AreEqual("OProduct", dataOrder.Aliases.ElementAt(0).Data.Item2);
            Assert.AreEqual("C", dataOrder.Aliases.ElementAt(1).Name);
            Assert.AreEqual("Name", dataOrder.Aliases.ElementAt(1).Data.Item1);
            Assert.AreEqual("CName", dataOrder.Aliases.ElementAt(1).Data.Item2);
        }

        [Test]
        public void ProduceCorrectSQLFromOuterJoinedSelectQueryOrderWithParameterizedClause() {

            var clause =
                Clause.New()
                    .AddClause("@customer", 2, x => $"Customer.Id = {x}")
                    .AddClause("@category", 5, x => $"Order.CategoryId = {x}")
                    ;

            var orderColumns = new string[] { "Product" };
            var customerColumns = new string[] { "Name" };

            var primary = new SelectQueryOrder("Order", "", orderColumns);
            var secondary = new SelectQueryOrder("Customer", "", customerColumns, "Id", new KeyValuePair<string, string>("Order", "CustomerId"));

            var dataOrder = SelectQuery.New(primary).AddFullOuterJoin(secondary).Build(clause);

            string expectedSQL = "SELECT\r\n    Order.Product AS OrderProduct,\r\n    Customer.Name AS CustomerName\r\nFROM Order\r\nFULL OUTER JOIN Customer ON Customer.Id = Order.CustomerId\r\nWHERE Customer.Id = @customer\r\nAND Order.CategoryId = @category";

            Assert.AreEqual(expectedSQL, dataOrder.Query);

            Assert.AreEqual(2, dataOrder.Parameters.Count());
            Assert.AreEqual("@customer" , dataOrder.Parameters.ElementAt(0).Key);
            Assert.AreEqual(2, dataOrder.Parameters.ElementAt(0).Value);
            Assert.AreEqual("@category" , dataOrder.Parameters.ElementAt(1).Key);
            Assert.AreEqual(5, dataOrder.Parameters.ElementAt(1).Value);

            Assert.AreEqual(2, dataOrder.Aliases.Count());
            Assert.AreEqual("Order", dataOrder.Aliases.ElementAt(0).Name);
            Assert.AreEqual("Product", dataOrder.Aliases.ElementAt(0).Data.Item1);
            Assert.AreEqual("OrderProduct", dataOrder.Aliases.ElementAt(0).Data.Item2);
            Assert.AreEqual("Customer", dataOrder.Aliases.ElementAt(1).Name);
            Assert.AreEqual("Name", dataOrder.Aliases.ElementAt(1).Data.Item1);
            Assert.AreEqual("CustomerName", dataOrder.Aliases.ElementAt(1).Data.Item2);
        }

        [Test]
        public void ProduceCorrectSQLFromOuterJoinedSelectQueryOrderWithTableAliasAndParameterizedClause() {
            var clause =
                Clause.New()
                    .AddClause("@customer", 2, x => $"C.Id = {x}")
                    .AddClause("@category", 5, x => $"O.CategoryId = {x}")
                    ;

            var orderColumns = new string[] { "Product" };
            var customerColumns = new string[] { "Name" };

            var primary = new SelectQueryOrder("Order", "O", orderColumns);
            var secondary = new SelectQueryOrder("Customer", "C", customerColumns, "Id", new KeyValuePair<string, string>("O", "CustomerId"));

            var dataOrder = SelectQuery.New(primary).AddFullOuterJoin(secondary).Build(clause);

            string expectedSQL = "SELECT\r\n    O.Product AS OProduct,\r\n    C.Name AS CName\r\nFROM Order O\r\nFULL OUTER JOIN Customer C ON C.Id = O.CustomerId\r\nWHERE C.Id = @customer\r\nAND O.CategoryId = @category";

            Assert.AreEqual(expectedSQL, dataOrder.Query);

            Assert.AreEqual(2, dataOrder.Parameters.Count());
            Assert.AreEqual("@customer" , dataOrder.Parameters.ElementAt(0).Key);
            Assert.AreEqual(2, dataOrder.Parameters.ElementAt(0).Value);
            Assert.AreEqual("@category" , dataOrder.Parameters.ElementAt(1).Key);
            Assert.AreEqual(5, dataOrder.Parameters.ElementAt(1).Value);

            Assert.AreEqual(2, dataOrder.Aliases.Count());
            Assert.AreEqual("O", dataOrder.Aliases.ElementAt(0).Name);
            Assert.AreEqual("Product", dataOrder.Aliases.ElementAt(0).Data.Item1);
            Assert.AreEqual("OProduct", dataOrder.Aliases.ElementAt(0).Data.Item2);
            Assert.AreEqual("C", dataOrder.Aliases.ElementAt(1).Name);
            Assert.AreEqual("Name", dataOrder.Aliases.ElementAt(1).Data.Item1);
            Assert.AreEqual("CName", dataOrder.Aliases.ElementAt(1).Data.Item2);
        }
    }
}