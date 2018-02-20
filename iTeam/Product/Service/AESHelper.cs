/**************************************************************************************\
*                                                                                      *
* AESHelper.cs -  AES functions, types, and definitions.       *
*                                                                                      *
*               Version 1.00 ��                                                        *
*                                                                                      *
*               Copyright (c) 2016-2016, iTeam. All rights reserved.               *
*               Created by Todd.                                                 *
*                                                                                      *
***************************************************************************************/

using System;
using System.Collections.Generic;
using System.Text;
using System.Security.Cryptography;

namespace OwLib
{
    /// <summary>
    /// �ӽ�����
    /// </summary>
    public class AESHelper
    {
        public static String m_key = "titabc321zb=ABCDEFGHIJKL";

        /// <summary>
        /// ����
        /// </summary>
        /// <param name="toDecrypt">����</param>
        /// <returns>���</returns>
        public static String Decrypt(String toDecrypt)
        {
            try
            {
                byte[] keyArray = UTF8Encoding.UTF8.GetBytes(m_key);
                byte[] toEncryptArray = Convert.FromBase64String(toDecrypt);
                RijndaelManaged rDel = new RijndaelManaged();
                rDel.Key = keyArray;
                rDel.Mode = CipherMode.ECB;
                rDel.Padding = PaddingMode.PKCS7;
                ICryptoTransform cTransform = rDel.CreateDecryptor();
                byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);
                return UTF8Encoding.UTF8.GetString(resultArray);
            }
            catch (Exception ex)
            {
                return "";
            }
        }

        /// <summary>
        /// ����
        /// </summary>
        /// <param name="toEncrypt">����</param>
        /// <returns>���</returns>
        public static String Encrypt(String toEncrypt)
        {
            byte[] keyArray = UTF8Encoding.UTF8.GetBytes(m_key);
            byte[] toEncryptArray = UTF8Encoding.UTF8.GetBytes(toEncrypt);
            RijndaelManaged rDel = new RijndaelManaged();
            rDel.Key = keyArray;
            rDel.Mode = CipherMode.ECB;
            rDel.Padding = PaddingMode.PKCS7;
            ICryptoTransform cTransform = rDel.CreateEncryptor();
            byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);
            return Convert.ToBase64String(resultArray, 0, resultArray.Length);
        }
    }
}
