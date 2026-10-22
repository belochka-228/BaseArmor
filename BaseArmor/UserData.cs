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
        public static Restaurants SelectedRestaurant { get; set; }
        public static Halls SelectedHall { get; set; }
        public static int RestoranSort { get; set; } = 0;
        public static int DepositFilter { get; set; } = 0;
        public static Deposit CurrentDepozit { get; set; }
    }
    public partial class Users
    {
        public override string ToString()
        {
            return $"{login} ({last_name} {first_name})";
        }
    }

    public partial class DiningTable
    {
        public override string ToString()
        {
            return $"Стол {id}";
        }
    }

    public partial class Status
    {
        public override string ToString()
        {
            return name;
        }
    }
}
 