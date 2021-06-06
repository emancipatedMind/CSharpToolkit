namespace CSharpToolkit.ViewModels.Abstractions {
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.Linq;
    using System.Reflection;
    using Extensions;
    using Utilities;
    using Utilities.Abstractions;
    using XAML;

    public abstract class GroupedFindSelectorViewModel<TSource, TGroupedFindInfo, TPart, TPartImplementation> : EntityBase, IIdProvider, IContainsParts<TPart>
        where TSource : TGroupedFindInfo
        where TGroupedFindInfo : IIdProvider
        where TPart : IIdProvider
        where TPartImplementation : EntityBase, TPart, IIdProvider
    {

        static string[] GroupedFindInfoInterfaceProperties =
            typeof(TGroupedFindInfo).GetInterfaceProperties().Select(prop => prop.Name).ToArray();
        static PropertyInfo[] PartInterfaceProperties =
            typeof(TPart).GetInterfaceProperties().ToArray();

        bool _disposedValue;

        public GroupedFindSelectorViewModel() : this(0) { }
        public GroupedFindSelectorViewModel(int id) : this(new TSource[0]) {
            Id = id;
        }
        public GroupedFindSelectorViewModel(IEnumerable<TSource> group) {
            var sourceArray = group?.ToArray() ?? new TSource[0];
            if (sourceArray.Length != 0 && this is TGroupedFindInfo) {
                GroupedFindInfoInterfaceProperties.ForEach(name =>
                    Perform.PropertyAssignmentThroughReflection(new KeyValuePair<object, string>(sourceArray[0], name), new KeyValuePair<object, string>(this, name))
                );
            }

            RawCollection = new ConvertingObservableCollection<TPart, TPartImplementation>(CreateParts(sourceArray), Convert);
            RawCollection.CollectionChanged += RawItems_CollectionChanged;
        }

        public ConvertingObservableCollection<TPart, TPartImplementation> RawCollection { get; }
        public IList<TPart> Parts => RawCollection;

        public int Id {
            get { return PropertyStore.GetCurrentValue<int>(nameof(Id)); }
            set { PropertyStore.SetValue(nameof(Id), value); }
        }

        protected abstract IEnumerable<TPart> CreateParts(IEnumerable<TSource> source);
        protected abstract TPartImplementation CreatePart();

        protected virtual bool PartNameFilter(System.Reflection.PropertyInfo info) => true;
        TPartImplementation Convert(TPart item) {

            TPartImplementation itemToAdd = item is TPartImplementation ? (TPartImplementation)item : CreatePart();

            if (itemToAdd.Equals(item) == false) {
                PartInterfaceProperties.Where(PartNameFilter).ForEach(prop =>
                    Perform.PropertyAssignmentThroughReflection(new KeyValuePair<object, string>(item, prop.Name), new KeyValuePair<object, string>(itemToAdd, prop.Name))
                );

                CompleteConvert(itemToAdd, item);
            }

            return itemToAdd;
        }

        protected virtual void CompleteConvert(TPartImplementation itemToAdd, TPart item) { }

        protected virtual void Item_PropertyChanged(object sender, PropertyChangedEventArgs e) { }
        protected virtual void CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e) { }
        private void RawItems_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e) {
            if (e.Action == NotifyCollectionChangedAction.Add) {
                e.NewItems?
                    .Cast<INotifyPropertyChanged>()
                    .ForEach(s => s.PropertyChanged += Item_PropertyChanged);
            }
            else if (e.Action == NotifyCollectionChangedAction.Remove) {
                e.OldItems?
                    .Cast<INotifyPropertyChanged>()
                    .ForEach(s => s.PropertyChanged -= Item_PropertyChanged);
            }

            CollectionChanged(sender, e);
        }

        protected override void Dispose(bool disposing) {
            if (!_disposedValue) {
                if (disposing) {
                    RawCollection.Dispose();
                }
                _disposedValue = true;
            }
            base.Dispose(disposing);
        }

    }
}
