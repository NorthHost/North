﻿using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor;

namespace North.Pages
{
    partial class Upload
    {
        private bool Clearing = false;
        private static string DefaultDragClass = "relative rounded-lg border-2 border-dashed pa-4 mt-4 mud-width-full mud-height-full";
        private string DragClass = DefaultDragClass;
        private List<string> fileNames = new List<string>();

        private void OnInputFileChanged(InputFileChangeEventArgs e)
        {
        ClearDragClass();
        var files = e.GetMultipleFiles();
        foreach (var file in files)
        {
            fileNames.Add(file.Name);
        }
        }

        private async Task Clear()
        {
        Clearing = true;
        fileNames.Clear();
        ClearDragClass();
        await Task.Delay(100);
        Clearing = false;
        }
        private void UploadImage()
        {
        //Upload the files here
        Snackbar.Configuration.PositionClass = Defaults.Classes.Position.TopCenter;
        Snackbar.Add("TODO: Upload your files!", Severity.Normal);
        }

        private void SetDragClass()
        {
        DragClass = $"{DefaultDragClass} mud-border-primary";
        }

        private void ClearDragClass()
        {
        DragClass = DefaultDragClass;
        }
    }
}
