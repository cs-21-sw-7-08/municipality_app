using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MunicipalityApp
{
    public class SubCategoryGroupHeader : BindableBase
    {
        private string title;
        private int categoryId;

        public SubCategoryGroupHeader()
        {

        }

        public string Title { get => title; set => SetValue(ref title, value); }
        public int CategoryId { get => categoryId; set => SetValue(ref categoryId, value); }
    }
}
