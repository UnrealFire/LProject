using System.Security.Cryptography;


namespace LProject
{
    class Security
    {
        public static string CreateSalt(string userName)
        {
            // Use first 3 name symbols as password salt
            return userName.Substring(3);
        }

        public string HashPassword(string password, string saltValue)
        {
            HashAlgorithm algorithm = new SHA256Managed();

            byte[] plainTextBytes = System.Text.Encoding.Unicode.GetBytes(password);
            byte[] saltBytes = System.Text.Encoding.Unicode.GetBytes(saltValue);

            byte[] plainTextWithSaltBytes = new byte[plainTextBytes.Length + saltBytes.Length];
            saltBytes.CopyTo(plainTextWithSaltBytes, 0);
            plainTextBytes.CopyTo(plainTextWithSaltBytes, saltValue.Length);

            byte[] result = algorithm.ComputeHash(plainTextWithSaltBytes);
            string hashed = System.Text.Encoding.Unicode.GetString(result);
            return hashed;
        }

        public bool IsUserAdmin(string name)
        {
            DBWork db = new DBWork();
            var users = db.GetUserDataByName(name);

            return users[0].IsAdmin; // TODO: assert list len >= 1 (= 1 in normal case)
        }

        public bool tryLogin(string name, string password)
        {

            DBWork db = new DBWork();

            string salt = CreateSalt(name);

            if (string.IsNullOrEmpty(salt)) return false;

            var hashedPassword = HashPassword(password, salt);

            var userData = db.GetUserDataByName(name);

            if (userData[0].Pass == hashedPassword)
                return true;
            // Ищем в базе соответствие имя/пароль и возвращем сессию с данными, если они есть, и ничего, если их нет.
            return false;
        }
    }
}
