using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Foundation.Metadata;

using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI.ViewManagement;

using Translate;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace AsciiToTextUWP
{
    public sealed partial class MainPage : Page
    {
        private int _base = 0;                                                   // Keeps information about the base
        private bool TextBoxIsFocused = false, AsciiBoxIsFocused = false;       // Reports the focus state
        
        public MainPage()
        {
            this.InitializeComponent();
            ShowStatusBar();

            foreach (var item in Enum.GetValues(typeof(Translator.Base)))
                spinner.Items.Add(item);
            spinner.SelectedIndex = 0;

            photoBT.Visibility = Visibility.Collapsed;
        }

        private async void ShowStatusBar()
        {
            // turn on SystemTray for mobile
            // don't forget to add a Reference to Windows Mobile Extensions For The UWP
            if (Windows.Foundation.Metadata.ApiInformation.IsTypePresent("Windows.UI.ViewManagement.StatusBar"))
            {
                var statusbar = Windows.UI.ViewManagement.StatusBar.GetForCurrentView();
                await statusbar.ShowAsync();
                statusbar.BackgroundColor = Windows.UI.Colors.Black;
                statusbar.BackgroundOpacity = 1;
                statusbar.ForegroundColor = Windows.UI.Colors.White;
            }
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if(TextBoxIsFocused)
            {
                LabelText.Text = (TextBox.Text == "") ? "Enter your text here:" : "Your text is:";
                LabelAscii.Text = (TextBox.Text == "") ? "Or enter your Ascii code here:" : "Your text in Ascii is:";
                AsciiBox.Text = Translator.ConvertTo(Translator.Type.Ascii, TextBox.Text, (Translator.Base)_base);
                Translator.InputText = TextBox.Text;

                if (TextBox.Text == "")
                    AsciiBox.Text = "";
            }
        }

        private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            TextBoxIsFocused = true;
            AsciiBoxIsFocused = false;
        }

        private void AsciiBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if(AsciiBoxIsFocused)
            {
                LabelText.Text = (AsciiBox.Text == "") ? "Enter your text here:" : "Your text is:";
                LabelAscii.Text = (AsciiBox.Text == "") ? "Or enter your Ascii code here:" : "Your text in Ascii is:";

                if (Translatable())
                    TextBox.Text = Translator.ConvertTo(Translator.Type.Text, AsciiBox.Text, (Translator.Base)_base);

                Translator.InputText = TextBox.Text;

                if (AsciiBox.Text == "")
                    TextBox.Text = "";
            }
        }

        private void AsciiBox_GotFocus(object sender, RoutedEventArgs e)
        {
            TextBoxIsFocused = false;
            AsciiBoxIsFocused = true;
        }

       
        private void spinner_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (spinner.SelectedIndex == (int)Translator.Base.Binary)
                AsciiBox.Text = Translator.ConvertTo(Translator.Type.Ascii, TextBox.Text, Translator.Base.Binary);

            if (spinner.SelectedIndex == (int)Translator.Base.Decimal)
                AsciiBox.Text = Translator.ConvertTo(Translator.Type.Ascii, TextBox.Text, Translator.Base.Decimal);

            if (spinner.SelectedIndex == (int)Translator.Base.Hexadecimal)
                AsciiBox.Text = Translator.ConvertTo(Translator.Type.Ascii, TextBox.Text, Translator.Base.Hexadecimal);

            if (spinner.SelectedIndex == (int)Translator.Base.Octadecimal)
                AsciiBox.Text = Translator.ConvertTo(Translator.Type.Ascii, TextBox.Text, Translator.Base.Octadecimal);
        }

        private bool Translatable()                                    // Checks if Ascii can be converted into text
        {

            bool translatable = true;
            switch ((Translator.Base)_base)
            {
                case Translator.Base.Binary: // Base 2
                    for (int i = 0; i < AsciiBox.Text.Length; i++)
                    {
                        if (AsciiBox.Text[i] == '0' || AsciiBox.Text[i] == '1' || AsciiBox.Text[i] == ' ')
                            continue;
                        else
                            translatable = false;
                    }
                    break;

                case Translator.Base.Decimal:
                    for (int i = 0; i < AsciiBox.Text.Length; i++)
                    {
                        if ((AsciiBox.Text[i] >= '0' && AsciiBox.Text[i] <= '9') || AsciiBox.Text[i] == ' ')
                            continue;
                        else
                            translatable = false;
                    }
                    if (AsciiBox.Text == "")
                        return false;
                    break;

                case Translator.Base.Hexadecimal:
                    string text = AsciiBox.Text;
                    for (int i = 0; i < text.Length; i++)
                    {
                        if ((text[i] >= 'A' && text[i] <= 'Z') || (text[i] >= 'a' && text[i] <= 'z') || (text[i] >= '0' && text[i] <= '9') || text[i] == ' ')
                            continue;
                        else
                            return false;
                    }
                    break;

                case Translator.Base.Octadecimal:
                    for (int i = 0; i < AsciiBox.Text.Length; i++)
                    {
                        if ((AsciiBox.Text[i] >= '0' && AsciiBox.Text[i] <= '7') || AsciiBox.Text[i] == ' ')
                            continue;
                        else
                            translatable = false;
                    }
                    break;
            }
            return translatable;
        }
    }
}
