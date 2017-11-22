[1mdiff --git a/CSharpToolkit/Abstractions/IEntityBase.cs b/CSharpToolkit/Abstractions/IEntityBase.cs[m
[1mindex f3e7e03..e8cf14d 100644[m
[1m--- a/CSharpToolkit/Abstractions/IEntityBase.cs[m
[1m+++ b/CSharpToolkit/Abstractions/IEntityBase.cs[m
[36m@@ -1,5 +1,5 @@[m
 ﻿namespace CSharpToolkit.Abstractions {[m
     using System.ComponentModel;[m
[31m-    public interface IEntityBase : IStringNotification, INotifyDataErrorInfo, INotifyPropertyChanged, IDataErrorInfo {[m
[32m+[m[32m    public interface IEntityBase : IUserNotifier, INotifyDataErrorInfo, INotifyPropertyChanged, IDataErrorInfo {[m
     }[m
 }[m
\ No newline at end of file[m
[1mdiff --git a/CSharpToolkit/Abstractions/IStringNotification.cs b/CSharpToolkit/Abstractions/IStringNotification.cs[m
[1mdeleted file mode 100644[m
[1mindex 58e8c6a..0000000[m
[1m--- a/CSharpToolkit/Abstractions/IStringNotification.cs[m
[1m+++ /dev/null[m
[36m@@ -1,7 +0,0 @@[m
[31m-﻿namespace CSharpToolkit.Abstractions {[m
[31m-    using System;[m
[31m-    using EventArgs;[m
[31m-    public interface IStringNotification {[m
[31m-        event EventHandler<GenericEventArgs<string>> Notify;[m
[31m-    }[m
[31m-}[m
\ No newline at end of file[m
[1mdiff --git a/CSharpToolkit/CSharpToolkit.csproj b/CSharpToolkit/CSharpToolkit.csproj[m
[1mindex 03c0f0d..51bd1a0 100644[m
[1m--- a/CSharpToolkit/CSharpToolkit.csproj[m
[1m+++ b/CSharpToolkit/CSharpToolkit.csproj[m
[36m@@ -55,7 +55,8 @@[m
     <Reference Include="WindowsBase" />[m
   </ItemGroup>[m
   <ItemGroup>[m
[31m-    <Compile Include="Abstractions\IStringNotification.cs" />[m
[32m+[m[32m    <Compile Include="Abstractions\IUserNotifier.cs" />[m[41m[m
[32m+[m[32m    <Compile Include="Abstractions\Urgency.cs" />[m[41m[m
     <Compile Include="Algorithms\BoyerMoore.cs" />[m
     <Compile Include="Algorithms\PeriodicMethodInvoker.cs" />[m
     <Compile Include="Algorithms\RunsLessThanSeed.cs" />[m
[1mdiff --git a/CSharpToolkit/Utilities/Get.cs b/CSharpToolkit/Utilities/Get.cs[m
[1mindex d64a6ae..fce66ab 100644[m
[1m--- a/CSharpToolkit/Utilities/Get.cs[m
[1m+++ b/CSharpToolkit/Utilities/Get.cs[m
[36m@@ -5,10 +5,22 @@[m
     using System.Linq;[m
     public static class Get {[m
 [m
[31m-        public static OperationResult<T> EnumValue<T>(object v) where T : struct =>[m
[31m-            typeof(T).IsEnum ?[m
[31m-                new OperationResult<T>(Enum.IsDefined(typeof(T), v), (T)v) :[m
[31m-                new OperationResult<T>(new Exception[] { new NotSupportedException("Type must be System.Enum.") });[m
[32m+[m[32m        /*[m
[32m+[m[32m        public static OperationResult<T> EnumValue<T>(object v) {[m
[32m+[m[32m            try {[m
[32m+[m[32m                if (Enum.IsDefined(typeof(T), v)) {[m
[32m+[m[32m                    if (v.GetType() == typeof(string))[m
[32m+[m[32m                        return new OperationResult<T>((T)Enum.Parse(typeof(T), v.ToString(), true));[m
[32m+[m
[32m+[m[32m                    return new OperationResult<T>((T)v);[m
[32m+[m[32m                }[m
[32m+[m[32m                return new OperationResult<T>(false, default(T));[m
[32m+[m[32m            }[m
[32m+[m[32m            catch (Exception ex) {[m
[32m+[m[32m                return new OperationResult<T>(new[] { ex });[m
[32m+[m[32m            }[m
[32m+[m[32m        }[m
[32m+[m[32m        */[m
 [m
         public static List<T> List<T>(Action<List<T>> action) =>[m
             General(action);[m
[1mdiff --git a/CSharpToolkit/Utilities/Operations.cs b/CSharpToolkit/Utilities/Operations.cs[m
[1mindex 45ee7b1..b7c6675 100644[m
[1m--- a/CSharpToolkit/Utilities/Operations.cs[m
[1m+++ b/CSharpToolkit/Utilities/Operations.cs[m
[36m@@ -4,6 +4,14 @@[m
     using System.Runtime.InteropServices;[m
     using System.Security;[m
     public static class Operations {[m
[32m+[m
[32m+[m[32m        public static int? DecodeInt(string intString) {[m
[32m+[m[32m            int n;[m
[32m+[m[32m            if (int.TryParse(intString, out n))[m
[32m+[m[32m                return n;[m
[32m+[m[32m            return null;[m
[32m+[m[32m        }[m[41m [m
[32m+[m
         public static OperationResult<string> DecodeSecureString(SecureString value) {[m
             IntPtr valuePtr = IntPtr.Zero;[m
             try {[m
[1mdiff --git a/CSharpToolkit/XAML/EntityBase.cs b/CSharpToolkit/XAML/EntityBase.cs[m
[1mindex 06666db..cb498bd 100644[m
[1m--- a/CSharpToolkit/XAML/EntityBase.cs[m
[1m+++ b/CSharpToolkit/XAML/EntityBase.cs[m
[36m@@ -20,7 +20,6 @@[m
 [m
         public bool HasErrors => _errors.Count != 0;[m
 [m
[31m-[m
         public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;[m
         protected void OnErrorsChanged(string propertyName) {[m
             ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));[m
[36m@@ -60,11 +59,11 @@[m
             PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));[m
         }[m
 [m
[31m-        public event EventHandler<GenericEventArgs<string>> Notify;[m
[31m-        protected void FireNotifyEvent(GenericEventArgs<string> e) =>[m
[32m+[m[32m        public event EventHandler<GenericEventArgs<string, Urgency>> Notify;[m
[32m+[m[32m        protected void FireNotifyEvent(GenericEventArgs<string, Urgency> e) =>[m
             Notify?.Invoke(this, e);[m
[31m-        protected void FireNotifyEvent(string notification) =>[m
[31m-            Notify?.Invoke(this, new GenericEventArgs<string>(notification));[m
[32m+[m[32m        protected void FireNotifyEvent(string notification, Urgency urgency) =>[m
[32m+[m[32m            Notify?.Invoke(this, new GenericEventArgs<string, Urgency>(notification, urgency));[m
 [m
         public virtual string Error { get; }[m
         public virtual string this[string columnName] => "";[m
