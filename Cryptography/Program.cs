﻿using System.Collections;
using System.Security.Cryptography;  
using System.Text;   
class Zadanie1  
{  
    public static void Start(string typ)
    {
        if (typ == "")
        {
            Console.WriteLine("Brak parametrów. Podaj typ polecenia (0, 1 lub 2).");
            return;
        }

        int typPolecenia;
        if (!int.TryParse(typ, out typPolecenia))
        {
            Console.WriteLine("Nieprawidłowy typ polecenia.");
            return;
        }

        switch (typPolecenia)
        {
            case 0:
                GenerujKlucze();
                break;
            case 1:
                
                Console.WriteLine("Brak wymaganych argumentów. Podaj nazwę pliku wejściowego.");
                string plikWejsciowy = Console.ReadLine();
                Console.WriteLine("Brak wymaganych argumentów. Podaj nazwę pliku wyjściowego.");
                string plikWyjsciowy = Console.ReadLine();
                SzyfrujPlik(plikWejsciowy, plikWyjsciowy, "publicKey.dat");
                break;
            case 2:
                Console.WriteLine("Brak wymaganych argumentów. Podaj nazwę pliku wejściowego.");
                string plikWejsciowy2 = Console.ReadLine();
                Console.WriteLine("Brak wymaganych argumentów. Podaj nazwę pliku wyjściowego.");
                string plikWyjsciowy2 = Console.ReadLine();
                OdszyfrujPlik(plikWejsciowy2, plikWyjsciowy2, "privateKey.dat");
                break;
            default:
                Console.WriteLine("Nieprawidłowy typ polecenia.");
                return;
        }
    }

    static void GenerujKlucze()
    {
        RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
        string filePublicKey = "publicKey.dat";
        string filePrivateKey = "privateKey.dat";

        string publicKey = rsa.ToXmlString(false);
        File.WriteAllText(filePublicKey, publicKey);

        string privateKey = rsa.ToXmlString(true);
        File.WriteAllText(filePrivateKey, privateKey);

        Console.WriteLine("Wygenerowano klucze i zapisano do plików.");
    }

    static void SzyfrujPlik(string plikWejsciowy, string plikWyjsciowy, string kluczPubliczny)
    {
        string publicKey = File.ReadAllText(kluczPubliczny);

        byte[] daneDoZaszyfrowania = File.ReadAllBytes(plikWejsciowy);

        byte[] zaszyfrowaneDane;
        using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider())
        {
            rsa.FromXmlString(publicKey);
            zaszyfrowaneDane = rsa.Encrypt(daneDoZaszyfrowania, false);
        }

        File.WriteAllBytes(plikWyjsciowy, zaszyfrowaneDane);

        Console.WriteLine("Plik został zaszyfrowany.");
    }

    static void OdszyfrujPlik(string plikWejsciowy, string plikWyjsciowy, string kluczPrywatny)
    {
        string privateKey = File.ReadAllText(kluczPrywatny);

        byte[] daneDoOdszyfrowania = File.ReadAllBytes(plikWejsciowy);

        byte[] odszyfrowaneDane;
        using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider())
        {
            rsa.FromXmlString(privateKey);
            odszyfrowaneDane = rsa.Decrypt(daneDoOdszyfrowania, false);
        }

        File.WriteAllBytes(plikWyjsciowy, odszyfrowaneDane);

        Console.WriteLine("Plik został odszyfrowany.");
    }
}

