using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace beer.umajkla.client
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LoginPage : ContentPage
    {
        public LoginPage()
        {
            InitializeComponent();
            Content = new TableView
            {
                Intent = TableIntent.Form,
                Root = new TableRoot("Přihlášení") {
                    new TableSection ("") {
                        new EntryCell {
                            Label = "Uživatelské jméno",
                            Placeholder = "majkl@umajkla.beer",
                            Keyboard = Keyboard.Default
                        },
                        new EntryCell
                        {
                            Label="Token",
                            
                        }
                    }
                }
            };
        }

        private void startLoginButton_Clicked(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private void ToolbarItem_Clicked(object sender, EventArgs e)
        {

        }
    }
}