using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Timeline.Views
{

    public class VUserPagesMenuItem
    {
        public VUserPagesMenuItem()
        {
            TargetType = typeof(VUserPagesDetail);
        }
        public int Id { get; set; }
        public string Title { get; set; }

        public Type TargetType { get; set; }
    }
}