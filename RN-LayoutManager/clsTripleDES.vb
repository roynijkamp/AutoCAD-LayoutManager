Imports System.Security.Cryptography
Public Class clsTripleDES

    Private bPassword As Byte()
    Private sPassword As String

    Public Sub New(Optional ByVal Password As String = "password")
        ' On Class Begin
        Me.Password = Password
    End Sub

    Public ReadOnly Property PasswordHash As String
        Get
            Dim UTF8 As System.Text.UTF8Encoding = New System.Text.UTF8Encoding
            Return UTF8.GetString(bPassword)
        End Get
    End Property

    Public Property Password() As String
        Get
            Dim UTF8 As System.Text.UTF8Encoding = New System.Text.UTF8Encoding
            Return sPassword
        End Get
        Set(value As String)
            Dim UTF8 As System.Text.UTF8Encoding = New System.Text.UTF8Encoding
            Dim HashProvider As MD5CryptoServiceProvider = New MD5CryptoServiceProvider()
            bPassword = HashProvider.ComputeHash(UTF8.GetBytes(value))
            sPassword = value
        End Set
    End Property

#Region "Encrypt"

    ' Encrypt using Password from Property Set (pre-hashed)
    Public Function Encrypt(ByVal Message As String) As String
        Dim Results() As Byte
        Dim UTF8 As System.Text.UTF8Encoding = New System.Text.UTF8Encoding
        Using HashProvider As MD5CryptoServiceProvider = New MD5CryptoServiceProvider()
            Dim TDESKey() As Byte = bPassword
            Using TDESAlgorithm As TripleDESCryptoServiceProvider = New TripleDESCryptoServiceProvider() With {.Key = TDESKey, .Mode = CipherMode.ECB, .Padding = PaddingMode.PKCS7}
                Dim DataToEncrypt() As Byte = UTF8.GetBytes(Message)
                Try
                    Dim Encryptor As ICryptoTransform = TDESAlgorithm.CreateEncryptor
                    Results = Encryptor.TransformFinalBlock(DataToEncrypt, 0, DataToEncrypt.Length)
                Finally
                    TDESAlgorithm.Clear()
                    HashProvider.Clear()
                End Try
            End Using
        End Using
        Return Convert.ToBase64String(Results)
    End Function

    ' Encrypt using Password as byte array
    Private Function Encrypt(ByVal Message As String, ByVal Password() As Byte) As String
        Dim Results() As Byte
        Dim UTF8 As System.Text.UTF8Encoding = New System.Text.UTF8Encoding
        Using HashProvider As MD5CryptoServiceProvider = New MD5CryptoServiceProvider()
            Dim TDESKey() As Byte = HashProvider.ComputeHash(UTF8.GetBytes(UTF8.GetString(Password)))
            Using TDESAlgorithm As TripleDESCryptoServiceProvider = New TripleDESCryptoServiceProvider() With {.Key = TDESKey, .Mode = CipherMode.ECB, .Padding = PaddingMode.PKCS7}
                Dim DataToEncrypt() As Byte = UTF8.GetBytes(Message)
                Try
                    Dim Encryptor As ICryptoTransform = TDESAlgorithm.CreateEncryptor
                    Results = Encryptor.TransformFinalBlock(DataToEncrypt, 0, DataToEncrypt.Length)
                Finally
                    TDESAlgorithm.Clear()
                    HashProvider.Clear()
                End Try
            End Using
        End Using
        Return Convert.ToBase64String(Results)
    End Function

    ' Encrypt using Password as string
    Public Function Encrypt(ByVal Message As String, ByVal Password As String) As String
        Dim Results() As Byte
        Dim UTF8 As System.Text.UTF8Encoding = New System.Text.UTF8Encoding
        ' Step 1. We hash the Passphrase using MD5
        ' We use the MD5 hash generator as the result is a 128 bit byte array
        ' which is a valid length for the TripleDES encoder we use below
        Using HashProvider As MD5CryptoServiceProvider = New MD5CryptoServiceProvider()
            Dim TDESKey() As Byte = HashProvider.ComputeHash(UTF8.GetBytes(Password))

            ' Step 2. Create a new TripleDESCryptoServiceProvider object

            ' Step 3. Setup the encoder
            Using TDESAlgorithm As TripleDESCryptoServiceProvider = New TripleDESCryptoServiceProvider() With {.Key = TDESKey, .Mode = CipherMode.ECB, .Padding = PaddingMode.PKCS7}
                ' Step 4. Convert the input string to a byte[]

                Dim DataToEncrypt() As Byte = UTF8.GetBytes(Message)

                ' Step 5. Attempt to encrypt the string
                Try
                    Dim Encryptor As ICryptoTransform = TDESAlgorithm.CreateEncryptor
                    Results = Encryptor.TransformFinalBlock(DataToEncrypt, 0, DataToEncrypt.Length)
                Finally
                    ' Clear the TripleDes and Hashprovider services of any sensitive information
                    TDESAlgorithm.Clear()
                    HashProvider.Clear()
                End Try
            End Using
        End Using

        ' Step 6. Return the encrypted string as a base64 encoded string
        Return Convert.ToBase64String(Results)
    End Function
