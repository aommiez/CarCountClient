using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Timers;
using System.Net;
using System.IO;
using System.Reflection;

namespace CarCountClient
{
    class Program
    {

        protected string api = "https://"+ ConfigurationManager.AppSettings["sv_host"] +"/";
        protected string options = "?order=desc&sort=name&site=stackoverflow";

        static void Main(string[] args)
        {
            
            Console.WriteLine("Hello is Me !\n\n ");
            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.WriteLine(@"                                                                                         ");
            Console.WriteLine(@"  _______ _    _ ______ ______ _______ ________   __   _____ _____   ____  _    _ _____  ");
            Console.WriteLine(@" |__   __| |  | |  ____|  ____|__   __|  ____\ \ / /  / ____|  __ \ / __ \| |  | |  __ \ ");
            Console.WriteLine(@"    | |  | |  | | |__  | |__     | |  | |__   \ V /  | |  __| |__) | |  | | |  | | |__) |");
            Console.WriteLine(@"    | |  | |  | |  __| |  __|    | |  |  __|   > <   | | |_ |  _  /| |  | | |  | |  ___/ ");
            Console.WriteLine(@"    | |  | |__| | |    | |       | |  | |____ / . \  | |__| | | \ \| |__| | |__| | |     ");
            Console.WriteLine(@"    |_|   \____/|_|    |_|       |_|  |______/_/ \_\  \_____|_|  \_\\____/ \____/|_|     ");
            Console.WriteLine(@"                                                                                         ");
            Console.WriteLine(@"                                                                                         ");
            Console.ResetColor();
            Console.WriteLine(@"CarPark Software Version : "+ Assembly.GetExecutingAssembly().GetName().Version);
            Console.WriteLine(@"                                                                                         ");
            Console.WriteLine(@"DB Host : " + ConfigurationManager.AppSettings["db_host"]);
            Console.WriteLine(@"DB Name : " + ConfigurationManager.AppSettings["db_name"]);
            Console.WriteLine(@"DB Username : " + ConfigurationManager.AppSettings["db_username"]);
            Console.WriteLine(@"DB Password : " + ConfigurationManager.AppSettings["db_password"]);
            Console.WriteLine(@"DB Table : " + ConfigurationManager.AppSettings["db_table"]);
            Console.WriteLine(@"                                                                                         ");
            Console.WriteLine(@"Server Host : " + ConfigurationManager.AppSettings["sv_host"]);
            Console.WriteLine(@"Client Id : " + ConfigurationManager.AppSettings["clientId"]);
            Console.WriteLine(@"Debug : " + ConfigurationManager.AppSettings["debug"]);
            Console.WriteLine(@"                                                                                         ");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(@"Status : Running");
            Console.ResetColor();
            //Console.WriteLine("Hello is Me !");

            Timer t = new Timer(1000); // 1 sec = 1000, 60 sec = 60000
            t.AutoReset = true;
            t.Elapsed += new System.Timers.ElapsedEventHandler(t_Elapsed);
            t.Start();

       

            Console.WriteLine("\n\n Press any key to exit.");
            Console.ReadKey();
        }
        private static void t_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            sendCount();
        }

        static public void sendCount()
        {
            MySql.Data.MySqlClient.MySqlConnection conn;
            string myConnectionString;
            string commandLine = "SELECT COUNT(*) FROM " + ConfigurationManager.AppSettings["db_table"];

            string db_host = ConfigurationManager.AppSettings["db_host"];
            string db_username = ConfigurationManager.AppSettings["db_username"];
            string db_password = ConfigurationManager.AppSettings["db_password"];
            string db_name = ConfigurationManager.AppSettings["db_name"];

            myConnectionString = "server=" + db_host + ";uid=" + db_username + ";" +
                "pwd=" + db_password + ";database=" + db_name + ";";

            try
            {
                conn = new MySql.Data.MySqlClient.MySqlConnection();
                conn.ConnectionString = myConnectionString;
                conn.Open();
                MySql.Data.MySqlClient.MySqlCommand cmd = new MySql.Data.MySqlClient.MySqlCommand(commandLine, conn);
                int x = Convert.ToInt32(cmd.ExecuteScalar());
                

                string html = string.Empty;
                string url = @"http://"+ ConfigurationManager.AppSettings["sv_host"]+ "/update?serverId="+ ConfigurationManager.AppSettings["clientId"] + "&count="+ x;

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.AutomaticDecompression = DecompressionMethods.GZip;

                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                using (Stream stream = response.GetResponseStream())
                using (StreamReader reader = new StreamReader(stream))
                {
                    html = reader.ReadToEnd();
                }

                //Console.WriteLine(html);
                if (ConfigurationManager.AppSettings["debug"] == "true")
                {
                    Console.WriteLine(DateTime.Now.ToString("HH:mm:ss", System.Globalization.DateTimeFormatInfo.InvariantInfo) + " -> Count : " + x + " " + url);
                } else
                {
                    //Console.WriteLine(DateTime.Now.ToString("HH:mm:ss", System.Globalization.DateTimeFormatInfo.InvariantInfo) + " -> Count : " + x);
                }
                
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        static public void Tick(Object stateInfo)
        {
            Console.WriteLine("Tick: {0}", DateTime.Now.ToString("h:mm:ss"));
        }

    }
}
