namespace CSharpToolkit.Utilities.EventArgs {
    using Utilities;
    public class DualLockEventArgs : System.EventArgs {

        public DualLockEventArgs(LockStatus primaryStatus, LockStatus secondaryStatus) {
            PrimaryStatus = primaryStatus;
            SecondaryStatus = secondaryStatus;
        } 

        public LockStatus PrimaryStatus { get; }
        public LockStatus SecondaryStatus { get; }

        public override string ToString() => $"{nameof(PrimaryStatus)}:{PrimaryStatus};{nameof(SecondaryStatus)}:{SecondaryStatus}";
        public override int GetHashCode() => ToString().GetHashCode();
        public override bool Equals(object obj) => obj is DualLockEventArgs && obj.GetHashCode() == GetHashCode();
    }
}