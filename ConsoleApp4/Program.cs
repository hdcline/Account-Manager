using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Data.Common;
using System.Data.SqlClient;
using System.Data;

namespace ConsoleApp4
{
    class Program
    {
        
        static string alphabet = "abcdefghijklmnopqrstuvwxyz";
        static string alphabetCap = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        static int[] arrInt = { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };
        //used for encryption and add to any numerical values in password
        static int shift = 5;
        static void Main(string[] args)
        {
            string provider = ConfigurationManager.AppSettings["provider"];

            string connectionString = ConfigurationManager.AppSettings["connectionString"];

            DbProviderFactory factory = DbProviderFactories.GetFactory(provider);

            using (DbConnection connection = factory.CreateConnection())
            {
                if (connection == null)
                {
                    Console.WriteLine("Connection Error");
                    Console.ReadLine();
                    return;
                }

                connection.ConnectionString = connectionString;

                connection.Open();

                DbCommand command = factory.CreateCommand();

                if (command == null)
                {
                    Console.WriteLine("Command Error");
                    Console.ReadLine();
                    return;
                }

                command.Connection = connection;

                

                Console.Write("Enter action('get info', 'insert', 'update', 'delete', or 'exit'): ");
                //Console.WriteLine(Convert.ToString(ShiftAlphabetCap(shift)));

                bool newAction = true;

                string action = Console.ReadLine();

                while (newAction == true)
                {
                    //case that user enters 'get info'
                    if (action.ToLower() == "get info")
                    {
                        Console.Write("Enter account: ");

                        string getInfo = Console.ReadLine();

                        bool keepGoing = true;
                        int count = 0;

                        while (keepGoing)
                        {
                            command.CommandText = "Select * from Passwords where account = '" + getInfo + "'";
                            using (DbDataReader dataReader = command.ExecuteReader())
                            {
                                while (dataReader.Read())
                                {
                                    string password = Decrypt(dataReader["password"].ToString());

                                    Console.WriteLine();
                                    Console.WriteLine($"Username: {dataReader["username"]}\n"
                                        + $"Password: {password}");
                                    Console.WriteLine();
                                    count++;
                                }
                                string yesOrNo;
                                if (count <= 0)
                                {
                                    Console.WriteLine();
                                    Console.Write("Account does not exist. Do you want to try again? (yes/no): ");
                                    yesOrNo = Console.ReadLine();

                                    while (yesOrNo.ToLower() != "yes" && yesOrNo.ToLower() != "no")
                                    {
                                        Console.Write("You must enter either yes or no.\n" +
                                            "Do you want to try again? (yes/no): ");
                                        yesOrNo = Console.ReadLine();
                                    }

                                    if (yesOrNo.ToLower() == "yes")
                                    {
                                        Console.WriteLine();
                                        Console.Write("Enter a different account name: ");
                                        getInfo = Console.ReadLine();
                                    }
                                    else if (yesOrNo.ToLower() == "no")
                                    {
                                        keepGoing = false;

                                        Console.Write("Do you wish to perform another command? (yes/no): ");
                                        string yesOrNo1 = Console.ReadLine();

                                        while (yesOrNo1.ToLower() != "yes" && yesOrNo1.ToLower() != "no")
                                        {
                                            Console.Write("You must enter either yes or no.\n" +
                                                "Do you want to try again? (yes/no): ");
                                            yesOrNo1 = Console.ReadLine();
                                        }

                                        if (yesOrNo1.ToLower() == "yes")
                                        {
                                            Console.WriteLine();
                                            Console.Write("Enter action('get info', 'insert', 'update', 'delete', or 'exit'): ");
                                            action = Console.ReadLine();
                                        }

                                        else if (yesOrNo1.ToLower() == "no")
                                        {
                                            Console.WriteLine();
                                            Console.WriteLine("Thank you");
                                            newAction = false;
                                        }
                                    }
                                }

                                else
                                {
                                    
                                    keepGoing = false;
                                    Console.Write("Do you wish to perform another command? (yes/no): ");
                                    yesOrNo = Console.ReadLine();

                                    while (yesOrNo.ToLower() != "yes" && yesOrNo.ToLower() != "no")
                                    {
                                        Console.Write("You must enter either yes or no.\n" +
                                            "Do you want to try again? (yes/no): ");
                                        yesOrNo = Console.ReadLine();
                                    }

                                    if (yesOrNo.ToLower() == "yes")
                                    {
                                        Console.WriteLine();
                                        Console.Write("Enter action('get info', 'insert', 'update', 'delete', or 'exit'): ");
                                        action = Console.ReadLine();
                                    }

                                    else if (yesOrNo.ToLower() == "no")
                                    {
                                        Console.WriteLine();
                                        Console.WriteLine("Thank you");
                                        newAction = false;
                                    }
                                }
                            }
                        }
                    }

                    //case that user enters 'insert'
                    else if (action.ToLower() == "insert")
                    {
                        using (SqlConnection cnn = new SqlConnection(ConfigurationManager.AppSettings["connectionString"]))
                        {
                            using (SqlCommand command1 = new SqlCommand())
                            {
                                command1.Connection = cnn;
                                command.CommandType = CommandType.Text;

                                Console.WriteLine();
                                Console.Write("Enter account: ");
                                string account = Console.ReadLine();
                                while (account == "" || account == " ")
                                {
                                    Console.WriteLine();
                                    Console.Write("You must enter a value for the account name\nPlease try again: ");
                                    account = Console.ReadLine();
                                }

                                Console.WriteLine();
                                Console.Write("Enter username: ");
                                string username = Console.ReadLine();
                                while (username == "" || username == " ")
                                {
                                    Console.WriteLine();
                                    Console.Write("You must enter a value for the username\nPlease try again: ");
                                    username = Console.ReadLine();
                                }

                                Console.WriteLine();
                                Console.Write("Enter password: ");
                                string password = Console.ReadLine();
                                while (password == "" || password == " ")
                                {
                                    Console.WriteLine();
                                    Console.Write("You must enter a value for the password\nPlease try again: ");
                                    password = Console.ReadLine();
                                }

                                //count++;
                                command1.CommandText = "Insert into Passwords (account, username, password) " +
                                    "values ('" + account + "', '" + username + "', '" + Encrypt(password) + "')";

                                cnn.Open();
                                command1.ExecuteNonQuery();
                                cnn.Close();
                                Console.WriteLine();
                                Console.WriteLine("The account " + account + " has been inserted into the database.");
                                Console.WriteLine();

                                Console.Write("Do you wish to perform another command? (yes/no): ");
                                string yesOrNo = Console.ReadLine();

                                while (yesOrNo.ToLower() != "yes" && yesOrNo.ToLower() != "no")
                                {
                                    Console.Write("You must enter either yes or no.\n" +
                                        "Do you want to try again? (yes/no): ");
                                    yesOrNo = Console.ReadLine();
                                }

                                if (yesOrNo.ToLower() == "yes")
                                {
                                    Console.WriteLine();
                                    Console.Write("Enter action('get info', 'insert', 'update', 'delete', or 'exit'): ");
                                    action = Console.ReadLine();
                                }

                                else if (yesOrNo.ToLower() == "no")
                                {
                                    Console.WriteLine();
                                    Console.WriteLine("Thank you");
                                    newAction = false;
                                }
                            }
                        }
                    }

                    //case that user enters 'update'
                    else if (action.ToLower() == "update")
                    {
                        Console.Write("Enter account to be updated: ");

                        string account = Console.ReadLine();

                        bool keepGoing = true;
                        int count = 0;
                        string sql;
                        SqlConnection cnn;
                        cnn = new SqlConnection(connectionString);
                        string uOrP;

                        while (keepGoing)
                        {

                            command.CommandText = "Select * from Passwords where account = '" + account + "'";
                            using (DbDataReader dataReader = command.ExecuteReader())
                            {
                                while (dataReader.Read())
                                {
                                    count++;
                                    Console.WriteLine();
                                    Console.Write("Do you want to update the 'username' or 'password' of " + account + "? ");
                                    uOrP = Console.ReadLine();

                                    while (uOrP.ToLower() != "username" && uOrP.ToLower() != "password")
                                    {
                                        //cnn.Close();
                                        Console.WriteLine();
                                        Console.Write("You must enter 'username' or 'password'.\n" +
                                            "Please try again: ");
                                        uOrP = Console.ReadLine();
                                    }

                                    if (uOrP.ToLower() == "username")
                                    {
                                        cnn.Open();
                                        Console.WriteLine();
                                        Console.Write("Enter the new username: ");
                                        string username = Console.ReadLine();

                                        while (username == "" || username == " ")
                                        {
                                            Console.WriteLine();
                                            Console.Write("You must enter a value for the username\nPlease try again: ");
                                            username = Console.ReadLine();
                                        }

                                        SqlCommand command2;
                                        SqlDataAdapter adapter = new SqlDataAdapter();

                                        sql = "Update Passwords set username = '" + username + "' where account = '" + account + "'";

                                        command2 = new SqlCommand(sql, cnn);

                                        adapter.UpdateCommand = new SqlCommand(sql, cnn);
                                        adapter.UpdateCommand.ExecuteNonQuery();

                                        command2.Dispose();
                                        cnn.Close();

                                        Console.WriteLine();
                                        Console.WriteLine("The username for account " + account + " has been updated.");
                                        keepGoing = false;

                                        Console.WriteLine();
                                        Console.Write("Do you wish to perform another command? (yes/no): ");
                                        string yesOrNo1 = Console.ReadLine();

                                        while (yesOrNo1.ToLower() != "yes" && yesOrNo1.ToLower() != "no")
                                        {
                                            Console.WriteLine();
                                            Console.Write("You must enter either yes or no.\n" +
                                                "Do you want to try again? (yes/no): ");
                                            yesOrNo1 = Console.ReadLine();
                                        }

                                        if (yesOrNo1.ToLower() == "yes")
                                        {
                                            Console.WriteLine();
                                            Console.Write("Enter action('get info', 'insert', 'update', 'delete', or 'exit'): ");
                                            action = Console.ReadLine();
                                        }

                                        else if (yesOrNo1.ToLower() == "no")
                                        {
                                            Console.WriteLine();
                                            Console.WriteLine("Thank you");
                                            newAction = false;
                                        }
                                    }

                                    else if (uOrP.ToLower() == "password")
                                    {
                                        cnn.Open();
                                        Console.WriteLine();
                                        Console.Write("Enter the new password: ");
                                        string password = Console.ReadLine();

                                        while (password == "" || password == " ")
                                        {
                                            Console.WriteLine();
                                            Console.Write("You must enter a value for the password\nPlease try again: ");
                                            password = Console.ReadLine();
                                        }

                                        SqlCommand command2;
                                        SqlDataAdapter adapter = new SqlDataAdapter();

                                        sql = "Update Passwords set password = '" + Encrypt(password) + "' where account = '" + account + "'";

                                        command2 = new SqlCommand(sql, cnn);

                                        adapter.UpdateCommand = new SqlCommand(sql, cnn);
                                        adapter.UpdateCommand.ExecuteNonQuery();

                                        command2.Dispose();
                                        cnn.Close();

                                        Console.WriteLine();
                                        Console.WriteLine("The password for account " + account + " has been updated.");
                                        keepGoing = false;

                                        Console.WriteLine();
                                        Console.Write("Do you wish to perform another command? (yes/no): ");
                                        string yesOrNo1 = Console.ReadLine();

                                        while (yesOrNo1.ToLower() != "yes" && yesOrNo1.ToLower() != "no")
                                        {
                                            Console.Write("You must enter either yes or no.\n" +
                                                "Do you want to try again? (yes/no): ");
                                            yesOrNo1 = Console.ReadLine();
                                        }

                                        if (yesOrNo1.ToLower() == "yes")
                                        {
                                            Console.WriteLine();
                                            Console.Write("Enter action('get info', 'insert', 'update', 'delete', or 'exit'): ");
                                            action = Console.ReadLine();
                                        }

                                        else if (yesOrNo1.ToLower() == "no")
                                        {
                                            Console.WriteLine();
                                            Console.WriteLine("Thank you");
                                            newAction = false;
                                        }
                                    }
                                }

                                string yesOrNo;
                                if (count <= 0)
                                {
                                    cnn.Close();
                                    Console.WriteLine();
                                    Console.Write("Account does not exist. Do you want to try again? (yes/no): ");
                                    yesOrNo = Console.ReadLine();

                                    while (yesOrNo.ToLower() != "yes" && yesOrNo.ToLower() != "no")
                                    {
                                        Console.Write("You must enter either yes or no.\n" +
                                            "Do you want to try again? (yes/no): ");
                                        yesOrNo = Console.ReadLine();
                                    }

                                    if (yesOrNo.ToLower() == "yes")
                                    {
                                        Console.Write("Enter a different account name: ");
                                        account = Console.ReadLine();
                                    }

                                    else if (yesOrNo.ToLower() == "no")
                                    {
                                        Console.WriteLine();
                                        Console.Write("Do you wish to perform another command? (yes/no): ");
                                        string yesOrNo1 = Console.ReadLine();

                                        while (yesOrNo1.ToLower() != "yes" && yesOrNo1.ToLower() != "no")
                                        {
                                            Console.Write("You must enter either yes or no.\n" +
                                                "Do you want to try again? (yes/no): ");
                                            yesOrNo1 = Console.ReadLine();
                                        }

                                        if (yesOrNo1.ToLower() == "yes")
                                        {
                                            keepGoing = false;
                                            Console.WriteLine();
                                            Console.Write("Enter action('get info', 'insert', 'update', 'delete', or 'exit'): ");
                                            action = Console.ReadLine();
                                        }

                                        else if (yesOrNo1.ToLower() == "no")
                                        {
                                            Console.WriteLine();
                                            Console.WriteLine("Thank you");
                                            newAction = false;
                                        }
                                    }
                                }
                            }
                        }
                    }

                    //case that user enters 'delete'
                    else if (action.ToLower() == "delete")
                    {
                        Console.Write("Enter the name of the account that you wish to delete: ");
                        string account = Console.ReadLine();

                        bool keepGoing = true;
                        int count = 0;
                        string sql;
                        SqlConnection cnn;
                        cnn = new SqlConnection(connectionString);
                        string yOrN;

                        while (keepGoing)
                        {

                            command.CommandText = "Select * from Passwords where account = '" + account + "'";
                            using (DbDataReader dataReader = command.ExecuteReader())
                            {
                                while (dataReader.Read())
                                {
                                    count++;
                                    Console.WriteLine();
                                    Console.Write("Are you sure you wish to delete " + account + " from the database? (yes/no): ");
                                    yOrN = Console.ReadLine();

                                    while (yOrN.ToLower() != "yes" && yOrN.ToLower() != "no")
                                    {
                                        //cnn.Close();
                                        Console.WriteLine();
                                        Console.Write("You must enter yes or no.\n" +
                                            "Please try again: ");
                                        yOrN = Console.ReadLine();
                                    }

                                    if (yOrN.ToLower() == "yes")
                                    {
                                        cnn.Open();
                                        SqlCommand command2;
                                        SqlDataAdapter adapter = new SqlDataAdapter();

                                        sql = "Delete Passwords where account = '" + account + "'";

                                        command2 = new SqlCommand(sql, cnn);

                                        adapter.DeleteCommand = new SqlCommand(sql, cnn);
                                        adapter.DeleteCommand.ExecuteNonQuery();

                                        command2.Dispose();
                                        cnn.Close();

                                        Console.WriteLine();
                                        Console.WriteLine("The account named " + account + " has been deleted.");
                                        keepGoing = false;

                                        Console.WriteLine();
                                        Console.Write("Do you wish to perform another command? (yes/no): ");
                                        string yesOrNo1 = Console.ReadLine();

                                        while (yesOrNo1.ToLower() != "yes" && yesOrNo1.ToLower() != "no")
                                        {
                                            Console.Write("You must enter either yes or no.\n" +
                                                "Do you want to try again? (yes/no): ");
                                            yesOrNo1 = Console.ReadLine();
                                        }

                                        if (yesOrNo1.ToLower() == "yes")
                                        {
                                            keepGoing = false;
                                            Console.WriteLine();
                                            Console.Write("Enter action('get info', 'insert', 'update', 'delete', or 'exit'): ");
                                            action = Console.ReadLine();
                                        }

                                        else if (yesOrNo1.ToLower() == "no")
                                        {
                                            Console.WriteLine();
                                            Console.WriteLine("Thank you");
                                            newAction = false;
                                        }
                                    }

                                    else if (yOrN.ToLower() == "no")
                                    {
                                        keepGoing = false;

                                        Console.WriteLine();
                                        Console.Write("Do you wish to perform another command? (yes/no): ");
                                        string yesOrNo1 = Console.ReadLine();

                                        while (yesOrNo1.ToLower() != "yes" && yesOrNo1.ToLower() != "no")
                                        {
                                            Console.Write("You must enter either yes or no.\n" +
                                                "Do you want to try again? (yes/no): ");
                                            yesOrNo1 = Console.ReadLine();
                                        }

                                        if (yesOrNo1.ToLower() == "yes")
                                        {
                                            keepGoing = false;
                                            Console.WriteLine();
                                            Console.Write("Enter action('get info', 'insert', 'update', 'delete', or 'exit'): ");
                                            action = Console.ReadLine();
                                        }

                                        else if (yesOrNo1.ToLower() == "no")
                                        {
                                            Console.WriteLine();
                                            Console.WriteLine("Thank you");
                                            newAction = false;
                                        }
                                    }
                                }

                                string yesOrNo2;
                                if (count <= 0)
                                {
                                    cnn.Close();
                                    Console.WriteLine();
                                    Console.Write("Account does not exist. Do you want to try again? (yes/no): ");
                                    yesOrNo2 = Console.ReadLine();

                                    while (yesOrNo2.ToLower() != "yes" && yesOrNo2.ToLower() != "no")
                                    {
                                        Console.WriteLine();
                                        Console.Write("You must enter either yes or no.\n" +
                                            "Do you want to try again? (yes/no): ");
                                        yesOrNo2 = Console.ReadLine();
                                    }

                                    if (yesOrNo2.ToLower() == "yes")
                                    {
                                        Console.WriteLine();
                                        Console.Write("Enter a different account name: ");
                                        account = Console.ReadLine();
                                    }
                                    else if (yesOrNo2.ToLower() == "no")
                                    {
                                        keepGoing = false;
                                        Console.WriteLine();
                                        Console.Write("Do you wish to perform another command? (yes/no): ");
                                        string yesOrNo1 = Console.ReadLine();

                                        while (yesOrNo1.ToLower() != "yes" && yesOrNo1.ToLower() != "no")
                                        {
                                            Console.WriteLine();
                                            Console.Write("You must enter either yes or no.\n" +
                                                "Do you want to try again? (yes/no): ");
                                            yesOrNo1 = Console.ReadLine();
                                        }

                                        if (yesOrNo1.ToLower() == "yes")
                                        {
                                            keepGoing = false;
                                            Console.WriteLine();
                                            Console.Write("Enter action('get info', 'insert', 'update', 'delete', or 'exit'): ");
                                            action = Console.ReadLine();
                                        }

                                        else if (yesOrNo1.ToLower() == "no")
                                        {
                                            Console.WriteLine();
                                            Console.WriteLine("Thank you");
                                            newAction = false;
                                        }
                                    }
                                }
                            }
                        }
                    }

                    //case that user enters 'exit'
                    else if (action.ToLower() == "exit")
                    {
                        newAction = false;
                        Console.WriteLine();
                        Console.WriteLine("Thank you.");
                    }

                    else
                    {
                        Console.Write("You must enter 'get info', 'insert', 'update', 'delete', or 'exit'\n" +
                            "Do you wish to enter a new command? (yes/no): ");
                        string yesOrNo = Console.ReadLine();

                        while (yesOrNo.ToLower() != "yes" && yesOrNo.ToLower() != "no")
                        {
                            Console.Write("You must enter either yes or no.\n" +
                                "Do you want to try again? (yes/no): ");
                            yesOrNo = Console.ReadLine();
                        }

                        if (yesOrNo.ToLower() == "yes")
                        {
                            Console.Write("Enter a new command ('get info', 'insert', 'update', 'delete', or 'exit'): ");
                            action = Console.ReadLine();
                        }

                        else if (yesOrNo.ToLower() == "no")
                        {
                            Console.WriteLine();
                            Console.WriteLine("Thank you.");
                            //keepGoing = false;
                            newAction = false;
                        }
                    }
                }
            }
        }


