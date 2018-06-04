﻿using System;
using System.IO;
using System.Security.Cryptography;

namespace g0nnaL4ugh.Crypto
{
	public class Encrypter
	{
		public Encrypter()
		{
		}

		private static byte[] GenerateTrulyRandom(int length)
		{
			byte[] randomBytes = new byte[length];
			using (var randito = new RNGCryptoServiceProvider())
			{
				randito.GetNonZeroBytes(randomBytes);
				randito.Dispose();
			}
			return randomBytes;
		}

		public string GenerateRandomPrivateKey()
		{
			// The length will be random between 23 and 28
			Random random = new Random();
			int length = random.Next(23, 28);
			byte[] randomBytes = Encrypter.GenerateTrulyRandom(length);
			return Convert.ToBase64String(randomBytes);
		}

		public string GenerateRandomSalt(int length)
		{
			byte[] randomBytes = Encrypter.GenerateTrulyRandom(length);
			return Convert.ToBase64String(randomBytes);
		}

		public byte[] EncryptBytes(byte[] bytes2Encrypt, byte[] password, byte[] salt)
		{
			byte[] encryptedBytes = null;
			using (MemoryStream ms = new MemoryStream())
			{
                using (RijndaelManaged RJM = new RijndaelManaged())
                {
                    /*
                    * Setting up the encryption block size (CBC size) and 
                    * key size (derivate key from password and salt with # iterations)
                    */
                    RJM.KeySize = 256;
                    RJM.BlockSize = 192;
                    var key = new Rfc2898DeriveBytes(password, salt, 1500);
                    RJM.Key = key.GetBytes(RJM.KeySize / 8);
                    RJM.IV = key.GetBytes(RJM.BlockSize / 8);
                    /* Set CBC mode, it's the most secure for AES alike */
                    RJM.Mode = CipherMode.CBC;
                    /* Finally encript bytes */
                    using (var cs = new CryptoStream(ms, RJM.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(bytes2Encrypt, 0, bytes2Encrypt.Length);
                        cs.Close();
                    }
                    /* TODO: We need to append the salt to the encrypted bytes */
                    encryptedBytes = ms.ToArray();
                }
            }
            return encryptedBytes;
        }
    }
}
