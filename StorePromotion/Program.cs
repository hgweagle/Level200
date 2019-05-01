using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StorePromotion
{
    public class Program
    {
        const int MAX_STORE_NUM = 200;  //maximum number of stores
        const int MAX_TRANSACTION_NUM = 10000;   //maximum number of transactions
        const int YEAR_OFFSET = 2000;   //year offset value
        const int BYTE_ARRAY_SIZE = 6;   //size of the binary array
        const int PROMOTION_CODE_LENGTH = 9;   //Length of promotion code
        
        /// <summary>
        /// Get a random character
        /// </summary>
        /// <returns>a random character</returns>
        public static char getRandomChar()
        {
            Random m_rnd = new Random();
            int ret = m_rnd.Next(122);
            while (ret < 48 || (ret > 57 && ret < 65) || (ret > 90 && ret < 97))
            {
                ret = m_rnd.Next(123);
            }
            return (char)ret;
        }

        /// <summary>
        /// Generate Promotion Code
        /// </summary>
        /// <param name="storeId">Store Id</param>
        /// <param name="date">The date when the code is generated</param>
        /// <param name="transactionId">Transaction Id</param>
        /// <returns></returns>
        public static string GeneratePromotionCode(byte storeId, ushort year, byte month, byte day, ushort transactionId, char randomChar)
        {
            //Check if parameters are valid
            if (storeId == 0 || storeId > MAX_STORE_NUM
                || year < 2019 || year > 2255
                || month == 0 || month > 12
                || day == 0 || day > 31
                || transactionId == 0 || transactionId > MAX_TRANSACTION_NUM)
                return "";

            byte[] arr = new byte[BYTE_ARRAY_SIZE];
            string promotionCode = "";

            //Save Store Id
            arr[1] = storeId;
            //Save Year
            arr[0] = (byte)(year - YEAR_OFFSET);
            //Save Month
            arr[2] = (byte)month;
            //Save Day
            arr[4] = (byte)day;
            //Save high 8 bits of Transaction Id
            arr[3] = (byte)(transactionId >> 8);
            //Save low 8 bits of Transaction Id
            arr[5] = (byte)(transactionId & 0x00ff);

            //Simple decryption, xor first, then add the value of the random character
            for(int i = 0; i < arr.Length; i++)
            {
                arr[i] = (byte)((arr[i] ^ 0xff) + randomChar);
            }

            //Transform byte array to readable string
            promotionCode = Convert.ToBase64String(arr);

            //Append the random character in the end of the promotion code
            return (promotionCode + randomChar);
        }

        /// <summary>
        /// Decode relevant information from given code
        /// </summary>
        /// <param name="promotionCode">Promotion Code</param>
        public static void DecodeInfo(string promotionCode)
        {
            //Check if promotion code is valid
            if (promotionCode.Length != PROMOTION_CODE_LENGTH)
            {
                Console.WriteLine("The promotion code is invalid!");
                return;
            }

            //Transform readable string to byte array
            byte[] arr = Convert.FromBase64String(promotionCode.Substring(0,8));
            char randomChar = promotionCode.ToCharArray()[8];

            //Simple encryption, deduct the value of the random character, then xor
            for (int i = 0; i < arr.Length; i++)
            {
                arr[i] = (byte)((arr[i] - randomChar) ^ 0xff);
            }

            //Get Store Id
            byte storeId = arr[1];
            //Get Transaction Id
            ushort transactionId = (ushort)((ushort)(arr[3] << 8) + arr[5]);
            //Get date in format "yyyy-mm-dd"
            string date = (arr[0] + YEAR_OFFSET) + "-" + arr[2] + "-" + arr[4];

            Console.WriteLine($"The decoded information from {promotionCode} is as below.");
            Console.WriteLine("Store Id:" + storeId);
            Console.WriteLine("Transaction Id:" + transactionId);
            Console.WriteLine("Date:" + date);
        }

        /// <summary>
        /// Main function
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            byte storeId = 0;
            ushort transactionId = 0;
            string readStr = "";
            DateTime date = DateTime.Now;

            //Input Store Id
            do
            {
                Console.WriteLine($"Please input Store Id (1 ~ {MAX_STORE_NUM}). Input 0 to exit.");
                readStr = Console.ReadLine();
            } while (readStr.Length > 3 || Convert.ToInt32(readStr) > MAX_STORE_NUM);

            storeId = Convert.ToByte(readStr);
            //Input 0 in order to exit
            if (storeId == 0) return;

            //Input Transaction Id
            do
            {
                Console.WriteLine($"Please input Transaction Id (1 ~ {MAX_TRANSACTION_NUM}). Input 0 to exit.");
                readStr = Console.ReadLine();
            } while (readStr.Length > 5 || Convert.ToInt32(readStr) > MAX_TRANSACTION_NUM);

            transactionId = Convert.ToUInt16(readStr);
            //Input 0 in order to exit
            if (transactionId == 0) return;

            //Get a random character
            char randomChar = getRandomChar();

            //Generate Promotion Code
            string promotionCode = GeneratePromotionCode(storeId, (ushort)date.Year, (byte)date.Month, (byte)date.Day, transactionId, randomChar);

            Console.WriteLine("Generated Promotion Code:");
            Console.WriteLine(promotionCode);
            Console.WriteLine("");

            //Decode Generated Promotion Code
            DecodeInfo(promotionCode);
        }
    }
}
