﻿namespace HCollections

open TypeEquality

/// HUnionTail represents the tail of an HUnion, that is the
/// choices in the union that appear *after* the type corresponds
/// to the single value held in the union type.
///
/// For example, if we have an (int -> string -> bool -> unit) HUnion
/// which holds a string, then this HUnion will contain a
/// (bool -> unit) HUnionTail.
///
/// In order to create an HUnion, you must supply a value and an
/// HUnionTail - this gives the HUnion a unique type and guarantees
/// that it contains exactly one element.
[<NoComparison>]
[<NoEquality>]
type 'ts HUnionTail

[<RequireQualifiedAccess>]
module HUnionTail =

    /// The unique empty HUnionTail
    val empty : unit HUnionTail

    /// Given an HUnionTail, extends it by introducing a new type
    /// corresponding to a new choice in the union.
    val extend : 'ts HUnionTail -> ('t -> 'ts) HUnionTail


/// HUnion is a heterogeneous union type, that is a generic way of
/// representing choice types.
///
/// We the types of each of the cases in the union in the single generic
/// type parameter of HUnion.
///
/// For example, if we want to represent the choice type int or string
/// or bool, we can represent this as the type:
///     (int -> string -> bool -> unit) HUnion.
[<NoComparison>]
[<NoEquality>]
type 'ts HUnion

[<RequireQualifiedAccess>]
module HUnion =

    /// Congruence proof for HUnions - given a proof of equality between two types 'ts1 and 'ts2,
    /// returns a proof that 'ts1 HUnion and 'ts2 HUnion are the same type.
    val cong : Teq<'ts1, 'ts2> -> Teq<'ts1 HUnion, 'ts2 HUnion>

    /// Given an HUnionTail and a value, creates an HUnion whose cases are exactly the
    /// cases in the HUnionTail, plus one case for the value supplied.
    /// Notice that when an HUnion is created using make, the value that it holds is always
    /// the first of the choices. Use HUnion.Extend to prepend further choices.
    val make : 'ts HUnionTail -> 't -> ('t -> 'ts) HUnion

    /// Given an HUnion, extends the choices by prepending a single additional choice to
    /// the front. Note that we do not have to supply a value as the HUnion must, by
    /// definition, already be holding a single value.
    val extend : 'ts HUnion -> ('t -> 'ts) HUnion

    /// Given a ('t -> 'ts) HUnion, returns a choice of either a 't (in the case where
    /// the value of the union corresponded to the first case of the choice) or a
    /// 'ts HUnion in the case where the value of the union corresponds to one of the
    /// choices denoted by 'ts.
    val split : ('t -> 'ts) HUnion -> Choice<'t, 'ts HUnion>