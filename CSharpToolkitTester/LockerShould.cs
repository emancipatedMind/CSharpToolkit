namespace CSharpToolkitTester {
    using CSharpToolkit.Utilities;
    using NUnit.Framework;
    [TestFixture]
    public class LockerShould {

        [Test]
        public void BeFreeByDefault() {
            var locker = new Locker();

            Assert.AreEqual(LockStatus.Free, locker.CurrentStatus);
        }

        [Test]
        public void HaveCurrentStatusOfLockedAfterRequestLock() {
            var token = new object();
            var locker = new Locker();

            locker.RequestLock(token);

            Assert.AreEqual(LockStatus.Locked, locker.CurrentStatus);
        } 

        [Test]
        public void FireLockStatusChangedAfterRequestLockIfCurrentStatusFree() {
            var token = new object();
            var locker = new Locker();

            Assert.AreEqual(LockStatus.Free, locker.CurrentStatus);

            bool eventRan = false;
            LockStatus eventStatus = LockStatus.Free;
            locker.LockStatusChanged += (s, e) => {
                eventRan = true;
                eventStatus = e.Data;
            }; 
            locker.RequestLock(token);

            Assert.IsTrue(eventRan);
            Assert.AreEqual(LockStatus.Locked, eventStatus);
        }

        [Test]
        public void NotFireLockStatusChangedAfterRequestLockIfCurrentStatusLocked() {
            var token = new object();
            var locker = new Locker();
            locker.RequestLock(locker);

            Assert.AreEqual(LockStatus.Locked, locker.CurrentStatus);

            bool eventRan = false;
            locker.LockStatusChanged += (s, e) => eventRan = true;
            locker.RequestLock(token);

            Assert.IsFalse(eventRan);
        }

        [Test]
        public void NotFireLockStatusChangedUnlessAllTokensRedeemed() {
            var token = new object();
            var secondToken = new object();
            var thirdToken = new object();
            var locker = new Locker();
            locker.RequestLock(token);
            locker.RequestLock(secondToken);
            locker.RequestLock(thirdToken);

            bool eventRan = false;
            locker.LockStatusChanged += (s, e) => eventRan = true;

            locker.RequestUnlock(token);
            Assert.IsFalse(eventRan);

            locker.RequestUnlock(secondToken);
            Assert.IsFalse(eventRan);

            locker.RequestUnlock(thirdToken);
            Assert.IsTrue(eventRan);
        }

        [Test]
        public void HaveCurrentStatusLockedUntilAllTokensRedeemed() {
            var token = new object();
            var secondToken = new object();
            var thirdToken = new object();
            var locker = new Locker();
            locker.RequestLock(token);
            locker.RequestLock(secondToken);
            locker.RequestLock(thirdToken);

            locker.RequestUnlock(token);
            Assert.AreEqual(LockStatus.Locked, locker.CurrentStatus);

            locker.RequestUnlock(secondToken);
            Assert.AreEqual(LockStatus.Locked, locker.CurrentStatus);

            locker.RequestUnlock(thirdToken);
            Assert.AreEqual(LockStatus.Free, locker.CurrentStatus);
        }

        [Test]
        public void NotFireLockChangedUntilCorrectTokenRedeemed() {
            var token = new object();
            var secondToken = new object();
            var thirdToken = new object();
            var locker = new Locker();
            locker.RequestLock(token);

            bool eventRan = false;
            locker.LockStatusChanged += (s, e) => eventRan = true;

            locker.RequestUnlock(thirdToken);
            Assert.IsFalse(eventRan);

            locker.RequestUnlock(secondToken);
            Assert.IsFalse(eventRan);

            locker.RequestUnlock(token);
            Assert.IsTrue(eventRan);
        }

        [Test]
        public void HaveCurrentStatusLockedUntilCorrectTokenRedeemed() {
            var token = new object();
            var secondToken = new object();
            var thirdToken = new object();
            var locker = new Locker();
            locker.RequestLock(token);

            locker.RequestUnlock(thirdToken);
            Assert.AreEqual(LockStatus.Locked, locker.CurrentStatus);

            locker.RequestUnlock(secondToken);
            Assert.AreEqual(LockStatus.Locked, locker.CurrentStatus);

            locker.RequestUnlock(token);
            Assert.AreEqual(LockStatus.Free, locker.CurrentStatus);
        }

    }
}