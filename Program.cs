// IBAN in Norway

#region Main Program
string errorMessage = string.Empty;

if (CheckIfArgsAreValid(args, out errorMessage))
{
    if (args[0] == "build")
    {
        CalculateChecksum(out string checksum, "NO", "7", args[1], args[2]);
        string bankCode = args[1];
        string accountNumber = args[2];

        System.Console.WriteLine($"Your IBAN is: {BuildIban(checksum, bankCode, accountNumber, "NO", "7")}");
    }
    else
    {
        AnalyzeIban(args, out string bankCode, out string accountNumber);
        string checksum = args[1].Substring(2,2);
        if (VerifyTheChecksum("NO", "7", bankCode, accountNumber, checksum))
        {
            System.Console.WriteLine("Checksum is valid");
        }
        else
        {
            System.Console.WriteLine("Checksum is invalid");
        }
        System.Console.WriteLine($"Your bank code is: {bankCode}");
        System.Console.WriteLine($"Your account number is: {accountNumber}");
    }
}
else
{
    System.Console.WriteLine(errorMessage);
}
#endregion

#region Methods
bool CheckIfArgsAreValid(string[] args, out string errorMessage)
{
    if (!(args[0] is "build" or "analyze"))
    {
        errorMessage = "Invalid command, must be \"build\" or \"analyze\".";
        return false;
    }

    if (args[0] == "build")
    {
        if (args.Length < 3)
        {
            errorMessage = "Too few arguments";
            return false;
        }
        else if (args.Length > 3)
        {
            errorMessage = "Too many arguments";
            return false;
        }
        else if (CheckIfLetterInString(args[1]))
        {
            errorMessage = "Bank code must not contain letters";
            return false;
        }
        else if (CheckIfLetterInString(args[2]))
        {
            errorMessage = "Account number must not contain letters";
            return false;
        }
        else if (args[1].Length != 4)
        {
            errorMessage = "Bank code has wrong length, must contain 4 digits";
            return false;
        }
        else if (args[2].Length != 6)
        {
            errorMessage = "Account number has wrong length, must contain 6 digits";
            return false;
        }
        errorMessage = string.Empty;
        return true;
    }
    else
    {
        string iban = args[1];
        if (iban.Length != 15)
        {
            errorMessage = "Wrong length of IBAN";
            return false;
        }
        else if (!iban.StartsWith("NO"))
        {
            errorMessage = "Wrong country code, we currently only support \"NO\"";
            return false;
        }
        else if (!iban.EndsWith("7"))
        {
            errorMessage = "Wrong national check digit, we currently only support \"7\"";
            return false;
        }
        errorMessage = string.Empty;
        return true;
    }
}

bool CheckIfLetterInString(string input)
{
    foreach (char c in input)
    {
        if (char.IsLetter(c))
        {
            return true;
        }
    }
    return false;
}

string BuildIban(string checksum, string bankCode, string accountNumber, string country, string lastNumber)
{
    return $"{country}{checksum}{bankCode}{accountNumber}{lastNumber}";
}

void AnalyzeIban(string[] args, out string bankCode, out string accountNumber)
{
    bankCode = args[1].Substring(4,4);
    accountNumber = args[1].Substring(8,6);
}

void CalculateChecksum(out string checksum, string country, string lastNumber, string bankCode, string accountNumber)
{
    string[] bigNumber = {bankCode, accountNumber, lastNumber, $"{country[0] - 'A' + 10}{country[1] - 'A' + 10}", "00"};
    ulong number = 0;
    for (int i = 0; i < bigNumber.Length; i++)
    {
       string easy = $"{number}{bigNumber[i]}"; 
       number = (ulong.Parse(easy) % 97);
    }
    checksum = $"{98 - number}";
}

bool VerifyTheChecksum(string country, string lastNumber, string bankCode, string accountNumber, string checksum)
{
    string countryNumber = $"{country[0] - 'A' + 10}{country[1] - 'A' + 10}";
    string bigNumber = $"{bankCode}{accountNumber}{lastNumber}{countryNumber}{checksum}";
    ulong number = ulong.Parse(bigNumber) % 97;
    if (number != 1)
    {
        return false;
    }
    return true;
}
#endregion
