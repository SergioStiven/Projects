using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Xpinn.SportsGo.Movil.Controls
{
    class ScrollViewInitTop : ScrollView
    {
        public ScrollViewInitTop()
        {
            ScrollToAsync(0, 0, false).RunSynchronously();
        }
    }
}
