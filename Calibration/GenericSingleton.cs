using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Calibration
{
    public class GenericSingleton<T> where T : Form, new()
    {
        private static T t = null;
        public static T CreateInstrance()
        {
            if (t == null || t.IsDisposed)
            {
                t = new T();
            }
            return t;
        }
    }
}
