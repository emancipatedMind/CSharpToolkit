namespace CSharpToolkit.DataAccess {
    using System.Collections.Generic;
    using System.Linq;
    public abstract class ResultBase {

        public void AddAliases(DataRowWrapper obj, string name, IEnumerable<IAlias> aliases) =>
            aliases.Where(a => a.Name == name).ToList().ForEach(a => obj.AddAlias(a.Data));

    }
}