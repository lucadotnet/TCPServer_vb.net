Imports System.Data.SqlClient
Imports System.IO
Imports System.Net
Imports System.Net.Sockets
Imports System.Text
Module Module1
    Dim port As Integer = 8888
    Sub Main()
        Dim ctThread As Threading.Thread = New Threading.Thread(AddressOf doChat)
        ctThread.Start()
    End Sub

    Dim kwIniziali As Int64 = 0
    Dim lastKw As Double = 0
    Private Function DoCommand(ByVal command As Char, ByVal param As String, ByRef paramOut As String) As Integer
        Dim sqlConnection1 As New SqlConnection("Data Source=YOUR_SERVER\DATABASEISTANCE;Initial Catalog=YOUR_DATABSENAME;Persist Security Info=True;User ID=YOUR_USER;Password=YOUR_PASSWORD")
        Dim cmd As New SqlCommand
        Dim cudate As DateTime = DateTime.Now
        Dim kwInzializzati As Boolean = False


        Select Case command
            Case "P"
                Try
                    cmd = New SqlCommand("INSERT INTO [dbo].[YOUR_TABLE_NAME] ([value], [date]) VALUES (@value, @Date);", sqlConnection1)
                    cmd.Parameters.AddWithValue("@value", Convert.ToDecimal(param))
                    cmd.Parameters.AddWithValue("@Date", cudate)
                    sqlConnection1.Open()

                    cmd.ExecuteNonQuery()
                    paramOut = param

                Catch ex As Exception
                    paramOut = ex.Message
                Finally
                    If sqlConnection1.State = ConnectionState.Open Then
                        sqlConnection1.Close()
                    End If
                    sqlConnection1.Dispose()
                    sqlConnection1 = Nothing

                    cmd = Nothing
                End Try

                Return 0

            Case "T"
                Dim potenza As String
                Dim kw As String

                If param.IndexOf(";") = 0 Then
                    paramOut = "String di input non corretta - manca ; "
                    Return 0
                End If

                Try
                    potenza = param.Split(";")(0)
                    kw = param.Split(";")(1)
                Catch ex As Exception
                    paramOut = ex.Message
                    Return 0
                End Try

                Dim pw As Integer = Convert.ToDecimal(potenza)

                If pw < 0 Then
                    pw = 0
                End If

                Try
                    cmd = New SqlCommand("INSERT INTO [dbo].[YOUR_TABLE_NAME] ([value], [date]) VALUES (@value, @Date);", sqlConnection1)
                    cmd.Parameters.AddWithValue("@value", pw)
                    cmd.Parameters.AddWithValue("@Date", cudate)
                    sqlConnection1.Open()

                    cmd.ExecuteNonQuery()
                    paramOut = param

                Catch ex As Exception
                    paramOut = ex.Message
                    LogFile("LogError", "doCommand TP", ex.Message)
                Finally
                    If sqlConnection1.State = ConnectionState.Open Then
                        sqlConnection1.Close()
                    End If
                    'sqlConnection1.Dispose()
                    'sqlConnection1 = Nothing

                    'cmd = Nothing
                End Try


                Try
                    Dim StrConnection As String = "SELECT TOP 1 [value] FROM YOUR_TOTKW_TABLE_NAME order by id desc"

                    Dim adapter As New SqlDataAdapter(StrConnection, sqlConnection1)
                    Dim table As New DataTable()

                    adapter.Fill(table)

                    Dim totKW As Integer = Convert.ToDecimal(kw)
                    'LogFile("LogExtra", "doCommand", "totKW (param): " + totKW)

                    If (totKW < table.Rows(0)(0)) Then
                        If kwInzializzati = True Then
                            kwInzializzati = True
                            kwIniziali = table.Rows(0)(0)
                            'LogFile("LogExtra", "doCommand", "kwIniziali: " + kwIniziali)
                        End If
                    Else
                        kwInzializzati = False
                        kwIniziali = 0
                    End If

                    totKW = totKW + kwIniziali
                    'LogFile("LogExtra", "doCommand", "totKW: " + totKW)

                    If (totKW < 0) Then
                        totKW = 0
                    End If

                    adapter.Dispose()
                    table.Clear()
                    table.Dispose()

                    ' scrivo solo se kw != ultimo valore nel database
                    ' per non riempire il database di valori uguali
                    If (lastKw <> totKW) Then
                        cmd = New SqlCommand("INSERT INTO [dbo].[YOUR_TOTKW_TABLE_NAME] ([value], [date]) VALUES (@value, @Date);", sqlConnection1)
                        cmd.Parameters.AddWithValue("@value", Convert.ToDecimal(totKW))
                        cmd.Parameters.AddWithValue("@Date", cudate)
                        sqlConnection1.Open()

                        cmd.ExecuteNonQuery()
                    End If

                    lastKw = totKW
                    paramOut = totKW

                Catch ex As Exception
                    paramOut = ex.Message
                    LogFile("LogError", "doCommand TW", ex.Message)
                Finally
                    If sqlConnection1.State = ConnectionState.Open Then
                        sqlConnection1.Close()
                    End If
                    sqlConnection1.Dispose()
                    sqlConnection1 = Nothing

                    cmd = Nothing
                End Try

                Return 0


            Case "W"
                Try
                    Dim StrConnection As String = "SELECT TOP 1 [value] FROM YOUR_TOTKW_TABLE_NAME order by id desc"

                    Dim adapter As New SqlDataAdapter(StrConnection, sqlConnection1)
                    Dim table As New DataTable()

                    adapter.Fill(table)

                    Dim totKW As Integer = Convert.ToDecimal(param)
                    LogFile("LogExtra", "doCommand", "totKW (param): " + totKW)

                    If (totKW < table.Rows(0)(0)) Then
                        If kwInzializzati = False Then
                            kwInzializzati = True
                            kwIniziali = table.Rows(0)(0)
                            LogFile("LogExtra", "doCommand", "kwIniziali: " + kwIniziali)
                        End If
                    End If

                    totKW = totKW + kwIniziali
                    LogFile("LogExtra", "doCommand", "totKW: " + totKW)

                    adapter.Dispose()
                    table.Clear()
                    table.Dispose()

                    cmd = New SqlCommand("INSERT INTO [dbo].[YOUR_TOTKW_TABLE_NAME] ([value], [date]) VALUES (@value, @Date);", sqlConnection1)
                    cmd.Parameters.AddWithValue("@value", Convert.ToDecimal(param))
                    cmd.Parameters.AddWithValue("@Date", cudate)
                    sqlConnection1.Open()

                    cmd.ExecuteNonQuery()
                    paramOut = totKW

                Catch ex As Exception
                    paramOut = ex.Message
                    LogFile("LogError", "doCommand ", ex.Message)

                Finally
                    If sqlConnection1.State = ConnectionState.Open Then
                        sqlConnection1.Close()
                    End If
                    sqlConnection1.Dispose()
                    sqlConnection1 = Nothing

                    cmd = Nothing
                End Try

                Return 0

            Case "R"
                Try
                    Dim StrConnection As String = "SELECT TOP 1 [value] FROM YOUR_TOTKW_TABLE_NAME order by id desc"

                    Dim adapter As New SqlDataAdapter(StrConnection, sqlConnection1)
                    Dim table As New DataTable()

                    adapter.Fill(table)

                    Try
                        paramOut = table.Rows(0)(0).ToString
                    Catch ex As Exception
                        paramOut = "0"
                        LogFile("LogError", "doCommand", "paramOut: " + ex.Message)
                    End Try

                    LogFile("LogExtra", "doCommand", "paramOut: " + paramOut)

                    adapter.Dispose()
                    table.Clear()
                    table.Dispose()

                Catch ex As Exception
                    paramOut = ex.Message
                Finally
                    If sqlConnection1.State = ConnectionState.Open Then
                        sqlConnection1.Close()
                    End If
                    sqlConnection1.Dispose()
                    sqlConnection1 = Nothing

                    cmd = Nothing
                End Try

                Return 0

        End Select

        Return -1
    End Function

    Sub doChat()
        Dim serverSocket As New TcpListener(IPAddress.Any, port)
        serverSocket.Server.ReceiveTimeout = 1000
        serverSocket.Server.SendTimeout = 1000

        Dim counter As Integer


        counter = 0
        While (True)
            Try
                counter += 1
                serverSocket.Start()
                msg("Server Started")
                LogFile("LogMsg", "doChat", "Server Started")

                Dim clientSocket As TcpClient = serverSocket.AcceptTcpClient()
                Dim ipend As Net.IPEndPoint = clientSocket.Client.RemoteEndPoint
                Dim clientIPAddress As String = ""
                If Not ipend Is Nothing Then
                    clientIPAddress = ipend.Address.ToString + ":" + ipend.Port.ToString
                End If
                msg("Client No: " + Convert.ToString(counter) + " started!")
                LogFile("LogMsg", "doChat", "Client No: " + Convert.ToString(counter) + " started!")
                If (clientIPAddress <> "") Then
                    msg("Client Address Ip is: " + clientIPAddress)
                    LogFile("LogMsg", "doChat", "Client Address Ip is: " + clientIPAddress)
                End If
                'Dim client As New handleClinet
                'client.startClient(clientSocket, Convert.ToString(counter))

                Dim result(50) As Byte
                Dim dataToClient As String = "risposta"
                Dim dataFromClient As String = ""

                Dim networkStream As NetworkStream = clientSocket.GetStream()

                Dim comando(1) As Byte
                networkStream.Read(comando, 0, 1)

                Dim bytesFrom(CInt(clientSocket.ReceiveBufferSize) + 1) As Byte
                networkStream.Read(bytesFrom, 0, CInt(clientSocket.ReceiveBufferSize))
                dataFromClient = System.Text.Encoding.ASCII.GetString(bytesFrom)
                Try
                    dataFromClient = dataFromClient.Substring(0, dataFromClient.IndexOf("$"))
                Catch ex As Exception

                End Try

                msg(Now().Year.ToString("0000") + "\" + Now().Month.ToString("00") + "\" + Now().Day.ToString("00") + " " + Now.Hour.ToString("00") + ":" + Now.Minute.ToString("00") + ":" + Now.Second.ToString("00"))
                msg("dataFromClient: " + dataFromClient)
                LogFile("LogMsg", "doChat", " - dataFromClient: " + dataFromClient)

                DoCommand(Convert.ToChar(comando(0)), dataFromClient, dataToClient)

                If dataToClient.Length > 0 Then
                    Dim resultStringByte1 As Byte() = System.Text.Encoding.Default.GetBytes(dataToClient)
                    Dim resultByt1e As Byte() = result.Union(resultStringByte1).ToArray()
                    networkStream.Write(resultStringByte1, 0, resultStringByte1.Length)
                    msg("dataToClient: " + dataToClient)
                    LogFile("LogMsg", "doChat", "dataToClient: " + dataToClient)
                    networkStream.Flush()
                End If

                clientSocket.Close()
                networkStream.Close()

            Catch ex As Exception
                LogFile("LogError", "testChat ", ex.Message)
                msg(ex.Message)
            End Try

            serverSocket.Stop()
        End While

        msg("exit")
        LogFile("LogMsg", "doChat", "exit")

        'Console.ReadLine()
    End Sub


    Sub msg(ByVal mesg As String)
        mesg.Trim()
        Console.WriteLine(" >> " + mesg)

    End Sub

    Sub logCSV(ByVal temperatura As String, ByVal umidita As String)
        Dim fileInfo As String = Application.StartupPath + "\LogTemperatura_" + Now().Year.ToString("0000") + Now().Month.ToString("00") + Now().Day.ToString("00") + ".csv"

        Try
            Dim stLine As String = ""
            Dim objWriter As IO.StreamWriter = IO.File.AppendText(fileInfo)
            If IO.File.Exists(fileInfo) Then
                objWriter.Write(Now().Hour.ToString("00") + ":")
                objWriter.Write(Now().Minute.ToString("00") + ":")
                objWriter.Write(Now().Second.ToString("00"))

                objWriter.Write(";")
                objWriter.Write(temperatura)
                objWriter.Write(";")
                objWriter.WriteLine(umidita)
            End If

            objWriter.Close()

        Catch ex As Exception
            LogFile("LogError", "logCSV", ex.Message)
        End Try
    End Sub
    Public Sub LogFile(ByVal _fName As String, ByVal _Subject As String, ByVal _Message As String)
        Try
            '---------------------------------------------------
            Dim dateLog As DateTime = Now
            Dim fileInfo As String = Application.StartupPath + "\" + _fName + Now().Year.ToString("0000") + Now().Month.ToString("00") + Now().Day.ToString("00") + ".log"
            Dim objWrite As New StreamWriter(fileInfo, True)
            objWrite.Write(vbCrLf + "Log Entry : ")
            objWrite.Write("{0} {1}", DateTime.Now.ToLongTimeString(),
               DateTime.Now.ToLongDateString())
            objWrite.WriteLine(" - {0}", _Subject + ": " + _Message)
            objWrite.WriteLine("---------------------------------------------------------------------------------------------")
            objWrite.Close()

        Catch ex As Exception
            Exit Sub
        End Try
    End Sub
    Public Class handleClinet
        Dim clientSocket As TcpClient
        Dim clNo As String
        Public Sub startClient(ByVal inClientSocket As TcpClient, ByVal clineNo As String)
            Me.clientSocket = inClientSocket
            Me.clNo = clineNo
            Dim ctThread As Threading.Thread = New Threading.Thread(AddressOf doChat)
            ctThread.Start()
        End Sub
        Private Sub doChat()

            Dim requestCount As Integer
            Dim dataFromClient As String
            Dim sendBytes As [Byte]()
            Dim serverResponse As String
            Dim rCount As String
            requestCount = 0

            While (True)
                Try
                    requestCount = requestCount + 1
                    Dim networkStream As NetworkStream = clientSocket.GetStream()
                    Dim bytesFrom(CInt(clientSocket.ReceiveBufferSize) + 1) As Byte
                    networkStream.Read(bytesFrom, 0, CInt(clientSocket.ReceiveBufferSize))
                    dataFromClient = System.Text.Encoding.ASCII.GetString(bytesFrom)
                    dataFromClient = dataFromClient.Substring(0, dataFromClient.IndexOf("$"))
                    msg("From client: " + clNo + " " + dataFromClient)

                    rCount = Convert.ToString(requestCount)
                    serverResponse = "Test " + requestCount.ToString
                    sendBytes = Encoding.ASCII.GetBytes(serverResponse)
                    networkStream.Write(sendBytes, 0, sendBytes.Length)
                    networkStream.Flush()
                    msg(serverResponse)
                Catch ex As Exception
                    msg("Excetpion" + clNo + ": " + ex.Message + " Exit!")
                    LogFile("LogError", "doChat", ex.Message)

                    Exit While
                End Try
            End While

            msg("Client No:" + clNo + " Closed!")
        End Sub
    End Class
End Module