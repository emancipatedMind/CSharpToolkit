namespace CSharpToolkit.DataAccess.Abstractions {
    using System;
    public interface ICreatable {
        string Creator { get; set; }
        DateTime? DateCreated { get; set; }
        string Updator { get; set; }
        DateTime? DateUpdated { get; set; }
    }

}
