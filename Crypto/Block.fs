namespace Core

open System
open System.Security.Cryptography
open System.Text.RegularExpressions

type Block (transaction, prevHash) = 

    (*let (|Satisfied|_|) str = 
        let m = Regex.Match(str, "00000000.*")
        if (m.Success) then Some (Satisfied)*)

    let hashToString (hash: byte[]) = Array.fold (fun acc b -> acc + sprintf "%i" b) "" hash

    member this.Transaction: int = transaction
    member this.PrevHash: byte[] = prevHash
    member this.TimeStamp: DateTime = DateTime.Now

    member this.Combined = sprintf "%d%s%s" this.Transaction (this.PrevHash |> hashToString) (this.TimeStamp.ToString())

    (*member this.Mine (nonce: int) = 
        let hash = sprintf "%s%i" this.Combined nonce
        if *)
        


