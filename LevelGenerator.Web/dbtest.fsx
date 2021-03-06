#I "../packages"
#I "../build"
#I "../../repository/packages"
#I "../../repository/build"

#r "WindowsAzure.Storage/lib/net45/Microsoft.WindowsAzure.Storage.dll"

#load "dbConnection.fsx"

open System
open System.IO
open Microsoft.WindowsAzure
open Microsoft.WindowsAzure.Storage
open Microsoft.WindowsAzure.Storage.Table
open DbConnection

let storageAccount = CloudStorageAccount.Parse connectionString
let tableClient = storageAccount.CreateCloudTableClient ()
let table = tableClient.GetTableReference("testTable")

type Customer(firstName, lastName, email: string, phone: string) =
    inherit TableEntity(partitionKey=lastName, rowKey=firstName)
    new() = Customer(null, null, null, null)
    member val Email = email with get, set
    member val PhoneNumber = phone with get, set


let customer = Customer("test", "test", "Walter@contoso.com", "425-555-0101")

let insertOp = TableOperation.Insert(customer)
let dostuff () = 
    table.CreateIfNotExists () |> ignore
    table.Execute(insertOp)

