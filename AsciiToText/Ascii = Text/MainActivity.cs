using System;

using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;

using Translate;
using OCR_Feature;

using TinyIoC;
using Tesseract;
using Tesseract.Droid;
using XLabs.Ioc;
using XLabs.Ioc.TinyIOC;
using XLabs.Platform.Device;
using XLabs.Platform.Services.Media;

namespace Ascii___Text
{
    [Activity(Label = "Ascii to Text", MainLauncher = true, Icon = "@drawable/icon", ScreenOrientation = ScreenOrientation.Portrait)]
    public class MainActivity : Activity
    {
        private int _base = 0;              //gets the position of the spinner
        public static string OCRtext { set; get; }                  // Stores the OCR text

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);

            // Objects
            var LabelText = FindViewById<TextView>(Resource.Id.LabelText);
            var LabelAscii = FindViewById<TextView>(Resource.Id.LabelAscii);
            var TextBox = FindViewById<EditText>(Resource.Id.TextBox);
            var AsciiBox = FindViewById<EditText>(Resource.Id.AsciiBox);
            var spinner = FindViewById<Spinner>(Resource.Id.spinner);
            var photoBT = FindViewById<Button>(Resource.Id.photoBT);
            photoBT.Enabled = true;

            // Code itself

            #region OCR                     // http://thatcsharpguy.com/post/tesseract-ocr-xamarin/
            var container = TinyIoCContainer.Current;

            container.Register<IDevice>(AndroidDevice.CurrentDevice);
            container.Register<ITesseractApi>((cont, parapeters) =>
            {
                return new TesseractApi(ApplicationContext, AssetsDeployment.OncePerInitialization);
            });

            Resolver.ResetResolver();
            Resolver.SetResolver(new TinyResolver(container));

            #endregion

            #region Spinner
            //https://developer.xamarin.com/guides/android/user_interface/spinner/

            spinner.ItemSelected += new EventHandler<AdapterView.ItemSelectedEventArgs>(spinner_ItemSelected);                 // Creates the event for spinner
            var adapter = ArrayAdapter.CreateFromResource(this, Resource.Array.bases_array, Android.Resource.Layout.SimpleSpinnerItem);     // Makes items in spinner accessable
            adapter.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerItem);
            spinner.Adapter = adapter;          // Displays the item on the spinner itself

            #endregion Spinner

            TextBox.TextChanged += delegate
            {
                if (TextBox.IsFocused)
                {
                    LabelText.Text = (TextBox.Text == "") ? "Enter your text here:" : "Your text is:";
                    LabelAscii.Text = (TextBox.Text == "") ? "Or enter your Ascii code here:" : "Your text in Ascii is:";
                    AsciiBox.Text = Translator.ConvertTo(Translator.Type.Ascii, TextBox.Text,_base);
                }
            };

            AsciiBox.TextChanged += delegate
            {
                if(AsciiBox.IsFocused)
                {
                    LabelText.Text = (AsciiBox.Text == "") ? "Enter your text here:" : "Your text is:";
                    LabelAscii.Text = (AsciiBox.Text == "") ? "Or enter your Ascii code here:" : "Your text in Ascii is:";
                    
                    if (Translatable())
                        TextBox.Text = Translator.ConvertTo(Translator.Type.Text, AsciiBox.Text,_base);
                }                
            };

            photoBT.Click += (object sender, EventArgs e) =>                            // Gets the OCR text
             {
                 OCR.CeatePicture();
                 TextBox.Text = "After taking the photo wait for a few seconds and hold the \"Take a photo\" button to view the text.";
                 AsciiBox.Text = "";
             };

            photoBT.LongClick += delegate                       // Sends the OCR text to the focused box
              {
                  if (TextBox.IsFocused)
                      TextBox.Text = OCRtext;
                  else
                      AsciiBox.Text = OCRtext;
              };
        }

        private void spinner_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            Spinner spinner = (Spinner)sender;
            _base = e.Position;
            // Connecting Fields
            var TextBox = FindViewById<EditText>(Resource.Id.TextBox);
            var AsciiBox = FindViewById<EditText>(Resource.Id.AsciiBox);

            //Conversion Code
            if (e.Position == 0)                                                                    // Base 2
                AsciiBox.Text = Translator.ConvertTo(Translator.Type.Ascii, TextBox.Text, 0);
            if(e.Position==1)                                                                       // Base 10
                AsciiBox.Text = Translator.ConvertTo(Translator.Type.Ascii, TextBox.Text, 1);
            if (e.Position == 2)                                                                    // base 16
                AsciiBox.Text = Translator.ConvertTo(Translator.Type.Ascii, TextBox.Text, 2);
            if (e.Position == 3)                                                                    // Base 8
                AsciiBox.Text = Translator.ConvertTo(Translator.Type.Ascii, TextBox.Text, 3);

        }

        private bool Translatable()                                    // Checks if Ascii can be converted into text
        {
            var AsciiBox = FindViewById<EditText>(Resource.Id.AsciiBox);

            bool translatable = true;
            switch(_base)
            {
                case 0: // Base 2
                    for (int i = 0; i < AsciiBox.Text.Length; i++)
                    {
                        if (AsciiBox.Text[i] == '0' || AsciiBox.Text[i] == '1' || AsciiBox.Text[i] == ' ')
                            continue;
                        else
                            translatable = false;
                    }
                    break;

                case 1:                                                     //base 10
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

                case 2:                                         // base 16
                    string text = AsciiBox.Text;
                    for(int i = 0;i<text.Length;i++)
                    {
                        if ((text[i] >= 'A' && text[i] <= 'Z') || (text[i] >= 'a' && text[i] <= 'z') || (text[i] >= '0' && text[i] <= '9') || text[i] == ' ')
                            continue;
                        else
                            return false;
                    }
                    break;

                case 3:                                     // base 8        
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

