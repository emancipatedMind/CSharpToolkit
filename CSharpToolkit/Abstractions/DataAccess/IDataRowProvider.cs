﻿namespace CSharpToolkit.Abstractions.DataAccess {
    using System.Collections.Generic;
    using System.Data;
    using Utilities;
    public interface IDataRowProvider {
        OperationResult<List<DataRow>> SubmitQuery(string query);
    }
}