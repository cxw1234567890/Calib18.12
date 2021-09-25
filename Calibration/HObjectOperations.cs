using HalconDotNet;

namespace Calibration
{
    public class HObjectOperations
    {
        /// <summary>
        /// True表示图像变量OK
        /// </summary>
        /// <param name="Obj">待判断的HObject类型变量</param>
        /// <returns></returns>
        public static bool ObjectValided(HObject Obj)
        {
            if (Obj == null)
            {
                return false;
            }
            if (!Obj.IsInitialized())
            {
                return false;
            }
            if (Obj.CountObj() < 1)
            {
                return false;
            }
            return true;
        }
    }
}
