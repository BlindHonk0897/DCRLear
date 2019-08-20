using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace DCRSystem.Models
{
    public class PasswordSecurity
    {

        public  string Encryptdata(string password)
        {
            string strmsg = string.Empty;
            byte[] encode = new byte[password.Length];
            encode = Encoding.UTF8.GetBytes(password);
            strmsg = Convert.ToBase64String(encode);
            return strmsg;
        }
        public  string Decryptdata(string encryptpwd)
        {
            string decryptpwd = string.Empty;
            UTF8Encoding encodepwd = new UTF8Encoding();
            Decoder Decode = encodepwd.GetDecoder();
            byte[] todecode_byte = Convert.FromBase64String(encryptpwd);
            int charCount = Decode.GetCharCount(todecode_byte, 0, todecode_byte.Length);
            char[] decoded_char = new char[charCount];
            Decode.GetChars(todecode_byte, 0, todecode_byte.Length, decoded_char, 0);
            decryptpwd = new String(decoded_char);
            return decryptpwd;
        }
         
        //LEAR ENCRYPT PASSWORD USED
        public string EncryptPassword(string password)
        {
            System.Security.Cryptography.MD5CryptoServiceProvider md5Pass = new System.Security.Cryptography.MD5CryptoServiceProvider();
            byte[] arrB;
            StringBuilder sb = new StringBuilder(string.Empty);

            arrB = md5Pass.ComputeHash(Encoding.ASCII.GetBytes(password));

            foreach (byte b in arrB)
            {
                sb.Append(b.ToString("x").PadLeft(2, '0'));
            }

            md5Pass.Clear();

            return sb.ToString().Substring(0, 15);
        }


    }
}