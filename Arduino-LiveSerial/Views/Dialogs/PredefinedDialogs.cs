using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arduino_LiveSerial.View.Dialogs.Predefined
{
    static class PredefinedDialogs
    {
        public static async Task<object> MessageDialog(object datacontext, string message, DialogOpenedEventHandler doe, DialogClosingEventHandler dce, string rootDialog = "RootDialog")
        {
            var dialog = new MessageDialog
            {
                DataContext = datacontext,
                Message = {Text = message}
            };

            return await DialogHost.Show(dialog, rootDialog, doe, dce);
        }

        public static async Task<object> MessageDialog(object datacontext, string message, string buttonText, DialogOpenedEventHandler doe, DialogClosingEventHandler dce, string rootDialog = "RootDialog")
        {
            var dialog = new MessageDialog
            {
                DataContext = datacontext,
                Message = { Text = message },
                Button = { Content = buttonText }
            };

            return await DialogHost.Show(dialog, rootDialog, doe, dce);
        }

        public static async Task<object> TwoButtonsDialog(object datacontext, string message, string yesText, string noText, DialogOpenedEventHandler doe, DialogClosingEventHandler dce, string rootDialog = "RootDialog")
        {
            var dialog = new ConfirmDialog
            {
                DataContext = datacontext,
                Message = {Text = message},
                YesButton = {Content = yesText},
                NoButton = {Content = noText}
            };

            return await DialogHost.Show(dialog, rootDialog, doe, dce);
        }

        public static async Task<object> AcceptCancelDialog(object datacontext, string message, DialogOpenedEventHandler doe, DialogClosingEventHandler dce)
        {
            return await TwoButtonsDialog(datacontext, message, "ACCEPT", "CANCEL", doe, dce);
        }

        public static async Task<object> AcceptYesNoDialog(object datacontext, string message, DialogOpenedEventHandler doe, DialogClosingEventHandler dce)
        {
            return await TwoButtonsDialog(datacontext, message, "YES", "NO", doe, dce);
        }
    }
}