        public static string Encrypt(string plaintxt)
        {
            string ciphertxt = "";
            for (int i = 0; i < plaintxt.Length; i++)
            {
                char[] chArr = plaintxt.ToCharArray();
                if (Char.IsNumber(chArr[i]))
                {
                    string integers = arrInt.ToString();
                    int w = Array.IndexOf(arrInt, chArr[i]);
                    //int w = integers.IndexOf(chArr[i]);
                    int r = ShiftNum(shift)[w];
                    char ch = Convert.ToChar(r);
                    ciphertxt = ciphertxt + ch;

                }
                else if (Char.IsUpper(chArr[i]))
                {
                    int w = alphabetCap.IndexOf(chArr[i]);
                    char t = ShiftAlphabetCap(shift)[w];
                    char ch = t;
                    ciphertxt = ciphertxt + ch;
                }
                else if (Char.IsLower(chArr[i]))
                {
                    int w = alphabet.IndexOf(chArr[i]);
                    char t = ShiftAlphabet(shift)[w];
                    char ch = t;
                    ciphertxt = ciphertxt + ch;
                }
                else
                {
                    ciphertxt = ciphertxt + chArr[i];
                }
            }
            return ciphertxt;
        }


        public static string Decrypt(string ciphertxt)
        {
            string shiftAlphaCap = ShiftAlphabetCap(shift).ToString();
            string shiftAlpha = ShiftAlphabet(shift).ToString();
            char[] alpha = alphabet.ToCharArray();
            char[] alphaCap = alphabetCap.ToCharArray();
            

            string plaintxt = "";

            for (int i = 0; i < ciphertxt.Length; i++)
            {
                //char[] chArr = ciphertxt.ToCharArray();
                if (Char.IsNumber(ciphertxt[i]))
                {
                   int w = Array.IndexOf(ShiftNum(shift), ciphertxt[i]);
                    int t = arrInt[w];
                    char ch = Convert.ToChar(t);
                    plaintxt = plaintxt + ch;
                }
                else if (Char.IsUpper(ciphertxt[i]))
                {
                    int w = Array.IndexOf(ShiftAlphabetCap(shift), ciphertxt[i]);
                    char t = alphaCap[w];
                    char ch = t;
                    plaintxt = plaintxt + ch;
                }
                else if (Char.IsLower(ciphertxt[i]))
                {
                    int w = Array.IndexOf(ShiftAlphabet(shift), ciphertxt[i]);
                    char t = alpha[w];
                    char ch = t;
                    plaintxt = plaintxt + ch;
                }
                else
                {
                    plaintxt = plaintxt + ciphertxt[i];
                }
            }
            return plaintxt;
        }


        public static int[] ShiftNum(int n)
        {
            int[] shiftArr = new int[10];

            for (int i = 0; i < shiftArr.Length; i++)
            {
                shiftArr[i] = arrInt[(i + n) % 10];
            }
            return shiftArr;
        }


        public static char[] ShiftAlphabet(int n)
        {
            char[] shiftAlpha = new char[26];
            //char[] shiftAlphaCap = new char[26];
            char[] alpha = alphabet.ToCharArray();
            //char[] alphaCap = alphabetCap.ToCharArray();

            for (int i = 0; i < alphabet.Length; i++)
            {
                shiftAlpha[i] = alpha[(i + n) % 26];
                //shiftAlphaCap[i] = alphaCap[(i + n) % 26];
            }
            return shiftAlpha;
        }


        public static char[] ShiftAlphabetCap(int n)
        {
            char[] shiftAlpha = new char[26];
            char[] alpha = alphabetCap.ToCharArray();

            for (int i = 0; i < alphabetCap.Length; i++)
            {
                shiftAlpha[i] = alpha[(i + n) % 26];
            }
            return shiftAlpha;
        }
    }
}
