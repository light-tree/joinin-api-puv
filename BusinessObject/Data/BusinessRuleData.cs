using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.Data
{
    public static class BusinessRuleData
    {
        public static int PREMIUM_FEE_PER_MONTH = 50000;

        public static int GROUP_SIZE_FOR_FREEMIUM = 5;
        public static int MAX_MAIN_TASK_NUMBER_EACH_GROUP_FOR_FREEMIUM = 10;
        public static int MAX_SUB_TASK_NUMBER_EACH_MAIN_TASK_FOR_FREEMIUM = 5;
        public static int MAX_NUMBER_GROUP_FOR_FREEMIUM = 5;

        public static int GROUP_SIZE_FOR_PREMIUM = 20;
        public static int MAX_MAIN_TASK_NUMBER_EACH_GROUP_FOR_PREMIUM = 100;
        public static int MAX_SUB_TASK_NUMBER_EACH_MAIN_TASK_FOR_PREMIUM = 7;
        public static int MAX_NUMBER_GROUP_FOR_PREMIUM = 30;
    }
}
