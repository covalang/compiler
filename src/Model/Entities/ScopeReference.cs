// using System;
// using System.Collections.Generic;
//
// namespace Cova.Model
// {
//     public class ScopeReference : EntityBase
//     {
//         private ScopeReference() {}
//         public ScopeReference(String scopeName) => ScopeName = scopeName;
//
//         public String ScopeName { get; set; } = null!;
//         public List<String> TypeParameterNames { get; } = new();
//     }
//     public class QualifiedScopeReference : EntityBase
//     {
//         public List<ScopeReference> ScopeReferences { get; } = new();
//     }
// }