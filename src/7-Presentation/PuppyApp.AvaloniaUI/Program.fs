namespace CounterApp

open System.Collections.Generic
open System.Net.Mime
open Avalonia
open Avalonia.Controls.ApplicationLifetimes
open Avalonia.Themes.Fluent
open Avalonia.FuncUI.Hosts
open Avalonia.Controls
open Avalonia.FuncUI
open Avalonia.FuncUI.DSL
open Avalonia.Layout
open Puppy.SqlViewModels
open PuppyData.Types
open PuppyMapper.AvaloniaApp

module Main =
    
    let testFieldDefinitions: ParamHelper[] = [|
        {
            SpParamName = "StudentName"
            FriendlyName = "Student Name"
            PuppyInfo = PuppySqlTypeInfo.CreateText true 10
        }
        {
            SpParamName = "StudentAge"
            FriendlyName = "Age"
            PuppyInfo = PuppySqlTypeInfo.CreateInt true 2
        }
    |]
        
    let initialValues: IDictionary<string,string> =
        ["StudentName", "John"; "StudentAge", "20";]
        |> dict
    let view () =
        Component(fun ctx ->
            let vm = new Puppy.SqlViewModels.RequestFormViewModel(fieldDefinitions = testFieldDefinitions, initialValues = initialValues)
            let state = ctx.useState ( vm )
            let draftFields = vm.Fields |> Seq.map (fun f -> f) 
            DockPanel.create [
                DockPanel.children [
                    DockPanel.create [
                        DockPanel.children [
                            TextBlock.create [
                                TextBlock.padding (Thickness 8.0)
                                TextBlock.dock Dock.Left
                                TextBlock.verticalAlignment VerticalAlignment.Center
                                TextBlock.horizontalAlignment HorizontalAlignment.Right
                                // TextBlock.text (string "Document Name")
                            ]
                            TextBox.create [
                                TextBox.padding (Thickness 8.0)
                                TextBox.dock Dock.Right
                                TextBox.verticalAlignment VerticalAlignment.Center
                                TextBox.horizontalAlignment HorizontalAlignment.Stretch
                                // TextBox.text (string state.Current.DocumentName)
                                // TextBox.onTextChanged (fun s -> state.Current.DocumentName <- s)
                            ]
                        ]
                    ]
                ]
            ]
        )

type MainWindow() =
    inherit HostWindow()
    do
        base.Title <- "Counter Example"
        base.Content <- Main.view ()

type App() =
    inherit Application()

    override this.Initialize() =
        this.Styles.Add (FluentTheme())
        this.RequestedThemeVariant <- Styling.ThemeVariant.Dark

    override this.OnFrameworkInitializationCompleted() =
        match this.ApplicationLifetime with
        | :? IClassicDesktopStyleApplicationLifetime as desktopLifetime ->
            desktopLifetime.MainWindow <- MainWindow()
        | _ -> ()

module Program =

    [<EntryPoint>]
    let main(args: string[]) =
        AppBuilder
            .Configure<App>()
            .UsePlatformDetect()
            .UseSkia()
            .StartWithClassicDesktopLifetime(args)
