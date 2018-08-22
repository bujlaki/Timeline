using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace Timeline.Models
{
    public class MEventType
    {
        public string TypeName { get; set; }
        public Color Color { get; set; }

        public MEventType()
        {
            TypeName = "";
            Color = Color.Black;
        }

        public MEventType(string _name, Color _color) : this()
        {
            TypeName = _name;
            Color = _color;
        }

        public MEventType Copy()
        {
            MEventType target = new MEventType();
            target.TypeName = TypeName;
            target.Color = Color;
            return target;
        }
    }

}