class Zadanie2{
    public static void Start()
    {
    Console.WriteLine("Podaj nazwę pliku:");
        string nazwaPliku = Console.ReadLine();

        Console.WriteLine("Podaj nazwę pliku zawierającego hash:");
        string nazwaPlikuHash = Console.ReadLine();

        Console.WriteLine("Algorytm hashowania (SHA256, SHA512, MD5):");
        string algorytm = Console.ReadLine();

        HashAlgorithmName hashAlgorithm;

        switch (algorytm.ToUpper())
        {
            case "SHA256":
                hashAlgorithm = HashAlgorithmName.SHA256;
                break;
            case "SHA512":
                hashAlgorithm = HashAlgorithmName.SHA512;
                break;
            case "MD5":
                hashAlgorithm = HashAlgorithmName.MD5;
                break;
            default:
                Console.WriteLine("Nieprawidłowy algorytm hashowania.");
                return;
        }

        string hashPliku = ObliczHash(nazwaPliku, hashAlgorithm);
        Console.WriteLine("Suma kontrolna pliku " + nazwaPliku + ": " + hashPliku);

        if (!string.IsNullOrEmpty(nazwaPlikuHash))
        {
            if (!File.Exists(nazwaPlikuHash))
            {
                Console.WriteLine("Plik " + nazwaPlikuHash + " nie istnieje. Zapisuję hash pliku " + nazwaPliku + " do pliku " + nazwaPlikuHash + ".");
                File.WriteAllText(nazwaPlikuHash, hashPliku);
                Console.WriteLine("Hash został zapisany.");
            }
            else
            {
                Console.WriteLine("Sprawdzanie zgodności hasha pliku " + nazwaPliku + " z hashem w pliku " + nazwaPlikuHash + "...");
                string zapisanyHash = File.ReadAllText(nazwaPlikuHash);

                if (hashPliku == zapisanyHash)
                {
                    Console.WriteLine("Hash pliku jest zgodny z hashem w pliku " + nazwaPlikuHash);
                }
                else
                {
                    Console.WriteLine("Uwaga! Hash pliku nie jest zgodny z hashem w pliku " + nazwaPlikuHash);
                }
            }
        }
    }

    static string ObliczHash(string nazwaPliku, HashAlgorithmName hashAlgorithm)
    {
        using (var hasher = HashAlgorithm.Create(hashAlgorithm.Name))
        {
            using (var stream = File.OpenRead(nazwaPliku))
            {
                byte[] hashBytes = hasher.ComputeHash(stream);
                return BitConverter.ToString(hashBytes).Replace("-", string.Empty).ToLower();
            }
        }
    }
}
class Zadanie3{
    public static void Start(){
        Console.WriteLine("Podaj nazwe pliku do wczytania");
        string nazwaPlikuA = Console.ReadLine(); 
        Console.WriteLine("Podaj nazwe pliku do wczytania");
        string nazwaPlikuB = Console.ReadLine(); 
        string filePublicKey = "publicKey.dat";
        string filePrivateKey = "privateKey.dat";

        if (!File.Exists(nazwaPlikuA))
        {
            Console.WriteLine("Plik " + nazwaPlikuA + " nie istnieje.");
            return;
        }
        if (File.Exists(nazwaPlikuB))
        {
            Console.WriteLine("Weryfikowanie podpisu pliku " + nazwaPlikuA + " przy użyciu klucza publicznego z pliku " + nazwaPlikuB + "...");
            bool podpisPoprawny = WeryfikujPodpis(nazwaPlikuA, nazwaPlikuB);

            if (podpisPoprawny)
            {
                Console.WriteLine("Podpis jest poprawny.");
            }
            else
            {
                Console.WriteLine("Uwaga! Podpis jest niepoprawny.");
            }
        }
        else
        {
            Console.WriteLine("Generowanie podpisu pliku " + nazwaPlikuA + " i zapisywanie go do pliku " + nazwaPlikuB + "...");
            GenerujIPodpisz(nazwaPlikuA, nazwaPlikuB);
            Console.WriteLine("Podpis został wygenerowany i zapisany.");
        }
    }
    static void GenerujIPodpisz(string nazwaPlikuWejsciowego, string nazwaPlikuPodpisu)
    {
        using (var rsa = new RSACryptoServiceProvider())
        {
            try
            {
                rsa.FromXmlString(File.ReadAllText("privateKey.dat"));

                byte[] daneDoPodpisania = File.ReadAllBytes(nazwaPlikuWejsciowego);
                byte[] podpis = rsa.SignData(daneDoPodpisania, new SHA256CryptoServiceProvider());

                File.WriteAllBytes(nazwaPlikuPodpisu, podpis);
            }
            finally
            {
                rsa.PersistKeyInCsp = false;
            }
        }
    }
    static bool WeryfikujPodpis(string nazwaPlikuWejsciowego, string nazwaPlikuPodpisu)
    {
        using (var rsa = new RSACryptoServiceProvider())
        {
            try
            {
                rsa.FromXmlString(File.ReadAllText("publicKey.dat"));

                byte[] daneDoWeryfikacji = File.ReadAllBytes(nazwaPlikuWejsciowego);
                byte[] podpis = File.ReadAllBytes(nazwaPlikuPodpisu);

                return rsa.VerifyData(daneDoWeryfikacji, new SHA256CryptoServiceProvider(), podpis);
            }
            finally
            {
                rsa.PersistKeyInCsp = false;
            }
        }
    }

}
public class Zadanie4
{
    public static void Start()
    {
        Console.WriteLine("Podaj nazwę pliku wejściowego:");
        string plikWejsciowy = Console.ReadLine();
        Console.WriteLine("Podaj nazwę pliku wyjściowego:");
        string plikWyjsciowy = Console.ReadLine();
        Console.WriteLine("Podaj hasło:");
        string haslo = Console.ReadLine();
        Console.WriteLine("Podaj typ operacji (0 - zaszyfrowanie, 1 - odszyfrowanie):");
        string typOperacji = Console.ReadLine();
        byte[] initVector = RandomNumberGenerator.GetBytes(16);
        int liczbaIteracji = 2000;
   
        if (typOperacji == "0")
        {
            Encrypt(plikWejsciowy,plikWyjsciowy,haslo);
            Console.WriteLine("Plik został zaszyfrowany.");
        }
        else if (typOperacji == "1")
        {
        Decrypt(plikWejsciowy,plikWyjsciowy,haslo);
        Console.WriteLine("Plik został odszyfrowany.");

        }
        else
        {
            Console.WriteLine("Nieprawidłowy typ operacji.");
        }
    }
    public static void Encrypt(string inputFile, string outputFile, string password)
        {
        var keyGenerator = new Rfc2898DeriveBytes(password, 8);
        var rijndael = Rijndael.Create();

        rijndael.IV = keyGenerator.GetBytes(rijndael.BlockSize / 8);
        rijndael.Key = keyGenerator.GetBytes(rijndael.KeySize / 8);

        using (var inputStream = File.OpenRead(inputFile))
        using (var outputStream = File.Create(outputFile))
        {
            outputStream.Write(keyGenerator.Salt, 0, 8);

            using (var cryptoStream = new CryptoStream(outputStream, rijndael.CreateEncryptor(), CryptoStreamMode.Write))
            {
                inputStream.CopyTo(cryptoStream);
            }
        }
    }

