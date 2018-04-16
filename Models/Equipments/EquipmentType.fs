namespace Models.Equipments

module Equipments=

    open System
    open System.ComponentModel.DataAnnotations
    [<CLIMutable>]
    type EquipmetType= {[<Key>]id: Guid; Title: string}