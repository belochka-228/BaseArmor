using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseArmor
{
    public static class UserData
    {
        public static Users CurrentUser { get; set; }
        public static bool CaptchaVisible { get; set; } = false;
        public static int FailedAttempts { get; set; } = 0;
    }
}
 