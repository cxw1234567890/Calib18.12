using Calibration.BaslerSDK;
using System;
using System.Collections.Generic;

namespace Calibration
{
    public sealed class DataClass
    {
        private static readonly Lazy<DataClass> lazy = new Lazy<DataClass>(() => new DataClass());

        public static DataClass Instance { get { return lazy.Value; } }

        private DataClass()
        {
        }
        public List<BaslerCamera> BaslerList = new List<BaslerCamera>();
    }
}