#End Region

#Region "Decrypt"
    ' Decrypt using Password from Property (pre-hashed)
    Public Function Decrypt(ByVal Message As String) As String
        Dim Results() As Byte
        Dim UTF8 As System.Text.UTF8Encoding = New System.Text.UTF8Encoding
        Using HashProvider As MD5CryptoServiceProvider = New MD5CryptoServiceProvider()
            Dim TDESKey() As Byte = Me.bPassword
            Using TDESAlgorithm As TripleDESCryptoServiceProvider = New TripleDESCryptoServiceProvider() With {.Key = TDESKey, .Mode = CipherMode.ECB, .Padding = PaddingMode.PKCS7}
                Dim DataToDecrypt() As Byte = Convert.FromBase64String(Message)
                Try
                    Dim Decryptor As ICryptoTransform = TDESAlgorithm.CreateDecryptor
                    Results = Decryptor.TransformFinalBlock(DataToDecrypt, 0, DataToDecrypt.Length)
                Finally
                    TDESAlgorithm.Clear()
                    HashProvider.Clear()
                End Try
            End Using
        End Using
        Return UTF8.GetString(Results)
    End Function

    ' Decrypt using Password as Byte array
    Public Function Decrypt(ByVal Message As String, ByVal Password() As Byte) As String
        Dim Results() As Byte
        Dim UTF8 As System.Text.UTF8Encoding = New System.Text.UTF8Encoding
        Using HashProvider As MD5CryptoServiceProvider = New MD5CryptoServiceProvider()
            Dim TDESKey() As Byte = HashProvider.ComputeHash(UTF8.GetBytes(UTF8.GetString(Password)))
            Using TDESAlgorithm As TripleDESCryptoServiceProvider = New TripleDESCryptoServiceProvider() With {.Key = TDESKey, .Mode = CipherMode.ECB, .Padding = PaddingMode.PKCS7}
                Dim DataToDecrypt() As Byte = Convert.FromBase64String(Message)
                Try
                    Dim Decryptor As ICryptoTransform = TDESAlgorithm.CreateDecryptor
                    Results = Decryptor.TransformFinalBlock(DataToDecrypt, 0, DataToDecrypt.Length)
                Finally
                    TDESAlgorithm.Clear()
                    HashProvider.Clear()
                End Try
            End Using
        End Using
        Return UTF8.GetString(Results)
    End Function


    ' Decrypt using Password as string
    Public Function Decrypt(ByVal Message As String, ByVal Password As String) As String
        Dim Results() As Byte
        Dim UTF8 As System.Text.UTF8Encoding = New System.Text.UTF8Encoding

        ' Step 1. We hash the Pass phrase using MD5
        ' We use the MD5 hash generator as the result is a 128 bit byte array
        ' which is a valid length for the TripleDES encoder we use below
        Using HashProvider As MD5CryptoServiceProvider = New MD5CryptoServiceProvider()
            Dim TDESKey() As Byte = HashProvider.ComputeHash(UTF8.GetBytes(Password))

            ' Step 2. Create a new TripleDESCryptoServiceProvider object
            ' Step 3. Setup the decoder
            Using TDESAlgorithm As TripleDESCryptoServiceProvider = New TripleDESCryptoServiceProvider() With {.Key = TDESKey, .Mode = CipherMode.ECB, .Padding = PaddingMode.PKCS7}

                ' Step 4. Convert the input string to a byte[]
                Dim DataToDecrypt() As Byte = Convert.FromBase64String(Message)
                ' Step 5. Attempt to decrypt the string
                Try
                    Dim Decryptor As ICryptoTransform = TDESAlgorithm.CreateDecryptor
                    Results = Decryptor.TransformFinalBlock(DataToDecrypt, 0, DataToDecrypt.Length)
                Finally

                    ' Clear the TripleDes and Hash provider services of any sensitive information
                    TDESAlgorithm.Clear()
                    HashProvider.Clear()
                End Try
            End Using
        End Using

        ' Step 6. Return the decrypted string in UTF8 format
        Return UTF8.GetString(Results)
    End Function

#End Region
End Class
