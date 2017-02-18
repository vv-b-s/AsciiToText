﻿using System;

using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Hardware;


using Translate;
using OCR_Feature;

using TinyIoC;
using Tesseract;
using Tesseract.Droid;
using XLabs.Ioc;
using XLabs.Ioc.TinyIOC;
using XLabs.Platform.Device;
//using XLabs.Platform.Services.Media;

namespace Ascii___Text
{
    [Activity(Label = "Ascii to Text", MainLauncher = true, Icon = "@drawable/icon", ScreenOrientation = ScreenOrientation.Portrait)]
    public class MainActivity : Activity
    {
        private int _base = 0;                                        //gets the position of the spinner
        public static string OCRtext { set; get; }                  // Stores the OCR text
        public static string Text { set; get; }                   // Takes the text of the changed textbox

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

            // Code itself

            #region OCR                                       // http://thatcsharpguy.com/post/tesseract-ocr-xamarin/

            int numCameras = Camera.NumberOfCameras;        // used to turn off the "Take a photo" feature if there's no camera available | https://stackoverflow.com/questions/1944117/check-if-device-has-a-camera

            if (numCameras > 0)
            {
                var container = TinyIoCContainer.Current;

                container.Register(AndroidDevice.CurrentDevice);
                container.Register<ITesseractApi>((cont, parapeters) =>
                {
                    return new TesseractApi(ApplicationContext, AssetsDeployment.OncePerInitialization);
                });

                Resolver.ResetResolver();
                Resolver.SetResolver(new TinyResolver(container));
            }
            else photoBT.Visibility = ViewStates.Invisible;


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
                    AsciiBox.Text = Translator.ConvertTo(Translator.Type.Ascii, TextBox.Text, (Translator.Base)_base);
                    Text = TextBox.Text;

                    if (TextBox.Text == "")
                        AsciiBox.Text = "";
                }
            };

            AsciiBox.TextChanged += delegate
            {
                if (AsciiBox.IsFocused)
                {
                    LabelText.Text = (AsciiBox.Text == "") ? "Enter your text here:" : "Your text is:";
                    LabelAscii.Text = (AsciiBox.Text == "") ? "Or enter your Ascii code here:" : "Your text in Ascii is:";

                    if (Translatable())
                        TextBox.Text = Translator.ConvertTo(Translator.Type.Text, AsciiBox.Text, (Translator.Base)_base);

                    Text = TextBox.Text;

                    if (AsciiBox.Text == "")
                        TextBox.Text = "";
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
            if (e.Position == (int)Translator.Base.Binary)                                                                    
                AsciiBox.Text = Translator.ConvertTo(Translator.Type.Ascii, TextBox.Text, Translator.Base.Binary);

            if (e.Position == (int)Translator.Base.Decimal)                                                                       
                AsciiBox.Text = Translator.ConvertTo(Translator.Type.Ascii, TextBox.Text, Translator.Base.Decimal);

            if (e.Position == (int)Translator.Base.Hexadecimal)                                                                       
                AsciiBox.Text = Translator.ConvertTo(Translator.Type.Ascii, TextBox.Text, Translator.Base.Hexadecimal);

            if (e.Position == (int)Translator.Base.Octadecimal)                                                                    
                AsciiBox.Text = Translator.ConvertTo(Translator.Type.Ascii, TextBox.Text, Translator.Base.Octadecimal);

        }

        private bool Translatable()                                    // Checks if Ascii can be converted into text
        {
            var AsciiBox = FindViewById<EditText>(Resource.Id.AsciiBox);

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

