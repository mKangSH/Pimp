using Pimp.UI.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Pimp.UI.Models
{
    public class PimpObject : IVisibility
    {
        protected Visibility _visibility;
        public Visibility Visibility
        {
            get => _visibility;
            set { _visibility = value; }
        }

        public Vector3 Transform { get; set; }

        public Vector3 Rotation { get; set; }

        public Vector3 Scale { get; set; }
    }
}
