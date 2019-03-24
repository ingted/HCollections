﻿namespace HCollections

open TypeEquality

[<NoComparison>]
[<NoEquality>]
type HListT<'ts, 'elem> =
    private
    | Empty of Teq<'ts, unit>
    | Cons of HListTConsCrate<'ts, 'elem>

and private HListTConsEvaluator<'ts, 'elem, 'ret> =
    abstract member Eval<'t, 'ts2> : 't -> 'elem -> HListT<'ts2, 'elem> -> Teq<'ts, 't -> 'ts2> -> 'ret

and private HListTConsCrate<'ts, 'elem> =
    abstract member Apply<'ret> : HListTConsEvaluator<'ts, 'elem, 'ret> -> 'ret

type HListTFolder<'state, 'elem> =
    abstract member Folder<'a> : 'state -> 'a -> 'elem -> 'state

module HListT =

    let cong (teq1 : Teq<'ts1, 'ts2>) (_ : Teq<'elem1, 'elem2>) : Teq<HListT<'ts1, 'elem1>, HListT<'ts2, 'elem2>> =
        Teq.Cong.believeMe teq1

    let empty<'elem> : HListT<unit, 'elem> = HListT.Empty Teq.refl

    let cons (x : 'a) (elem : 'elem) (xs : HListT<'ts, 'elem>) =
        HListT.Cons
            { new HListTConsCrate<_, _> with
                member __.Apply e = e.Eval x elem xs Teq.refl
            }

    let rec length<'ts, 'elem> (xs : HListT<'ts, 'elem>) : int =
        match xs with
        | Empty _ -> 0
        | Cons b ->
            b.Apply
                { new HListTConsEvaluator<_,_,_> with
                    member __.Eval _ _ xs _ = length xs + 1
                }

    let head (xs : HListT<'t -> 'ts, 'elem>) : 't * 'elem =
        match xs with
        | Empty _ -> raise Unreachable
        | Cons b ->
            b.Apply
                { new HListTConsEvaluator<_,_,_> with
                    member __.Eval x y _ teq =
                        let teq = teq |> Teq.Cong.domainOf
                        x |> Teq.castFrom teq, y
                }

    let tail (xs : HListT<'t -> 'ts, 'elem>) : HListT<'ts, 'elem> =
        match xs with
        | Empty _ -> raise Unreachable
        | Cons b ->
            b.Apply
                { new HListTConsEvaluator<_,_,_> with
                    member __.Eval _ _ xs teq =
                        let teq = cong (teq |> Teq.Cong.rangeOf) Teq.refl
                        xs |> Teq.castFrom teq
                }

    let rec fold<'state, 'elem, 'a> (folder : HListTFolder<'state, 'elem>) (seed : 'state) (xs : HListT<'a, 'elem>) : 'state =
        match xs with
        | Empty _ -> seed
        | Cons c ->
            c.Apply
                { new HListTConsEvaluator<_,_,_> with
                    member __.Eval x y xs _ = fold folder (folder.Folder seed x y) xs
                }