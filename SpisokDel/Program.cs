using System;
using MySql.Data.MySqlClient;
using System.Data;
using System.Collections.Generic;
using MySql.Data.Types;

namespace SpisokDel
{
    class Program
    {
        static SortedList<int, DateTime> IDsInDB = new SortedList<int, DateTime>();
        private static string server = "127.0.0.1",user = "root",db = "dela",port = "3306",passwd = "";
        private static string connStr = "server=" + server + ";user=" + user + ";database=" + db + ";port=" + port + ";password=" + passwd + ";";

        static void Main(string[] args)
        {
            try { 
            Console.Title = "Планировщик дел - Уровень 3 - сохранение в базу данных";
            while(true) Menu();
            }catch(Exception e)
                {
                    Console.BackgroundColor = ConsoleColor.Red;
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine("ERROR!!!"
                        +"\n"+e.Message+"\n"+e.Source+"\n"+e.StackTrace
                        +"\n Программа будет закрыта.");
                    Console.Read();
                    Environment.Exit(0);
                }
        }
        
        protected static void Menu()
        {
            try
            {
                Console.WriteLine("Введите номер желаемого действия:"
                    + "\n 1) Просмотреть список;"
                    + "\n 2) Добавить дело в список;"
                    + "\n 3) Изменить запись;"
                    + "\n 4) Вычеркнуть дело из списка;"
                    + "\n 5) Очистить весь список;"
                    + "\n 0) Закрыть программу.");


                string R = Console.ReadLine();
                switch (R)
                {
                    case "0": Environment.Exit(0); break;
                    case "1": ShowList(); break;
                    case "2": AddPoint(); break;
                    case "3": UpdatePoint(); break;
                    case "4": DeletePoint(); break;
                    case "5": DeleteAll(); break;
                    default:
                        Console.Clear();
                        Console.BackgroundColor = ConsoleColor.DarkRed;
                        Console.ForegroundColor = ConsoleColor.White;
                        Console.WriteLine("Вариант отсутствует, либо введённые данные не распознаны."
                        + "\nПожалуйста, введите только одну цифру желаемого варианта действий.");
                        Console.ResetColor(); break;
                }
            }catch(Exception e)
            {
                Console.BackgroundColor = ConsoleColor.Red;
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine("ERROR!!!"
                    +"\n"+e.Message+"\n"+e.Source+"\n"+e.StackTrace
                    +"\n Программа будет закрыта.");
                Console.Read();
                Environment.Exit(0);
            }
        }

