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

        readonly static IListItemConverter DefaultConverter = new NullListItemConverter();
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
            _masterTargetConverter = masterTargetConverter;
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
            HandleCollectionChanged(sender as IList, e as NotifyCollectionChangedEventArgs);
            return true;
        }

        /// <summary>
        /// Start synchronizing the lists.
        /// </summary>
        public void StartSynchronizing() {
            ListenForChangeEvents(_masterList);
            ListenForChangeEvents(_targetList);

            SetListValuesFromSource(_masterList, _targetList, ConvertFromMasterToTarget);

            if (TargetAndMasterCollectionsAreEqual() == false)
                SetListValuesFromSource(_targetList, _masterList, ConvertFromTargetToMaster);
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

        private void AddItems(IList list, NotifyCollectionChangedEventArgs e, Func<object, object> converter) {
            int length = e.NewItems.Count;

            for (int i = 0; i < length; i++) {
                int insertionPoint = e.NewStartingIndex + i;
                object item = converter(e.NewItems[i]);

                if (insertionPoint > list.Count)
                    list.Add(item);
                else
                    list.Insert(insertionPoint, item);
            }
        }

        private object ConvertFromMasterToTarget(object item) =>
            _masterTargetConverter?.Convert(item) ?? item;

        private object ConvertFromTargetToMaster(object item) =>
            _masterTargetConverter?.ConvertBack(item) ?? item;

        private void HandleCollectionChanged(object sender, NotifyCollectionChangedEventArgs e) {
            IList sourceList = sender as IList;

            switch (e.Action) {
                case NotifyCollectionChangedAction.Add:
                    PerformActionOnAllLists(AddItems, sourceList, e);
                    break;
                case NotifyCollectionChangedAction.Move:
                    PerformActionOnAllLists(MoveItems, sourceList, e);
                    break;
                case NotifyCollectionChangedAction.Remove:
                    PerformActionOnAllLists(RemoveItems, sourceList, e);
                    break;
                case NotifyCollectionChangedAction.Replace:
                    PerformActionOnAllLists(ReplaceItems, sourceList, e);
                    break;
                case NotifyCollectionChangedAction.Reset:
                    UpdateListsFromSource(sourceList);
                    break;
            }
        }

        private void MoveItems(IList list, NotifyCollectionChangedEventArgs e, Func<object, object> converter) {
            RemoveItems(list, e, converter);
            AddItems(list, e, converter);
        }

        void PerformActionOnAllLists(Action<IList, NotifyCollectionChangedEventArgs, Func<object, object>> action, IList sourceList, NotifyCollectionChangedEventArgs e) {
            if (ReferenceEquals(sourceList, _masterList))
                PerformActionOnList(_targetList, action, e, ConvertFromMasterToTarget);
            else
                PerformActionOnList(_masterList, action, e, ConvertFromTargetToMaster);
        }

        private void PerformActionOnList(IList list, Action<IList, NotifyCollectionChangedEventArgs, Func<object, object>> action, NotifyCollectionChangedEventArgs e, Func<object, object> converter) {
            StopListeningForChangeEvents(list);
            action(list, e, converter);
            ListenForChangeEvents(list);
        }

        private void RemoveItems(IList list, NotifyCollectionChangedEventArgs e, Func<object, object> converter) {
            int length = e.OldItems.Count;
            for (int i = 0; i < length; i++)
                list.RemoveAt(e.OldStartingIndex);
        }

        private void ReplaceItems(IList list, NotifyCollectionChangedEventArgs e, Func<object, object> converter) {
            RemoveItems(list, e, converter);
            AddItems(list, e, converter);
        }

        private void SetListValuesFromSource(IList sourceList, IList targetList, Func<object, object> converter) {
            StopListeningForChangeEvents(targetList);
            targetList.Clear();

            foreach (var item in sourceList)
                targetList.Add(converter(item));

            ListenForChangeEvents(targetList);
        }

        private bool TargetAndMasterCollectionsAreEqual() =>
            _masterList.Cast<object>().SequenceEqual(_targetList.Cast<object>().Select(item => ConvertFromTargetToMaster(item)));

        private void UpdateListsFromSource(IList list) {
            if (ReferenceEquals(list, _masterList))
                SetListValuesFromSource(_masterList, _targetList, ConvertFromMasterToTarget);
            else
                SetListValuesFromSource(_targetList, _masterList, ConvertFromTargetToMaster);
        }

    }
}