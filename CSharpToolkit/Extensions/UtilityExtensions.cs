namespace CSharpToolkit.Extensions {
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Utilities.Abstractions;
    public static class UtilityExtensions {
        public static bool IsLocked(this ILocker locker) => locker.CurrentStatus == Utilities.LockStatus.Locked;
        public static bool IsFree(this ILocker locker) => locker.CurrentStatus == Utilities.LockStatus.Free;

        public static void LockForDuration(this ILocker locker, Action method) =>
            LockForDuration(locker, new object(), method);

        public static void LockForDuration(this ILocker locker, object token, Action method) {
            locker.RequestLock(token);
            method();
            locker.RequestUnlock(token);
        }

        public static TReturn LockForDuration<TReturn>(this ILocker locker, Func<TReturn> method) =>
            LockForDuration(locker, new object(), method);

        public static TReturn LockForDuration<TReturn>(this ILocker locker, object token, Func<TReturn> method) {
            locker.RequestLock(token);
            var returnObject = method();
            locker.RequestUnlock(token);
            return returnObject;
        }

        public static TReturn LockForDuration<TReturn>(this IEnumerable<ILocker> locker, Func<TReturn> method) =>
            LockForDuration(locker, new object(), method);

        public static TReturn LockForDuration<TReturn>(this IEnumerable<ILocker> lockers, object token, Func<TReturn> method) {
            if ((lockers?.Any() ?? false) == false) {
                return default(TReturn);
            }
            lockers.ForEach(locker => locker.RequestLock(token));
            var returnObject = method();
            lockers.ForEach(locker => locker.RequestUnlock(token));
            return returnObject;
        }

        public static void LockForDuration(this IEnumerable<ILocker> lockers, Action method) =>
            LockForDuration(lockers, new object(), method);
        public static void LockForDuration(this IEnumerable<ILocker> lockers, object token, Action method) {
            if ((lockers?.Any() ?? false) == false) {
                return;
            }
            lockers.ForEach(locker => locker.RequestLock(token));
            method();
            lockers.ForEach(locker => locker.RequestUnlock(token));
        }

        public static Task<TReturn> LockForDurationAsync<TReturn>(this ILocker locker, Func<Task<TReturn>> method) =>
            LockForDurationAsync(locker, new object(), method);

        public static async Task<TReturn> LockForDurationAsync<TReturn>(this ILocker locker, object token, Func<Task<TReturn>> method) {
            locker.RequestLock(token);
            var returnObject = await method();
            locker.RequestUnlock(token);
            return returnObject;
        }

        public static Task LockForDurationAsync(this ILocker locker, Func<Task> method) =>
            LockForDurationAsync(locker, new object(), method);
        public static async Task LockForDurationAsync(this ILocker locker, object token, Func<Task> method) {
            locker.RequestLock(token);
            await method();
            locker.RequestUnlock(token);
        }

        public static Task<TReturn> LockForDurationAsync<TReturn>(this IEnumerable<ILocker> locker, Func<Task<TReturn>> method) =>
            LockForDurationAsync(locker, new object(), method);

        public static async Task<TReturn> LockForDurationAsync<TReturn>(this IEnumerable<ILocker> lockers, object token, Func<Task<TReturn>> method) {
            if ((lockers?.Any() ?? false) == false) {
                return default(TReturn);
            }
            lockers.ForEach(locker => locker.RequestLock(token));
            var returnObject = await method();
            lockers.ForEach(locker => locker.RequestUnlock(token));
            return returnObject;
        }

        public static Task LockForDurationAsync(this IEnumerable<ILocker> lockers, Func<Task> method) =>
            LockForDurationAsync(lockers, new object(), method);
        public static async Task LockForDurationAsync(this IEnumerable<ILocker> lockers, object token, Func<Task> method) {
            if ((lockers?.Any() ?? false) == false) {
                return;
            }
            lockers.ForEach(locker => locker.RequestLock(token));
            await method();
            lockers.ForEach(locker => locker.RequestUnlock(token));
        }
    }
}