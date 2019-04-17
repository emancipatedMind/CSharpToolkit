namespace CSharpToolkit.DataAccess.Abstractions {
    public interface IQueryable {
        byte[] Query(byte[] request);
    }
}