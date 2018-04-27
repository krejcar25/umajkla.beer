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
    public partial class BasePage : TabbedPage
    {
        public BasePage()
        {
            InitializeComponent();
        }
    }
}