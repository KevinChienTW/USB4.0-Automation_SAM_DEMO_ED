using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace USB4._0_Automation
{
    static class Program
    {

        public static int localtime;
        public static int nettime;
        public static DateTime nowtime;

        public static string test_type;
        public static string test_item;

        public static string DUT_lenght;

        //public static string BST_max;    //MAX Linit line
        //public static string EST_max;      //MAX Linit line
        //public static string BR_max;   //MAX Linit line
        //public static string ER_max;     //MAX Linit line

        //public static string BST_min;    //Min Linit line
        //public static string EST_min;      //Min Linit line
        //public static string BR_min;   //Min Linit line
        //public static string ER_min;     //Min Linit line
        /// <summary>
        /// 應用程式的主要進入點。
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Main());
        }
    }
}
