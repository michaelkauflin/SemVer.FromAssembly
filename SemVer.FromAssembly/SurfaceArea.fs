﻿namespace SemVer.FromAssembly

module SurfaceArea =
    open System.Reflection
    open System
    open System.Text.RegularExpressions
    // from https://github.com/Microsoft/visualfsharp/blob/master/src/fsharp/FSharp.Core.Unittests/LibraryTestFx.fs
    // gets public surface area for the assembly
    let get (asm:Assembly) : Package=
    
        // public types only
        let types =
            asm.GetExportedTypes()

        //string list*Map<string,Type list>
        let getTypeMembers (t : Type) =
            t.GetMembers()
            |> Array.map (fun v -> v.ToString())
                             
        let actual =
            types 
            |> Array.map (fun t-> (t.Namespace, (t.Name,{Members=set (getTypeMembers t)})))
            |> Array.groupBy (fun (ns,_)->ns)
            |> Array.map (fun (ns,ns_ts)-> (ns,ns_ts |> Array.map snd)) 
            |> Array.map (fun (ns,ts)-> ((if ns <> null then ns else ""), 
                                           {
                                            Adts= ts|> Map.ofSeq 
                                           } ))
            |> Map.ofSeq
        {Namespaces=actual}