    public static void Decrypt(string inputFile, string outputFile, string password)
    {
        using (var inputStream = File.OpenRead(inputFile))
        {
            var salt = new byte[8];
            inputStream.Read(salt, 0, 8);

            var keyGenerator = new Rfc2898DeriveBytes(password, salt);
            var rijndael = Rijndael.Create();
            rijndael.IV = keyGenerator.GetBytes(rijndael.BlockSize / 8);
            rijndael.Key = keyGenerator.GetBytes(rijndael.KeySize / 8);

            using (var outputStream = File.Create(outputFile))
            using (var cryptoStream = new CryptoStream(inputStream, rijndael.CreateDecryptor(), CryptoStreamMode.Read))
            {
                cryptoStream.CopyTo(outputStream);
            }
        }
    }


}
class Program{

   static void Main(string[] args)
    {
        while (true){
        Console.WriteLine("Wpisz numer zadanie:");
        string zadanie = Console.ReadLine();
        switch (zadanie)
        {
            case "1":
                Console.WriteLine("Podaj typ polecenia (0, 1, 2):");
                string typ = Console.ReadLine();
                string tekstDoSzyfrowania = "To jest przykładowy tekst do zaszyfrowania.";
                string nazwaPliku = "doSzyfrowania.dat";
                File.WriteAllBytes(nazwaPliku, ConvertStringToBytes(tekstDoSzyfrowania));

                Console.WriteLine("Plik do szyfrowania został wygenerowany.");

                Zadanie1.Start(typ);
                break;

            case "2":
                Zadanie2.Start();
                break;

            case "3":
                Zadanie3.Start();
                break;

            case "4":
                Zadanie4.Start();
                break;
            case "x":
                return;
            default:
                Console.WriteLine("Nieznany numer zadania.");
                break;
        }
        
    }
    // komentarz 
        
        static byte[] ConvertStringToBytes(string text)
        {
            return System.Text.Encoding.UTF8.GetBytes(text);
        }
    }
}