        protected static void AddPoint()
        {
            try
            {
                string NewPoint = "";
                Console.Write("Введите текст, который хотите записать: ");
                NewPoint = Console.ReadLine();
                NewPoint = Process(NewPoint);
                Console.Write("Дата, к которой это должно быть завершено(В формате \"ДД/ММ/ГГГГ\"): ");
                DateTime DT = Convert.ToDateTime(Console.ReadLine());
                string q = "INSERT INTO `todolist` VALUES ('" 
                    + DateTime.Now.Year + "-" + DateTime.Now.Month + "-" + DateTime.Now.Day + " " + DateTime.Now.TimeOfDay + "','" 
                    + NewPoint + "','" 
                    + DT.Year+"-"+ DT.Month+"-"+ DT.Day+"');";
                using (MySqlConnection con = new MySqlConnection(connStr))
                {
                    con.Open();
                    MySqlCommand com = new MySqlCommand(q, con);
                    com.ExecuteNonQuery();
                    con.Close();
                }
                Console.Clear();
                Console.WriteLine("Успешно записано!");
            }catch(Exception e)
            {
                Console.BackgroundColor = ConsoleColor.Red;
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine("ERROR!!!"
                    +"\n"+e.Message+"\n"+e.Source+"\n"+e.StackTrace
                    +"\n Программа будет закрыта.");
                Console.Read();
                Environment.Exit(0);
            }
}
        protected static void UpdatePoint()
        {
            try { 
            MySqlDateTime DoBefo;
            
            ShowList();
                Console.WriteLine("Укажите номер пункта, который нужно изменить:");
                string N = Console.ReadLine();
            int n = 0;
            if (int.TryParse(N, out n))
            {
                DateTime V;
                IDsInDB.TryGetValue(n, out V);
                Console.WriteLine("Выберите, что именно нужно изменить:"
                    + "1) Текст и дату окончания"
                    + "2) Только текст"
                    + "3) Только дату окончания");
                int T = int.Parse(Console.ReadLine());
                string text = "", BefDate = "", q="";
                
                switch (T)
                {
                    case 1:
                        Console.WriteLine("Введите отредактированный текст записи:");
                        text = Console.ReadLine();
                            text = Process(text);
                        Console.WriteLine("Введите новую дату окончания (В формате \"ДД/ММ/ГГГГ\"):");
                        BefDate = Console.ReadLine();
                        DoBefo = new MySqlDateTime(Convert.ToDateTime(BefDate));
                        q = "UPDATE `todolist` SET `text`='" + text + "', `dobefore`='" + DoBefo + "' WHERE `added`='"  +V.Year + "-" + V.Month + "-" + V.Day + " " + V.TimeOfDay + "';";
                        break;
                    case 2:
                        Console.WriteLine("Введите отредактированный текст записи:");
                        text = Console.ReadLine();
                            text = Process(text);
                            q = "UPDATE `todolist` SET `text`='" + text + "' WHERE `added`='" + +V.Year + "-" + V.Month + "-" + V.Day + " " + V.TimeOfDay + "';";
                        break;
                    case 3:
                        Console.WriteLine("Введите новую дату окончания (В формате \"ДД/ММ/ГГГГ\"):");
                        BefDate = Console.ReadLine();
                        DoBefo = new MySqlDateTime(Convert.ToDateTime(BefDate));
                        q = "UPDATE `todolist` SET `dobefore`='" + DoBefo + "' WHERE 'added'='" + V.Year + "-" + V.Month + "-" + V.Day + " " + V.TimeOfDay + "';";
                        break;
                    default: Console.Clear(); Console.WriteLine("Указан некорректный номер записи! Возврат в меню..."); break;
                }
                
                using (MySqlConnection con = new MySqlConnection(connStr))
                {
                    con.Open();
                    MySqlCommand com = new MySqlCommand(q, con);
                    com.ExecuteNonQuery();
                    con.Close();
                    Console.Clear();
                }
                Console.WriteLine("Запись №" + n + " успешно обновлена!");
            }
            else { Console.Clear(); Console.WriteLine("Указан некорректный номер записи! Возврат в меню..."); }
            }
            catch (Exception e)
            {
                Console.BackgroundColor = ConsoleColor.Red;
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine("ERROR!!!"
                    + "\n" + e.Message + "\n" + e.Source + "\n" + e.StackTrace
                    + "\n Программа будет закрыта.");
                Console.Read();
                Environment.Exit(0);
            }
        }
        protected static void DeletePoint()
        {
            try { 
            
            ShowList();
            Console.WriteLine("Укажите номер пункта, который нужно удалить:");
            string N = Console.ReadLine();
            int n = 0;
            if (int.TryParse(N, out n))
            {
                DateTime V;
                IDsInDB.TryGetValue(n, out V);
                string q = "DELETE FROM `todolist` WHERE `added`='"+V.Year + "-" + V.Month + "-" + V.Day +" "+V.TimeOfDay+ "';";
                using (MySqlConnection con = new MySqlConnection(connStr))
                {
                    con.Open();
                    MySqlCommand com = new MySqlCommand(q, con);
                    com.ExecuteNonQuery();
                    con.Close();
                    Console.Clear();
                }
                Console.WriteLine("Запись №" + n + " успешно удалена!");
            }
            else Console.WriteLine("Указан некорректный номер записи! Возврат в меню...");
            }
            catch (Exception e)
            {
                Console.BackgroundColor = ConsoleColor.Red;
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine("ERROR!!!"
                    + "\n" + e.Message + "\n" + e.Source + "\n" + e.StackTrace
                    + "\n Программа будет закрыта.");
                Console.Read();
                Environment.Exit(0);
            }
        }
        protected static void ShowList()
        {
            try { 
            Console.Clear();
            Console.WriteLine("Текущий список дел:");
            string q = "SELECT * FROM `todolist`;";
            using (MySqlConnection con = new MySqlConnection(connStr))
            {
                con.Open();
                MySqlCommand com = new MySqlCommand(q, con);
                MySqlDataReader DA = com.ExecuteReader();
                int K = 0;
                IDsInDB.Clear();
                while (DA.Read())
                {
                    K++;
                    DateTime Added = DA.GetDateTime(0);
                    string Text = DA.GetString(1);
                    DateTime DoBefore = DA.GetDateTime(2);
                    Console.WriteLine(K+") "+Text+" Добавлено:"+Added+"| Сделать до:"+DoBefore);
                    IDsInDB.Add(K, Added);
                }
            }
            }
            catch (Exception e)
            {
                Console.BackgroundColor = ConsoleColor.Red;
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine("ERROR!!!"
                    + "\n" + e.Message + "\n" + e.Source + "\n" + e.StackTrace
                    + "\n Программа будет закрыта.");
                Console.Read();
                Environment.Exit(0);
            }
        }
        protected static void DeleteAll()
        {
            try { 
            string q = "DELETE FROM `todolist` ;";
            using (MySqlConnection con = new MySqlConnection(connStr))
            {
                con.Open();
                MySqlCommand com = new MySqlCommand(q, con);
                com.ExecuteNonQuery();
                con.Close();
            }
            Console.Clear();
            Console.WriteLine("Список успешно очищен! Возврат в меню...");
            }
            catch (Exception e)
            {
                Console.BackgroundColor = ConsoleColor.Red;
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine("ERROR!!!"
                    + "\n" + e.Message + "\n" + e.Source + "\n" + e.StackTrace
                    + "\n Программа будет закрыта.");
                Console.Read();
                Environment.Exit(0);
            }
        }

        protected static string Process(string S)
        {
            string result = "";
            while(S.Contains(@"`")) S=S.Remove(S.IndexOf('`'), 1);
            while (S.Contains(@"'")) S=S.Remove(S.IndexOf('\''), 1);
            while (S.Contains("\"")) S=S.Remove(S.IndexOf('\"'), 1);
            result = S;
            return result;
        }
    }
}
