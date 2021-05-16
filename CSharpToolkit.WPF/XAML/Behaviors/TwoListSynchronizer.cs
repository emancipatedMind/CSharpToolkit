namespace CSharpToolkit.XAML.Behaviors {
    using System;
    using System.Collections;
    using System.Collections.Specialized;
    using System.Linq;
    using System.Windows;
    using Abstractions;
    /// <summary>
    /// Used to keep two lists synchronized.
    /// </summary>
    public class TwoListSynchronizer : IWeakEventListener {

        readonly static IListItemConverter DefaultConverter = new CallbackListItemConverter();
        readonly IList _masterList;
        readonly IListItemConverter _masterTargetConverter;
        readonly IList _targetList;

        /// <summary>
        /// Initializes a new instance of the <see cref="TwoListSynchronizer"/> class.
        /// </summary>
        /// <param name="masterList">The master list.</param>
        /// <param name="targetList">The target list.</param>
        public TwoListSynchronizer(IList masterList, IList targetList) : this(masterList, targetList, DefaultConverter) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="TwoListSynchronizer"/> class.
        /// </summary>
        /// <param name="masterList">The master list.</param>
        /// <param name="targetList">The target list.</param>
        /// <param name="masterTargetConverter">The master-target converter.</param>
        public TwoListSynchronizer(IList masterList, IList targetList, IListItemConverter masterTargetConverter) {
            _masterList = masterList;
            _targetList = targetList;
            _masterTargetConverter = masterTargetConverter ?? DefaultConverter;
        }

        /// <summary>
        /// Receives events from the centralized event manager.
        /// </summary>
        /// <param name="managerType">The type of the <see cref="T:System.Windows.WeakEventManager"/> calling this method.</param>
        /// <param name="sender">Object that originated the event.</param>
        /// <param name="e">Event data.</param>
        /// <returns>
        /// true if the listener handled the event. It is considered an error by the <see cref="T:System.Windows.WeakEventManager"/> handling in WPF to register a listener for an event that the listener does not handle. Regardless, the method should return false if it receives an event that it does not recognize or handle.
        /// </returns>
        public bool ReceiveWeakEvent(Type managerType, object sender, EventArgs e) {
            if ((sender is IList && e is NotifyCollectionChangedEventArgs) == false)
                return false;

            HandleCollectionChanged((IList)sender, (NotifyCollectionChangedEventArgs)e);
            return true;
        }

        /// <summary>
        /// Start synchronizing the lists.
        /// </summary>
        public void StartSynchronizing() {
            SetListValuesFromSource(_masterList, _targetList, ConvertFromMasterToTarget);

            bool targetAndMasterCollectionsAreNotEqual =
                _masterList
                    .Cast<object>()
                    .SequenceEqual(
                        _targetList
                            .Cast<object>()
                            .Select(ConvertFromTargetToMaster)
                    ) == false;

            if (targetAndMasterCollectionsAreNotEqual)
                SetListValuesFromSource(_targetList, _masterList, ConvertFromTargetToMaster);

            ListenForChangeEvents(_masterList);
            ListenForChangeEvents(_targetList);
        }

        /// <summary>
        /// Stop synchronizing the lists.
        /// </summary>
        public void StopSynchronizing() {
            StopListeningForChangeEvents(_masterList);
            StopListeningForChangeEvents(_targetList);
        }

        /// <summary>
        /// Listens for change events on a list.
        /// </summary>
        /// <param name="list">The list to listen to.</param>
        protected void ListenForChangeEvents(IList list) {
            if (list is INotifyCollectionChanged)
                CollectionChangedEventManager.AddListener((INotifyCollectionChanged)list, this);
        }

        /// <summary>
        /// Stops listening for change events.
        /// </summary>
        /// <param name="list">The list to stop listening to.</param>
        protected void StopListeningForChangeEvents(IList list) {
            if (list is INotifyCollectionChanged)
                CollectionChangedEventManager.RemoveListener((INotifyCollectionChanged)list, this);
        }

        private object ConvertFromMasterToTarget(object item) => _masterTargetConverter.Convert(item);
        private object ConvertFromTargetToMaster(object item) => _masterTargetConverter.ConvertBack(item);

        private void HandleCollectionChanged(object s, NotifyCollectionChangedEventArgs e) {
            IList sourceList = (IList)s;
            bool sourceListIsMasterList = ReferenceEquals(sourceList, _masterList);

            IList listToUpdate;
            Func<object, object> converter;

            if (sourceListIsMasterList) {
                listToUpdate = _targetList;
                converter = ConvertFromMasterToTarget;
            }
            else {
                listToUpdate = _masterList;
                converter = ConvertFromTargetToMaster;
            }

            StopListeningForChangeEvents(listToUpdate);
            switch (e.Action) {
                case NotifyCollectionChangedAction.Add:
                    AddItems(listToUpdate, e, converter);
                    break;
                case NotifyCollectionChangedAction.Move:
                case NotifyCollectionChangedAction.Replace:
                    RemoveItems(listToUpdate, e, converter);
                    AddItems(listToUpdate, e, converter);
                    break;
                case NotifyCollectionChangedAction.Remove:
                    RemoveItems(listToUpdate, e, converter);
                    break;
                case NotifyCollectionChangedAction.Reset:
                    SetListValuesFromSource(sourceList, listToUpdate, converter);
                    break;
            }
            ListenForChangeEvents(listToUpdate);
        }

        #region Static Helpers
        protected static void SetListValuesFromSource(IList sourceList, IList listToUpdate, Func<object, object> converter) {
            listToUpdate.Clear();

            foreach (object item in sourceList)
                listToUpdate.Add(converter(item));
        }

        protected static void AddItems(IList listToUpdate, NotifyCollectionChangedEventArgs e, Func<object, object> converter) {
            int length = e.NewItems.Count;

            for (int i = 0; i < length; i++) {
                int insertionPoint = e.NewStartingIndex + i;
                object item = converter(e.NewItems[i]);

                if (insertionPoint > listToUpdate.Count)
                    listToUpdate.Add(item);
                else
                    listToUpdate.Insert(insertionPoint, item);
            }
        }

        protected static void RemoveItems(IList listToUpdate, NotifyCollectionChangedEventArgs e, Func<object, object> converter) {
            int length = e.OldItems.Count;
            for (int i = 0; i < length; i++)
                listToUpdate.RemoveAt(e.OldStartingIndex);
        }
        #endregion Static Helpers

    }
}
