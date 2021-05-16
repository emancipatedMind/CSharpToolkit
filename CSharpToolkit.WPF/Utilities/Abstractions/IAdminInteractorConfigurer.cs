namespace CSharpToolkit.Utilities.Abstractions {
    using System;
    using System.Threading.Tasks;
    public interface IAdminInteractorConfigurer {
        Func<Task> ActivateMainWindowCallback { set; }
    }
}
