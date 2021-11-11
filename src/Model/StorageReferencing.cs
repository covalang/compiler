using System;

namespace Cova.Model
{
    public abstract class StorageReferencing
    {
        public Ownership Ownership { get; set; }
        public Visibility Visibility { get; set; }
        public Mutability Mutability { get; set; }
        public Nullability Nullability { get; set; }
        public StorageType StorageType { get; set; }
        public CyclePossibility CyclePossibility { get; set; }
        public InstanceDependency InstanceDependency { get; set; }
        public ThreadShareability ThreadShareability { get; set; }
    }
    
    public enum Ownership : Byte { Unique, Shared }
    public enum Visibility : Byte { None, Private, Protected, Internal, Public }
    public enum Mutability : Byte { Immutable, Mutable }
    public enum Nullability : Byte { None, Nullable }
    public enum StorageType : Byte { Static, Instance }
    public enum CyclePossibility : Byte { Unknown, None, Some }
    public enum InstanceDependency : Byte { Value, Reference } // Independent, Interdependent
    public enum ThreadShareability : Byte { Local, Global }
}